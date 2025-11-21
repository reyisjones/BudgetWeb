namespace BudgetWeb.Domain.Entities;

using BudgetWeb.Domain.Common;
using BudgetWeb.Domain.Enums;
using BudgetWeb.Domain.ValueObjects;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public CategoryType Type { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public string? TenantId { get; private set; }
    public bool IsSystem { get; private set; }
    
    private readonly List<Category> _subcategories = new();
    public IReadOnlyCollection<Category> Subcategories => _subcategories.AsReadOnly();

    private Category() { } // For EF Core

    public Category(string name, CategoryType type, string description = "", 
                    Guid? parentCategoryId = null, string? icon = null, string? color = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        Type = type;
        ParentCategoryId = parentCategoryId;
        Icon = icon;
        Color = color;
        IsSystem = false;
    }

    public void UpdateDetails(string name, string description, string? icon, string? color)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        
        Description = description;
        Icon = icon;
        Color = color;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSubcategory(Category subcategory)
    {
        if (subcategory.Type != Type)
            throw new InvalidOperationException("Subcategory must have the same type as parent");
            
        _subcategories.Add(subcategory);
        UpdatedAt = DateTime.UtcNow;
    }
}

public class Transaction : BaseEntity
{
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = null!;
    public DateTime TransactionDate { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    
    public Guid? BudgetId { get; private set; }
    public Budget? Budget { get; private set; }
    
    public Guid? ProjectId { get; private set; }
    public Project? Project { get; private set; }
    
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public string? AttachmentUrl { get; private set; }
    public string? Vendor { get; private set; }
    public string? OwnerId { get; private set; }
    public string? TenantId { get; private set; }
    
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private Transaction() { } // For EF Core

    public Transaction(string description, Money amount, DateTime transactionDate, 
                       TransactionType type, Guid categoryId, Guid? budgetId = null, 
                       Guid? projectId = null, string? ownerId = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Description = description;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        TransactionDate = transactionDate;
        Type = type;
        CategoryId = categoryId;
        BudgetId = budgetId;
        ProjectId = projectId;
        OwnerId = ownerId;
        Status = TransactionStatus.Pending;
    }

    public void Complete()
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException("Transaction is already completed");

        Status = TransactionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed transaction");

        Status = TransactionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTags(params string[] tags)
    {
        foreach (var tag in tags.Where(t => !string.IsNullOrWhiteSpace(t)))
        {
            if (!_tags.Contains(tag))
                _tags.Add(tag);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string description, string? notes, string? reference, string? vendor)
    {
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
        
        Notes = notes;
        Reference = reference;
        Vendor = vendor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachDocument(string attachmentUrl)
    {
        if (string.IsNullOrWhiteSpace(attachmentUrl))
            throw new ArgumentException("Attachment URL cannot be empty", nameof(attachmentUrl));

        AttachmentUrl = attachmentUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class Project : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ProjectStatus Status { get; private set; }
    public ProjectPriority Priority { get; private set; }
    public DateRange ProjectPeriod { get; private set; } = null!;
    public Money TotalBudget { get; private set; } = null!;
    public Money SpentAmount { get; private set; } = null!;
    public Money RemainingBudget => TotalBudget.Subtract(SpentAmount);
    
    public string? ProjectManager { get; private set; }
    public string? Client { get; private set; }
    public string? TenantId { get; private set; }
    public Percentage CompletionPercentage { get; private set; } = null!;
    
    private readonly List<ProjectPhase> _phases = new();
    public IReadOnlyCollection<ProjectPhase> Phases => _phases.AsReadOnly();
    
    private readonly List<ProjectMilestone> _milestones = new();
    public IReadOnlyCollection<ProjectMilestone> Milestones => _milestones.AsReadOnly();

    private Project() { CompletionPercentage = new Percentage(0); } // For EF Core

    public Project(string name, string description, Money totalBudget, DateRange projectPeriod, 
                   string? projectManager = null, string? client = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        TotalBudget = totalBudget ?? throw new ArgumentNullException(nameof(totalBudget));
        SpentAmount = Money.Zero(totalBudget.Currency);
        ProjectPeriod = projectPeriod ?? throw new ArgumentNullException(nameof(projectPeriod));
        ProjectManager = projectManager;
        Client = client;
        Status = ProjectStatus.Planning;
        Priority = ProjectPriority.Medium;
        CompletionPercentage = new Percentage(0);
    }

    public void Start()
    {
        if (Status != ProjectStatus.Planning)
            throw new InvalidOperationException("Only planning projects can be started");

        Status = ProjectStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = ProjectStatus.Completed;
        CompletionPercentage = new Percentage(100);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage)
    {
        CompletionPercentage = new Percentage(percentage);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSpentAmount(Money amount)
    {
        SpentAmount = amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPhase(string name, Money budget, DateRange phasePeriod)
    {
        var phase = new ProjectPhase(Id, name, budget, phasePeriod);
        _phases.Add(phase);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMilestone(string name, DateTime dueDate, string description)
    {
        var milestone = new ProjectMilestone(Id, name, dueDate, description);
        _milestones.Add(milestone);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPriority(ProjectPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetBudgetUtilization()
    {
        if (TotalBudget.Amount == 0) return 0;
        return (SpentAmount.Amount / TotalBudget.Amount) * 100m;
    }
}

public class ProjectPhase : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money Budget { get; private set; } = null!;
    public Money SpentAmount { get; private set; } = null!;
    public DateRange PhasePeriod { get; private set; } = null!;
    public ProjectStatus Status { get; private set; }
    public Percentage CompletionPercentage { get; private set; } = null!;

    private ProjectPhase() { CompletionPercentage = new Percentage(0); } // For EF Core

    public ProjectPhase(Guid projectId, string name, Money budget, DateRange phasePeriod)
    {
        ProjectId = projectId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        SpentAmount = Money.Zero(budget.Currency);
        PhasePeriod = phasePeriod ?? throw new ArgumentNullException(nameof(phasePeriod));
        Status = ProjectStatus.Planning;
        CompletionPercentage = new Percentage(0);
    }

    public void UpdateProgress(decimal percentage)
    {
        CompletionPercentage = new Percentage(percentage);
        if (percentage >= 100)
            Status = ProjectStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class ProjectMilestone : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime DueDate { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedDate { get; private set; }

    private ProjectMilestone() { } // For EF Core

    public ProjectMilestone(Guid projectId, string name, DateTime dueDate, string description = "")
    {
        ProjectId = projectId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DueDate = dueDate;
        Description = description;
        IsCompleted = false;
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        CompletedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
