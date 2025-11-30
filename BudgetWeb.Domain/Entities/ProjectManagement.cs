using System;
using System.Collections.Generic;

namespace BudgetWeb.Domain.Entities
{
    /// <summary>
    /// Root aggregate for project management
    /// </summary>
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProjectType { get; set; } = string.Empty;
        public string Status { get; set; } = "Planning";
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal Budget { get; set; }
        public string Owner { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation properties
        public virtual ICollection<ProjectPhase> Phases { get; set; } = new List<ProjectPhase>();
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public virtual ICollection<ResourceAllocation> ResourceAllocations { get; set; } = new List<ResourceAllocation>();
        public virtual ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public virtual ICollection<ContractDocument> Contracts { get; set; } = new List<ContractDocument>();
    }

    /// <summary>
    /// Project phase - major milestone or stage
    /// </summary>
    public class ProjectPhase
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PlannedDuration { get; set; } // days
        public int? ActualDuration { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public string DependsOn { get; set; } = "[]"; // JSON array of phase IDs
        
        // Navigation properties
        public virtual Project Project { get; set; } = null!;
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }

    /// <summary>
    /// Individual task within a phase
    /// </summary>
    public class ProjectTask
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "NotStarted";
        public string Priority { get; set; } = "Medium";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal ActualHours { get; set; }
        public string AssignedTo { get; set; } = "[]"; // JSON array of user names
        public string DependsOn { get; set; } = "[]"; // JSON array of task IDs
        public int CompletionPercentage { get; set; }
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; } = null!;
        public virtual ProjectPhase Phase { get; set; } = null!;
        public virtual ICollection<ResourceAllocation> ResourceAllocations { get; set; } = new List<ResourceAllocation>();
    }

    /// <summary>
    /// Resource allocation to a task
    /// </summary>
    public class ResourceAllocation
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid TaskId { get; set; }
        public string ResourceType { get; set; } = string.Empty; // "Labor", "Material", "Equipment"
        public string ResourceName { get; set; } = string.Empty;
        public string ResourceDetails { get; set; } = "{}"; // JSON object with type-specific details
        public decimal Quantity { get; set; }
        public DateTime AllocatedDate { get; set; }
        public decimal ConsumedQuantity { get; set; }
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; } = null!;
        public virtual ProjectTask Task { get; set; } = null!;
    }

    /// <summary>
    /// Project expense record
    /// </summary>
    public class ProjectExpense
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ReceiptPath { get; set; }
        public string? Vendor { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; } = null!;
    }

    /// <summary>
    /// Inventory item for project materials
    /// </summary>
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal ReorderPoint { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? LastRestocked { get; set; }
        public string? Location { get; set; }
        public string? Supplier { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; } = null!;
    }

    /// <summary>
    /// Contract or legal document
    /// </summary>
    public class ContractDocument
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
        public string Status { get; set; } = "Draft";
        public string? FilePath { get; set; }
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; } = null!;
    }
}
