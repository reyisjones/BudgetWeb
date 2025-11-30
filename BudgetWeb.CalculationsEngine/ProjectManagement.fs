namespace BudgetWeb.CalculationsEngine

open System

/// Project Management Engine - Enterprise-grade project administration module
/// Supports multiple project types: construction, film production, manufacturing, etc.
module ProjectManagement =

    // ==================== DOMAIN TYPES ====================

    /// Supported project types - extensible via configuration
    type ProjectType =
        | Construction
        | FilmProduction
        | Manufacturing
        | HomeImprovement
        | Custom of string

    /// Project status throughout lifecycle
    type ProjectStatus =
        | Planning
        | InProgress
        | OnHold
        | Completed
        | Cancelled

    /// Task status and priority
    type TaskStatus =
        | NotStarted
        | InProgress
        | Blocked
        | Completed
        | Cancelled

    type TaskPriority =
        | Low
        | Medium
        | High
        | Critical

    /// Resource types used in projects
    type ResourceType =
        | Labor of LaborDetails
        | Material of MaterialDetails
        | Equipment of EquipmentDetails

    and LaborDetails = {
        Skill: string
        HourlyRate: decimal
        CertificationRequired: bool
    }

    and MaterialDetails = {
        Unit: string
        UnitCost: decimal
        Supplier: string option
        LeadTimeDays: int
    }

    and EquipmentDetails = {
        DailyRate: decimal
        RequiresOperator: bool
        MaintenanceCost: decimal
    }

    /// Budget category for expense tracking
    type BudgetCategory =
        | Labor
        | Materials
        | Equipment
        | Permits
        | Overhead
        | Contingency
        | Other of string

    /// Expense record
    type Expense = {
        Id: Guid
        Category: BudgetCategory
        Description: string
        Amount: decimal
        Date: DateTime
        ApprovedBy: string option
    }

    /// Project phase definition
    type ProjectPhase = {
        Id: Guid
        Name: string
        Description: string
        StartDate: DateTime option
        EndDate: DateTime option
        PlannedDuration: int // days
        ActualDuration: int option
        DependsOn: Guid list // Phase IDs
        Budget: decimal
        ActualCost: decimal
    }

    /// Project task
    type ProjectTask = {
        Id: Guid
        PhaseId: Guid
        Name: string
        Description: string
        Status: TaskStatus
        Priority: TaskPriority
        StartDate: DateTime option
        EndDate: DateTime option
        PlannedHours: decimal
        ActualHours: decimal
        AssignedTo: string list
        DependsOn: Guid list // Task IDs
        CompletionPercentage: int
    }

    /// Resource allocation
    type ResourceAllocation = {
        Id: Guid
        TaskId: Guid
        ResourceType: ResourceType
        Quantity: decimal
        AllocatedDate: DateTime
        ConsumedQuantity: decimal
        Notes: string option
    }

    /// Inventory item
    type InventoryItem = {
        Id: Guid
        Name: string
        Category: string
        Unit: string
        QuantityOnHand: decimal
        ReorderPoint: decimal
        UnitCost: decimal
        LastRestocked: DateTime option
        Location: string option
    }

    /// Contract document
    type ContractDocument = {
        Id: Guid
        Type: string // "Main Contract", "Subcontract", "NDA", etc.
        Title: string
        Party1: string
        Party2: string
        SignedDate: DateTime option
        EffectiveDate: DateTime
        ExpirationDate: DateTime option
        Value: decimal option
        Status: string // "Draft", "Signed", "Expired"
        FilePath: string option
    }

    /// Complete project model
    type Project = {
        Id: Guid
        Name: string
        ProjectType: ProjectType
        Status: ProjectStatus
        Description: string
        StartDate: DateTime
        PlannedEndDate: DateTime
        ActualEndDate: DateTime option
        Budget: decimal
        Phases: ProjectPhase list
        Tasks: ProjectTask list
        Resources: ResourceAllocation list
        Expenses: Expense list
        Inventory: InventoryItem list
        Contracts: ContractDocument list
        Owner: string
        CreatedDate: DateTime
    }

    // ==================== PROGRESS CALCULATION ====================

    /// Calculate overall project completion percentage
    let calculateProjectProgress (project: Project) : decimal =
        if project.Tasks.IsEmpty then
            0m
        else
            let totalProgress = 
                project.Tasks 
                |> List.sumBy (fun t -> decimal t.CompletionPercentage)
            let taskCount = decimal project.Tasks.Length
            totalProgress / taskCount

    /// Calculate phase completion percentage
    let calculatePhaseProgress (project: Project) (phaseId: Guid) : decimal =
        let phaseTasks = 
            project.Tasks 
            |> List.filter (fun t -> t.PhaseId = phaseId)
        
        if phaseTasks.IsEmpty then
            0m
        else
            let totalProgress = 
                phaseTasks 
                |> List.sumBy (fun t -> decimal t.CompletionPercentage)
            let taskCount = decimal phaseTasks.Length
            totalProgress / taskCount

    /// Get tasks by status
    let getTasksByStatus (project: Project) (status: TaskStatus) : ProjectTask list =
        project.Tasks |> List.filter (fun t -> t.Status = status)

    /// Get blocked tasks
    let getBlockedTasks (project: Project) : ProjectTask list =
        getTasksByStatus project Blocked

    /// Check if project is on schedule
    let isProjectOnSchedule (project: Project) : bool =
        let now = DateTime.Now
        let daysElapsed = (now - project.StartDate).TotalDays
        let totalPlannedDays = (project.PlannedEndDate - project.StartDate).TotalDays
        let expectedProgress = (daysElapsed / totalPlannedDays) * 100.0
        let actualProgress = float (calculateProjectProgress project)
        
        actualProgress >= (expectedProgress - 5.0) // 5% tolerance

    // ==================== COST ANALYSIS ====================

    /// Calculate total actual costs
    let calculateTotalActualCost (project: Project) : decimal =
        project.Expenses |> List.sumBy (fun e -> e.Amount)

    /// Calculate costs by category
    let calculateCostsByCategory (project: Project) : Map<BudgetCategory, decimal> =
        project.Expenses
        |> List.groupBy (fun e -> e.Category)
        |> List.map (fun (category, expenses) -> 
            category, expenses |> List.sumBy (fun e -> e.Amount))
        |> Map.ofList

    /// Calculate budget variance (positive = under budget)
    let calculateBudgetVariance (project: Project) : decimal =
        project.Budget - calculateTotalActualCost project

    /// Calculate budget variance percentage
    let calculateBudgetVariancePercent (project: Project) : decimal =
        if project.Budget = 0m then
            0m
        else
            (calculateBudgetVariance project / project.Budget) * 100m

    /// Check if project is over budget
    let isOverBudget (project: Project) : bool =
        calculateBudgetVariance project < 0m

    /// Calculate cost performance index (CPI)
    /// CPI > 1 = under budget, CPI < 1 = over budget
    let calculateCPI (project: Project) : decimal option =
        let actualCost = calculateTotalActualCost project
        if actualCost = 0m then
            None
        else
            let progress = calculateProjectProgress project
            let earnedValue = project.Budget * (progress / 100m)
            Some (earnedValue / actualCost)

    /// Forecast total cost at completion
    let forecastTotalCost (project: Project) : decimal =
        match calculateCPI project with
        | None -> project.Budget
        | Some cpi when cpi = 0m -> project.Budget
        | Some cpi -> project.Budget / cpi

    /// Calculate estimated cost to complete
    let estimateCostToComplete (project: Project) : decimal =
        let forecastTotal = forecastTotalCost project
        let actualSpent = calculateTotalActualCost project
        Math.Max(0m, forecastTotal - actualSpent)

    // ==================== RESOURCE MANAGEMENT ====================

    /// Calculate total labor hours used
    let calculateTotalLaborHours (project: Project) : decimal =
        project.Tasks |> List.sumBy (fun t -> t.ActualHours)

    /// Calculate labor hours by task
    let calculateLaborHoursByTask (project: Project) : Map<Guid, decimal> =
        project.Tasks
        |> List.map (fun t -> t.Id, t.ActualHours)
        |> Map.ofList

    /// Get resource allocations by type
    let getResourcesByType (project: Project) (filterType: ResourceType -> bool) : ResourceAllocation list =
        project.Resources |> List.filter (fun r -> filterType r.ResourceType)

    /// Calculate labor costs
    let calculateLaborCosts (project: Project) : decimal =
        project.Resources
        |> List.choose (fun r ->
            match r.ResourceType with
            | Labor details -> Some (r.ConsumedQuantity * details.HourlyRate)
            | _ -> None)
        |> List.sum

    /// Calculate material costs
    let calculateMaterialCosts (project: Project) : decimal =
        project.Resources
        |> List.choose (fun r ->
            match r.ResourceType with
            | Material details -> Some (r.ConsumedQuantity * details.UnitCost)
            | _ -> None)
        |> List.sum

    /// Calculate equipment costs
    let calculateEquipmentCosts (project: Project) : decimal =
        project.Resources
        |> List.choose (fun r ->
            match r.ResourceType with
            | Equipment details -> Some (r.ConsumedQuantity * details.DailyRate)
            | _ -> None)
        |> List.sum

    /// Check resource availability
    let checkResourceAvailability (inventory: InventoryItem list) (itemId: Guid) (requiredQuantity: decimal) : bool =
        match inventory |> List.tryFind (fun i -> i.Id = itemId) with
        | Some item -> item.QuantityOnHand >= requiredQuantity
        | None -> false

    /// Get items below reorder point
    let getItemsBelowReorderPoint (inventory: InventoryItem list) : InventoryItem list =
        inventory |> List.filter (fun i -> i.QuantityOnHand <= i.ReorderPoint)

    // ==================== SCHEDULE ANALYSIS ====================

    /// Calculate project duration in days
    let calculateProjectDuration (project: Project) : int option =
        match project.ActualEndDate with
        | Some endDate -> Some ((endDate - project.StartDate).Days)
        | None -> 
            let daysElapsed = (DateTime.Now - project.StartDate).Days
            Some daysElapsed

    /// Calculate schedule variance in days
    let calculateScheduleVariance (project: Project) : int =
        let plannedDays = (project.PlannedEndDate - project.StartDate).Days
        match calculateProjectDuration project with
        | Some actualDays -> plannedDays - actualDays
        | None -> 
            let daysElapsed = (DateTime.Now - project.StartDate).Days
            plannedDays - daysElapsed

    /// Calculate schedule performance index (SPI)
    /// SPI > 1 = ahead of schedule, SPI < 1 = behind schedule
    let calculateSPI (project: Project) : decimal =
        let now = DateTime.Now
        let daysElapsed = (now - project.StartDate).TotalDays
        let plannedDays = (project.PlannedEndDate - project.StartDate).TotalDays
        let plannedProgress = (daysElapsed / plannedDays) * 100.0
        let actualProgress = float (calculateProjectProgress project)
        
        if plannedProgress = 0.0 then
            1m
        else
            decimal (actualProgress / plannedProgress)

    /// Estimate completion date
    let estimateCompletionDate (project: Project) : DateTime =
        let spi = calculateSPI project
        if spi = 0m then
            project.PlannedEndDate
        else
            let remainingDays = (project.PlannedEndDate - DateTime.Now).TotalDays
            let adjustedRemainingDays = remainingDays / float spi
            DateTime.Now.AddDays(adjustedRemainingDays)

    // ==================== REPORTING ====================

    /// Progress report data
    type ProgressReport = {
        ProjectName: string
        CompletionPercentage: decimal
        TasksTotal: int
        TasksCompleted: int
        TasksInProgress: int
        TasksBlocked: int
        OnSchedule: bool
        ScheduleVarianceDays: int
        EstimatedCompletionDate: DateTime
    }

    /// Cost analysis report data
    type CostAnalysisReport = {
        ProjectName: string
        Budget: decimal
        ActualCost: decimal
        Variance: decimal
        VariancePercent: decimal
        IsOverBudget: bool
        CostsByCategory: Map<BudgetCategory, decimal>
        CPI: decimal option
        ForecastedTotal: decimal
        EstimatedToComplete: decimal
    }

    /// Resource utilization report data
    type ResourceUtilizationReport = {
        ProjectName: string
        TotalLaborHours: decimal
        LaborCosts: decimal
        MaterialCosts: decimal
        EquipmentCosts: decimal
        LowInventoryItems: InventoryItem list
    }

    /// Generate progress report
    let generateProgressReport (project: Project) : ProgressReport =
        let completedTasks = getTasksByStatus project Completed
        let inProgressTasks = getTasksByStatus project InProgress
        let blockedTasks = getBlockedTasks project
        
        {
            ProjectName = project.Name
            CompletionPercentage = calculateProjectProgress project
            TasksTotal = project.Tasks.Length
            TasksCompleted = completedTasks.Length
            TasksInProgress = inProgressTasks.Length
            TasksBlocked = blockedTasks.Length
            OnSchedule = isProjectOnSchedule project
            ScheduleVarianceDays = calculateScheduleVariance project
            EstimatedCompletionDate = estimateCompletionDate project
        }

    /// Generate cost analysis report
    let generateCostAnalysisReport (project: Project) : CostAnalysisReport =
        {
            ProjectName = project.Name
            Budget = project.Budget
            ActualCost = calculateTotalActualCost project
            Variance = calculateBudgetVariance project
            VariancePercent = calculateBudgetVariancePercent project
            IsOverBudget = isOverBudget project
            CostsByCategory = calculateCostsByCategory project
            CPI = calculateCPI project
            ForecastedTotal = forecastTotalCost project
            EstimatedToComplete = estimateCostToComplete project
        }

    /// Generate resource utilization report
    let generateResourceUtilizationReport (project: Project) : ResourceUtilizationReport =
        {
            ProjectName = project.Name
            TotalLaborHours = calculateTotalLaborHours project
            LaborCosts = calculateLaborCosts project
            MaterialCosts = calculateMaterialCosts project
            EquipmentCosts = calculateEquipmentCosts project
            LowInventoryItems = getItemsBelowReorderPoint project.Inventory
        }

    // ==================== VALIDATION ====================

    /// Validate project data
    let validateProject (project: Project) : Result<unit, string list> =
        let errors = ResizeArray<string>()
        
        if String.IsNullOrWhiteSpace(project.Name) then
            errors.Add("Project name is required")
        
        if project.Budget <= 0m then
            errors.Add("Budget must be greater than zero")
        
        if project.PlannedEndDate <= project.StartDate then
            errors.Add("Planned end date must be after start date")
        
        if project.Phases.IsEmpty then
            errors.Add("Project must have at least one phase")
        
        if errors.Count > 0 then
            Error (errors |> List.ofSeq)
        else
            Ok ()

    /// Validate task dependencies (no circular dependencies)
    let validateTaskDependencies (tasks: ProjectTask list) : Result<unit, string> =
        let rec hasCycle (taskId: Guid) (visited: Set<Guid>) (path: Set<Guid>) =
            if path.Contains(taskId) then
                true
            elif visited.Contains(taskId) then
                false
            else
                let task = tasks |> List.find (fun t -> t.Id = taskId)
                let newPath = path.Add(taskId)
                let newVisited = visited.Add(taskId)
                task.DependsOn |> List.exists (fun depId -> hasCycle depId newVisited newPath)
        
        let hasCircular = 
            tasks 
            |> List.exists (fun t -> hasCycle t.Id Set.empty Set.empty)
        
        if hasCircular then
            Error "Circular task dependencies detected"
        else
            Ok ()
