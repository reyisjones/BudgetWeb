using System;
using System.Collections.Generic;

namespace BudgetWeb.Application.ProjectManagement.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProjectType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal Budget { get; set; }
        public string Owner { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int PhaseCount { get; set; }
        public int TaskCount { get; set; }
        public decimal CompletionPercentage { get; set; }
    }

    public class ProjectDetailDto : ProjectDto
    {
        public List<ProjectPhaseDto> Phases { get; set; } = new();
        public List<ProjectTaskDto> Tasks { get; set; } = new();
        public List<ProjectExpenseDto> Expenses { get; set; } = new();
        public decimal ActualCost { get; set; }
    }

    public class ProjectPhaseDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PlannedDuration { get; set; }
        public int? ActualDuration { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public List<Guid> DependsOn { get; set; } = new();
        public decimal CompletionPercentage { get; set; }
    }

    public class ProjectTaskDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal ActualHours { get; set; }
        public List<string> AssignedTo { get; set; } = new();
        public List<Guid> DependsOn { get; set; } = new();
        public int CompletionPercentage { get; set; }
        public string? Notes { get; set; }
    }

    public class ProjectExpenseDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Vendor { get; set; }
    }

    public class ResourceAllocationDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal ConsumedQuantity { get; set; }
        public DateTime AllocatedDate { get; set; }
    }

    public class InventoryItemDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal ReorderPoint { get; set; }
        public decimal UnitCost { get; set; }
        public bool BelowReorderPoint { get; set; }
        public string? Location { get; set; }
    }

    public class ContractDocumentDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Party1 { get; set; } = string.Empty;
        public string Party2 { get; set; } = string.Empty;
        public DateTime? SignedDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal? Value { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // Report DTOs
    public class ProgressReportDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public decimal CompletionPercentage { get; set; }
        public int TasksTotal { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksInProgress { get; set; }
        public int TasksBlocked { get; set; }
        public bool OnSchedule { get; set; }
        public int ScheduleVarianceDays { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
    }

    public class CostAnalysisReportDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercent { get; set; }
        public bool IsOverBudget { get; set; }
        public Dictionary<string, decimal> CostsByCategory { get; set; } = new();
        public decimal? CPI { get; set; }
        public decimal ForecastedTotal { get; set; }
        public decimal EstimatedToComplete { get; set; }
    }

    public class ResourceUtilizationReportDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public decimal TotalLaborHours { get; set; }
        public decimal LaborCosts { get; set; }
        public decimal MaterialCosts { get; set; }
        public decimal EquipmentCosts { get; set; }
        public List<InventoryItemDto> LowInventoryItems { get; set; } = new();
    }
}
