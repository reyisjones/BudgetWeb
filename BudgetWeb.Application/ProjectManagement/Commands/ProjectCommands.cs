using System;
using System.Collections.Generic;
using BudgetWeb.Application.ProjectManagement.DTOs;

namespace BudgetWeb.Application.ProjectManagement.Commands
{
    // ==================== PROJECT COMMANDS ====================

    public class CreateProjectCommand
    {
        public string Name { get; set; } = string.Empty;
        public string ProjectType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public decimal Budget { get; set; }
        public string Owner { get; set; } = string.Empty;
    }

    public class UpdateProjectCommand
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
    }

    public class DeleteProjectCommand
    {
        public Guid Id { get; set; }
    }

    // ==================== PHASE COMMANDS ====================

    public class CreatePhaseCommand
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PlannedDuration { get; set; }
        public decimal Budget { get; set; }
        public List<Guid> DependsOn { get; set; } = new();
    }

    public class UpdatePhaseCommand
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
    }

    public class DeletePhaseCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== TASK COMMANDS ====================

    public class CreateTaskCommand
    {
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PlannedHours { get; set; }
        public List<string> AssignedTo { get; set; } = new();
        public List<Guid> DependsOn { get; set; } = new();
    }

    public class UpdateTaskCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
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

    public class UpdateTaskStatusCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CompletionPercentage { get; set; }
    }

    public class DeleteTaskCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== EXPENSE COMMANDS ====================

    public class AddExpenseCommand
    {
        public Guid ProjectId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Vendor { get; set; }
    }

    public class UpdateExpenseCommand
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

    public class DeleteExpenseCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== RESOURCE COMMANDS ====================

    public class AllocateResourceCommand
    {
        public Guid ProjectId { get; set; }
        public Guid TaskId { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public string ResourceDetails { get; set; } = "{}";
        public decimal Quantity { get; set; }
        public DateTime AllocatedDate { get; set; }
    }

    public class UpdateResourceAllocationCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public decimal ConsumedQuantity { get; set; }
        public string? Notes { get; set; }
    }

    public class DeleteResourceAllocationCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== INVENTORY COMMANDS ====================

    public class AddInventoryItemCommand
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal ReorderPoint { get; set; }
        public decimal UnitCost { get; set; }
        public string? Location { get; set; }
        public string? Supplier { get; set; }
    }

    public class UpdateInventoryItemCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? LastRestocked { get; set; }
    }

    public class DeleteInventoryItemCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== CONTRACT COMMANDS ====================

    public class AddContractCommand
    {
        public Guid ProjectId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Party1 { get; set; } = string.Empty;
        public string Party2 { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal? Value { get; set; }
    }

    public class UpdateContractCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime? SignedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class DeleteContractCommand
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }
}
