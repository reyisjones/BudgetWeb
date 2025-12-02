module ProjectManagementTests

open System
open Xunit
open BudgetWeb.CalculationsEngine.ProjectManagement

// ==================== TEST DATA ====================

let createSampleProject() =
    {
        Id = Guid.NewGuid()
        Name = "Test Project"
        ProjectType = Construction
        Status = InProgress
        Description = "Test Description"
        StartDate = DateTime(2024, 1, 1)
        PlannedEndDate = DateTime(2024, 12, 31)
        ActualEndDate = None
        Budget = 1000000m
        Phases = []
        Tasks = []
        Resources = []
        Expenses = []
        Inventory = []
        Contracts = []
        Owner = "Test Owner"
        CreatedDate = DateTime.UtcNow
    }

let createSampleTask phaseId name status plannedHours actualHours completionPct =
    {
        Id = Guid.NewGuid()
        PhaseId = phaseId
        Name = name
        Description = "Task Description"
        Status = status
        Priority = Medium
        StartDate = Some(DateTime(2024, 1, 1))
        EndDate = Some(DateTime(2024, 1, 31))
        PlannedHours = plannedHours
        ActualHours = actualHours
        AssignedTo = []
        DependsOn = []
        CompletionPercentage = completionPct
    }

// ==================== PROGRESS CALCULATION TESTS ====================

[<Fact>]
let ``calculateProjectProgress returns 0 for project with no tasks`` () =
    let project = createSampleProject()
    let progress = calculateProjectProgress project
    Assert.Equal(0m, progress)

[<Fact>]
let ``calculateProjectProgress calculates correct percentage`` () =
    let phaseId = Guid.NewGuid()
    let tasks = [
        createSampleTask phaseId "Task1" Completed 10m 10m 100
        createSampleTask phaseId "Task2" InProgress 20m 10m 50
        createSampleTask phaseId "Task3" NotStarted 10m 0m 0
    ]
    let project = { createSampleProject() with Tasks = tasks }
    let progress = calculateProjectProgress project
    // (100 + 50 + 0) / 3 = 50
    Assert.Equal(50m, progress)

[<Fact>]
let ``getTasksByStatus filters completed tasks correctly`` () =
    let phaseId = Guid.NewGuid()
    let tasks = [
        createSampleTask phaseId "T1" Completed 10m 10m 100
        createSampleTask phaseId "T2" InProgress 20m 10m 50
        createSampleTask phaseId "T3" Completed 15m 15m 100
    ]
    let project = { createSampleProject() with Tasks = tasks }
    
    let completedTasks = getTasksByStatus project Completed
    Assert.Equal(2, List.length completedTasks)

[<Fact>]
let ``getBlockedTasks returns only blocked tasks`` () =
    let phaseId = Guid.NewGuid()
    let tasks = [
        createSampleTask phaseId "T1" Completed 10m 10m 100
        createSampleTask phaseId "T2" Blocked 20m 0m 0
        createSampleTask phaseId "T3" Blocked 15m 0m 0
    ]
    let project = { createSampleProject() with Tasks = tasks }
    
    let blockedTasks = getBlockedTasks project
    Assert.Equal(2, List.length blockedTasks)

// ==================== COST CALCULATION TESTS ====================

[<Fact>]
let ``calculateTotalActualCost sums all expenses`` () =
    let expenses = [
        { Id = Guid.NewGuid(); Category = BudgetCategory.Labor; Description = "Labor"; Amount = 1000m; Date = DateTime.UtcNow; ApprovedBy = None }
        { Id = Guid.NewGuid(); Category = BudgetCategory.Materials; Description = "Materials"; Amount = 2000m; Date = DateTime.UtcNow; ApprovedBy = None }
        { Id = Guid.NewGuid(); Category = BudgetCategory.Equipment; Description = "Equipment"; Amount = 500m; Date = DateTime.UtcNow; ApprovedBy = None }
    ]
    let project = { createSampleProject() with Expenses = expenses }
    let total = calculateTotalActualCost project
    Assert.Equal(3500m, total)

[<Fact>]
let ``calculateBudgetVariance returns positive when under budget`` () =
    let expenses = [
        { Id = Guid.NewGuid(); Category = BudgetCategory.Labor; Description = "Labor"; Amount = 80000m; Date = DateTime.UtcNow; ApprovedBy = None }
    ]
    let project = { createSampleProject() with Budget = 100000m; Expenses = expenses }
    let variance = calculateBudgetVariance project
    Assert.Equal(20000m, variance)

[<Fact>]
let ``calculateBudgetVariance returns negative when over budget`` () =
    let expenses = [
        { Id = Guid.NewGuid(); Category = BudgetCategory.Labor; Description = "Labor"; Amount = 120000m; Date = DateTime.UtcNow; ApprovedBy = None }
    ]
    let project = { createSampleProject() with Budget = 100000m; Expenses = expenses }
    let variance = calculateBudgetVariance project
    Assert.Equal(-20000m, variance)

[<Fact>]
let ``isOverBudget returns true when expenses exceed budget`` () =
    let expenses = [
        { Id = Guid.NewGuid(); Category = BudgetCategory.Labor; Description = "Labor"; Amount = 120000m; Date = DateTime.UtcNow; ApprovedBy = None }
    ]
    let project = { createSampleProject() with Budget = 100000m; Expenses = expenses }
    let overBudget = isOverBudget project
    Assert.True(overBudget)

[<Fact>]
let ``isOverBudget returns false when within budget`` () =
    let expenses = [
        { Id = Guid.NewGuid(); Category = BudgetCategory.Labor; Description = "Labor"; Amount = 80000m; Date = DateTime.UtcNow; ApprovedBy = None }
    ]
    let project = { createSampleProject() with Budget = 100000m; Expenses = expenses }
    let overBudget = isOverBudget project
    Assert.False(overBudget)

// ==================== RESOURCE CALCULATION TESTS ====================

[<Fact>]
let ``calculateLaborCosts calculates total labor cost`` () =
    let resources = [
        {
            Id = Guid.NewGuid()
            TaskId = Guid.NewGuid()
            ResourceType = ResourceType.Labor { Skill = "Developer"; HourlyRate = 50m; CertificationRequired = false }
            Quantity = 100m
            AllocatedDate = DateTime.UtcNow
            ConsumedQuantity = 100m
            Notes = None
        }
        {
            Id = Guid.NewGuid()
            TaskId = Guid.NewGuid()
            ResourceType = ResourceType.Labor { Skill = "Designer"; HourlyRate = 60m; CertificationRequired = false }
            Quantity = 150m
            AllocatedDate = DateTime.UtcNow
            ConsumedQuantity = 150m
            Notes = None
        }
    ]
    let project = { createSampleProject() with Resources = resources }
    let totalCost = calculateLaborCosts project
    Assert.Equal(14000m, totalCost)

[<Fact>]
let ``calculateMaterialCosts sums material resources`` () =
    let resources = [
        {
            Id = Guid.NewGuid()
            TaskId = Guid.NewGuid()
            ResourceType = ResourceType.Material { Unit = "kg"; UnitCost = 50m; Supplier = None; LeadTimeDays = 7 }
            Quantity = 100m
            AllocatedDate = DateTime.UtcNow
            ConsumedQuantity = 100m
            Notes = None
        }
        {
            Id = Guid.NewGuid()
            TaskId = Guid.NewGuid()
            ResourceType = ResourceType.Material { Unit = "m3"; UnitCost = 20m; Supplier = None; LeadTimeDays = 3 }
            Quantity = 200m
            AllocatedDate = DateTime.UtcNow
            ConsumedQuantity = 200m
            Notes = None
        }
    ]
    let project = { createSampleProject() with Resources = resources }
    let totalCost = calculateMaterialCosts project
    Assert.Equal(9000m, totalCost)

[<Fact>]
let ``getItemsBelowReorderPoint filters correctly`` () =
    let items = [
        { Id = Guid.NewGuid(); Name = "Item1"; Category = "Materials"; QuantityOnHand = 45m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m; LastRestocked = None; Location = None }
        { Id = Guid.NewGuid(); Name = "Item2"; Category = "Materials"; QuantityOnHand = 150m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m; LastRestocked = None; Location = None }
        { Id = Guid.NewGuid(); Name = "Item3"; Category = "Materials"; QuantityOnHand = 20m; ReorderPoint = 50m; Unit = "piece"; UnitCost = 10m; LastRestocked = None; Location = None }
    ]
    let lowStockItems = getItemsBelowReorderPoint items
    Assert.Equal(2, List.length lowStockItems)

[<Fact>]
let ``checkResourceAvailability returns true when sufficient`` () =
    let itemId = Guid.NewGuid()
    let items = [
        { Id = itemId; Name = "Item1"; Category = "Materials"; QuantityOnHand = 150m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m; LastRestocked = None; Location = None }
    ]
    let available = checkResourceAvailability items itemId 100m
    Assert.True(available)

[<Fact>]
let ``checkResourceAvailability returns false when insufficient`` () =
    let itemId = Guid.NewGuid()
    let items = [
        { Id = itemId; Name = "Item1"; Category = "Materials"; QuantityOnHand = 50m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m; LastRestocked = None; Location = None }
    ]
    let available = checkResourceAvailability items itemId 100m
    Assert.False(available)


let createSampleExpense amount category =
    {
        Id = Guid.NewGuid()
        Description = "Test Expense"
        Amount = amount
        Category = category
        Date = DateTime(2024, 1, 15)
    }

let createLaborResource hours rate =
    {
        Id = Guid.NewGuid()
        Name = "Test Resource"
        Type = Labor { Role = "Developer"; HourlyRate = rate }
        Quantity = hours
        UnitCost = rate
    }

// ==================== PROGRESS CALCULATION TESTS ====================

[<Fact>]
let ``calculateTaskProgress returns 0 for NotStarted task`` () =
    let task = createSampleTask "Task" NotStarted 10m 0m
    let progress = calculateTaskProgress task
    Assert.Equal(0m, progress)

[<Fact>]
let ``calculateTaskProgress returns 100 for Completed task`` () =
    let task = createSampleTask "Task" Completed 10m 10m
    let progress = calculateTaskProgress task
    Assert.Equal(100m, progress)

[<Fact>]
let ``calculateTaskProgress calculates based on actual vs planned hours`` () =
    let task = createSampleTask "Task" InProgress 20m 10m
    let progress = calculateTaskProgress task
    Assert.Equal(50m, progress)

[<Fact>]
let ``calculatePhaseProgress returns 0 for phase with no tasks`` () =
    let phase = createSamplePhase "Phase" 100000m
    let progress = calculatePhaseProgress phase
    Assert.Equal(0m, progress)

[<Fact>]
let ``calculatePhaseProgress calculates weighted average of tasks`` () =
    let tasks = [
        createSampleTask "Task1" Completed 10m 10m
        createSampleTask "Task2" InProgress 20m 10m
        createSampleTask "Task3" NotStarted 10m 0m
    ]
    let phase = { createSamplePhase "Phase" 100000m with Tasks = tasks }
    let progress = calculatePhaseProgress phase
    // (100 * 10 + 50 * 20 + 0 * 10) / 40 = 50
    Assert.Equal(50m, progress)

[<Fact>]
let ``calculateProjectProgress returns 0 for project with no phases`` () =
    let project = createSampleProject()
    let progress = calculateProjectProgress project
    Assert.Equal(0m, progress)

[<Fact>]
let ``calculateProjectProgress calculates weighted average of phases`` () =
    let phase1Tasks = [createSampleTask "T1" Completed 10m 10m]
    let phase2Tasks = [createSampleTask "T2" InProgress 20m 10m]
    let phase1 = { createSamplePhase "Phase1" 100000m with Tasks = phase1Tasks }
    let phase2 = { createSamplePhase "Phase2" 100000m with Tasks = phase2Tasks }
    let project = { createSampleProject() with Phases = [phase1; phase2] }
    let progress = calculateProjectProgress project
    // (100 * 10 + 50 * 20) / 30 = 66.67
    Assert.Equal(66.67m, Math.Round(progress, 2))

[<Fact>]
let ``getTasksByStatus filters tasks correctly`` () =
    let tasks = [
        createSampleTask "T1" Completed 10m 10m
        createSampleTask "T2" InProgress 20m 10m
        createSampleTask "T3" Blocked 15m 0m
        createSampleTask "T4" NotStarted 10m 0m
    ]
    let phase = { createSamplePhase "Phase" 100000m with Tasks = tasks }
    
    let completedTasks = getTasksByStatus Completed [phase]
    let inProgressTasks = getTasksByStatus InProgress [phase]
    let blockedTasks = getTasksByStatus Blocked [phase]
    
    Assert.Equal(1, List.length completedTasks)
    Assert.Equal(1, List.length inProgressTasks)
    Assert.Equal(1, List.length blockedTasks)

// ==================== COST CALCULATION TESTS ====================

[<Fact>]
let ``calculateTotalActualCost sums all expenses`` () =
    let expenses = [
        createSampleExpense 1000m LaborCost
        createSampleExpense 2000m Materials
        createSampleExpense 500m Equipment
    ]
    let total = calculateTotalActualCost expenses
    Assert.Equal(3500m, total)

[<Fact>]
let ``calculateCostsByCategory groups expenses correctly`` () =
    let expenses = [
        createSampleExpense 1000m LaborCost
        createSampleExpense 2000m LaborCost
        createSampleExpense 1500m Materials
        createSampleExpense 500m Equipment
    ]
    let byCategory = calculateCostsByCategory expenses
    Assert.Equal(3000m, byCategory.[LaborCost])
    Assert.Equal(1500m, byCategory.[Materials])
    Assert.Equal(500m, byCategory.[Equipment])

[<Fact>]
let ``calculateBudgetVariance returns positive when under budget`` () =
    let project = { createSampleProject() with Budget = 100000m }
    let expenses = [createSampleExpense 80000m LaborCost]
    let variance = calculateBudgetVariance project.Budget expenses
    Assert.Equal(20000m, variance)

[<Fact>]
let ``calculateBudgetVariance returns negative when over budget`` () =
    let project = { createSampleProject() with Budget = 100000m }
    let expenses = [createSampleExpense 120000m LaborCost]
    let variance = calculateBudgetVariance project.Budget expenses
    Assert.Equal(-20000m, variance)

[<Fact>]
let ``isOverBudget returns true when expenses exceed budget`` () =
    let project = { createSampleProject() with Budget = 100000m }
    let expenses = [createSampleExpense 120000m LaborCost]
    let overBudget = isOverBudget project.Budget expenses
    Assert.True(overBudget)

[<Fact>]
let ``isOverBudget returns false when within budget`` () =
    let project = { createSampleProject() with Budget = 100000m }
    let expenses = [createSampleExpense 80000m LaborCost]
    let overBudget = isOverBudget project.Budget expenses
    Assert.False(overBudget)

[<Fact>]
let ``calculateCPI returns correct performance index`` () =
    let earnedValue = 100000m
    let actualCost = 80000m
    let cpi = calculateCPI earnedValue actualCost
    Assert.Equal(1.25m, cpi)

[<Fact>]
let ``calculateCPI returns 0 when actual cost is 0`` () =
    let earnedValue = 50000m
    let actualCost = 0m
    let cpi = calculateCPI earnedValue actualCost
    Assert.Equal(0m, cpi)

[<Fact>]
let ``forecastTotalCost predicts accurately with CPI`` () =
    let budget = 1000000m
    let actualCost = 400000m
    let cpi = 0.8m
    let forecast = forecastTotalCost budget actualCost cpi
    // Budget / CPI = 1000000 / 0.8 = 1250000
    Assert.Equal(1250000m, forecast)

[<Fact>]
let ``estimateCostToComplete calculates remaining cost`` () =
    let budget = 1000000m
    let actualCost = 400000m
    let cpi = 1.0m
    let remaining = estimateCostToComplete budget actualCost cpi
    Assert.Equal(600000m, remaining)

// ==================== RESOURCE CALCULATION TESTS ====================

[<Fact>]
let ``calculateTotalLaborHours sums all labor resources`` () =
    let resources = [
        createLaborResource 100m 50m
        createLaborResource 150m 60m
        createLaborResource 80m 45m
    ]
    let totalHours = calculateTotalLaborHours resources
    Assert.Equal(330m, totalHours)

[<Fact>]
let ``calculateLaborCosts calculates total labor cost`` () =
    let resources = [
        createLaborResource 100m 50m  // 5000
        createLaborResource 150m 60m  // 9000
    ]
    let totalCost = calculateLaborCosts resources
    Assert.Equal(14000m, totalCost)

[<Fact>]
let ``calculateMaterialCosts sums material resources`` () =
    let resources = [
        { Id = Guid.NewGuid(); Name = "Steel"; Type = Material "Steel Beams"; Quantity = 100m; UnitCost = 50m }
        { Id = Guid.NewGuid(); Name = "Concrete"; Type = Material "Concrete Mix"; Quantity = 200m; UnitCost = 20m }
    ]
    let totalCost = calculateMaterialCosts resources
    Assert.Equal(9000m, totalCost)

[<Fact>]
let ``calculateEquipmentCosts sums equipment resources`` () =
    let resources = [
        { Id = Guid.NewGuid(); Name = "Excavator"; Type = Equipment { Model = "CAT320"; DailyRate = 500m }; Quantity = 10m; UnitCost = 500m }
        { Id = Guid.NewGuid(); Name = "Crane"; Type = Equipment { Model = "Liebherr"; DailyRate = 800m }; Quantity = 5m; UnitCost = 800m }
    ]
    let totalCost = calculateEquipmentCosts resources
    Assert.Equal(9000m, totalCost)

[<Fact>]
let ``getItemsBelowReorderPoint filters correctly`` () =
    let items = [
        { Id = Guid.NewGuid(); Name = "Item1"; Category = "Materials"; QuantityOnHand = 45m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m }
        { Id = Guid.NewGuid(); Name = "Item2"; Category = "Materials"; QuantityOnHand = 150m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m }
        { Id = Guid.NewGuid(); Name = "Item3"; Category = "Materials"; QuantityOnHand = 20m; ReorderPoint = 50m; Unit = "piece"; UnitCost = 10m }
    ]
    let lowStockItems = getItemsBelowReorderPoint items
    Assert.Equal(2, List.length lowStockItems)

[<Fact>]
let ``checkResourceAvailability returns true when sufficient`` () =
    let items = [
        { Id = Guid.NewGuid(); Name = "Item1"; Category = "Materials"; QuantityOnHand = 150m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m }
    ]
    let available = checkResourceAvailability "Item1" 100m items
    Assert.True(available)

[<Fact>]
let ``checkResourceAvailability returns false when insufficient`` () =
    let items = [
        { Id = Guid.NewGuid(); Name = "Item1"; Category = "Materials"; QuantityOnHand = 50m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m }
    ]
    let available = checkResourceAvailability "Item1" 100m items
    Assert.False(available)

// ==================== SCHEDULE CALCULATION TESTS ====================

[<Fact>]
let ``calculateProjectDuration sums phase durations`` () =
    let phases = [
        { createSamplePhase "P1" 100000m with PlannedDuration = 30 }
        { createSamplePhase "P2" 100000m with PlannedDuration = 45 }
        { createSamplePhase "P3" 100000m with PlannedDuration = 60 }
    ]
    let duration = calculateProjectDuration phases
    Assert.Equal(135, duration)

[<Fact>]
let ``calculateScheduleVariance returns positive when ahead`` () =
    let plannedValue = 100000m
    let earnedValue = 120000m
    let variance = calculateScheduleVariance plannedValue earnedValue
    Assert.Equal(20000m, variance)

[<Fact>]
let ``calculateScheduleVariance returns negative when behind`` () =
    let plannedValue = 100000m
    let earnedValue = 80000m
    let variance = calculateScheduleVariance plannedValue earnedValue
    Assert.Equal(-20000m, variance)

[<Fact>]
let ``calculateSPI calculates schedule performance index`` () =
    let earnedValue = 120000m
    let plannedValue = 100000m
    let spi = calculateSPI earnedValue plannedValue
    Assert.Equal(1.2m, spi)

[<Fact>]
let ``isProjectOnSchedule returns true when SPI >= 1`` () =
    let spi = 1.1m
    let onSchedule = isProjectOnSchedule spi
    Assert.True(onSchedule)

[<Fact>]
let ``isProjectOnSchedule returns false when SPI < 1`` () =
    let spi = 0.9m
    let onSchedule = isProjectOnSchedule spi
    Assert.False(onSchedule)

[<Fact>]
let ``estimateCompletionDate calculates correctly when on schedule`` () =
    let startDate = DateTime(2024, 1, 1)
    let endDate = DateTime(2024, 12, 31)
    let spi = 1.0m
    let estimated = estimateCompletionDate startDate endDate spi
    Assert.Equal(endDate, estimated)

[<Fact>]
let ``estimateCompletionDate extends when behind schedule`` () =
    let startDate = DateTime(2024, 1, 1)
    let endDate = DateTime(2024, 12, 31) // 366 days (leap year)
    let spi = 0.8m // 20% behind
    let estimated = estimateCompletionDate startDate endDate spi
    // 366 / 0.8 = 457.5 days
    let expectedDate = startDate.AddDays(457.5)
    Assert.Equal(expectedDate.Date, estimated.Date)

// ==================== VALIDATION TESTS ====================

[<Fact>]
let ``validateProject returns errors for invalid budget`` () =
    let project = { createSampleProject() with Budget = -1000m }
    let errors = validateProject project
    Assert.Contains("Budget must be greater than 0", errors)

[<Fact>]
let ``validateProject returns errors for invalid dates`` () =
    let project = { createSampleProject() with StartDate = DateTime(2024, 12, 31); EndDate = DateTime(2024, 1, 1) }
    let errors = validateProject project
    Assert.Contains("End date must be after start date", errors)

[<Fact>]
let ``validateProject returns no errors for valid project`` () =
    let project = createSampleProject()
    let errors = validateProject project
    Assert.Empty(errors)

[<Fact>]
let ``validateTaskDependencies detects circular dependencies`` () =
    let task1 = { createSampleTask "T1" NotStarted 10m 0m with DependsOn = [Guid.Parse("00000000-0000-0000-0000-000000000002")] }
    let task2 = { createSampleTask "T2" NotStarted 10m 0m with Id = Guid.Parse("00000000-0000-0000-0000-000000000002"); DependsOn = [task1.Id] }
    let tasks = [task1; task2]
    
    let errors = validateTaskDependencies tasks
    Assert.NotEmpty(errors)

[<Fact>]
let ``validateTaskDependencies allows valid dependencies`` () =
    let task1Id = Guid.NewGuid()
    let task1 = { createSampleTask "T1" NotStarted 10m 0m with Id = task1Id }
    let task2 = { createSampleTask "T2" NotStarted 10m 0m with DependsOn = [task1Id] }
    let tasks = [task1; task2]
    
    let errors = validateTaskDependencies tasks
    Assert.Empty(errors)

// ==================== REPORT GENERATION TESTS ====================

[<Fact>]
let ``generateProgressReport creates complete report`` () =
    let tasks = [
        createSampleTask "T1" Completed 10m 10m
        createSampleTask "T2" InProgress 20m 10m
        createSampleTask "T3" Blocked 15m 0m
        createSampleTask "T4" NotStarted 10m 0m
    ]
    let phase = { createSamplePhase "Phase" 100000m with Tasks = tasks }
    let project = { createSampleProject() with Phases = [phase] }
    
    let report = generateProgressReport project
    
    Assert.Equal(project.Name, report.ProjectName)
    Assert.True(report.CompletionPercentage > 0m)
    Assert.Equal(4, report.TasksTotal)
    Assert.Equal(1, report.TasksCompleted)
    Assert.Equal(1, report.TasksInProgress)
    Assert.Equal(1, report.TasksBlocked)

[<Fact>]
let ``generateCostAnalysisReport creates complete report`` () =
    let expenses = [
        createSampleExpense 40000m LaborCost
        createSampleExpense 30000m Materials
    ]
    let phase = { createSamplePhase "Phase" 100000m with ActualCost = 70000m }
    let project = { createSampleProject() with Phases = [phase]; Budget = 100000m }
    
    let report = generateCostAnalysisReport project expenses
    
    Assert.Equal(project.Name, report.ProjectName)
    Assert.Equal(100000m, report.Budget)
    Assert.Equal(70000m, report.ActualCost)
    Assert.True(report.Variance > 0m)
    Assert.False(report.IsOverBudget)

[<Fact>]
let ``generateResourceUtilizationReport creates complete report`` () =
    let resources = [
        createLaborResource 100m 50m
        { Id = Guid.NewGuid(); Name = "Steel"; Type = Material "Steel"; Quantity = 100m; UnitCost = 30m }
    ]
    let inventoryItems = [
        { Id = Guid.NewGuid(); Name = "Item1"; Category = "Materials"; QuantityOnHand = 45m; ReorderPoint = 100m; Unit = "piece"; UnitCost = 10m }
    ]
    
    let report = generateResourceUtilizationReport resources inventoryItems
    
    Assert.Equal(100m, report.TotalLaborHours)
    Assert.Equal(5000m, report.TotalLaborCost)
    Assert.Equal(3000m, report.TotalMaterialCost)
    Assert.Equal(1, List.length report.LowInventoryItems)
