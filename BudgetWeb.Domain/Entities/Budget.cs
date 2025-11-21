namespace BudgetWeb.Domain.Entities;

using BudgetWeb.Domain.Common;
using BudgetWeb.Domain.Enums;
using BudgetWeb.Domain.ValueObjects;
using BudgetWeb.Domain.Events;

public class Budget : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public BudgetStatus Status { get; private set; }
    public BudgetPeriodType PeriodType { get; private set; }
    public DateRange Period { get; private set; } = null!;
    public Money TotalAmount { get; private set; } = null!;
    public Money AllocatedAmount { get; private set; } = null!;
    public Money SpentAmount { get; private set; } = null!;
    public Money RemainingAmount => TotalAmount.Subtract(SpentAmount);
    
    public string? OwnerId { get; private set; }
    public string? TenantId { get; private set; }
    public bool IsTemplate { get; private set; }
    
    private readonly List<BudgetCategory> _categories = new();
    public IReadOnlyCollection<BudgetCategory> Categories => _categories.AsReadOnly();
    
    private readonly List<BudgetGoal> _goals = new();
    public IReadOnlyCollection<BudgetGoal> Goals => _goals.AsReadOnly();

    private Budget() { } // For EF Core

    public Budget(string name, string description, Money totalAmount, DateRange period, 
                  BudgetPeriodType periodType, string? ownerId = null, string? tenantId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Budget name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        TotalAmount = totalAmount ?? throw new ArgumentNullException(nameof(totalAmount));
        AllocatedAmount = Money.Zero(totalAmount.Currency);
        SpentAmount = Money.Zero(totalAmount.Currency);
        Period = period ?? throw new ArgumentNullException(nameof(period));
        PeriodType = periodType;
        Status = BudgetStatus.Draft;
        OwnerId = ownerId;
        TenantId = tenantId;
        IsTemplate = false;

        AddDomainEvent(new BudgetCreatedEvent(this));
    }

    public void Activate()
    {
        if (Status == BudgetStatus.Active)
            throw new InvalidOperationException("Budget is already active");

        Status = BudgetStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new BudgetStatusChangedEvent(this, BudgetStatus.Draft, BudgetStatus.Active));
    }

    public void Complete()
    {
        Status = BudgetStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new BudgetStatusChangedEvent(this, Status, BudgetStatus.Completed));
    }

    public void AddCategory(Category category, Money allocatedAmount)
    {
        if (_categories.Any(bc => bc.CategoryId == category.Id))
            throw new InvalidOperationException("Category already exists in budget");

        var budgetCategory = new BudgetCategory(Id, category.Id, allocatedAmount);
        _categories.Add(budgetCategory);
        
        AllocatedAmount = AllocatedAmount.Add(allocatedAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSpentAmount(Money amount)
    {
        SpentAmount = amount;
        UpdatedAt = DateTime.UtcNow;

        var utilizationRate = (SpentAmount.Amount / TotalAmount.Amount) * 100m;
        if (utilizationRate > 90m)
        {
            AddDomainEvent(new BudgetThresholdExceededEvent(this, utilizationRate));
        }
    }

    public void AddGoal(string description, Money targetAmount, DateTime targetDate)
    {
        var goal = new BudgetGoal(Id, description, targetAmount, targetDate);
        _goals.Add(goal);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertToTemplate()
    {
        IsTemplate = true;
        Status = BudgetStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetUtilizationPercentage()
    {
        if (TotalAmount.Amount == 0) return 0;
        return (SpentAmount.Amount / TotalAmount.Amount) * 100m;
    }
}

public class BudgetCategory : BaseEntity
{
    public Guid BudgetId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Money AllocatedAmount { get; private set; } = null!;
    public Money SpentAmount { get; private set; } = null!;
    
    public Budget? Budget { get; private set; }
    public Category? Category { get; private set; }

    private BudgetCategory() { } // For EF Core

    public BudgetCategory(Guid budgetId, Guid categoryId, Money allocatedAmount)
    {
        BudgetId = budgetId;
        CategoryId = categoryId;
        AllocatedAmount = allocatedAmount ?? throw new ArgumentNullException(nameof(allocatedAmount));
        SpentAmount = Money.Zero(allocatedAmount.Currency);
    }

    public void UpdateSpentAmount(Money amount)
    {
        SpentAmount = amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public Money GetRemainingAmount() => AllocatedAmount.Subtract(SpentAmount);
    
    public decimal GetUtilizationPercentage()
    {
        if (AllocatedAmount.Amount == 0) return 0;
        return (SpentAmount.Amount / AllocatedAmount.Amount) * 100m;
    }
}

public class BudgetGoal : BaseEntity
{
    public Guid BudgetId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Money TargetAmount { get; private set; } = null!;
    public Money CurrentAmount { get; private set; } = null!;
    public DateTime TargetDate { get; private set; }
    public bool IsAchieved { get; private set; }
    public DateTime? AchievedDate { get; private set; }

    private BudgetGoal() { } // For EF Core

    public BudgetGoal(Guid budgetId, string description, Money targetAmount, DateTime targetDate)
    {
        BudgetId = budgetId;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        TargetAmount = targetAmount ?? throw new ArgumentNullException(nameof(targetAmount));
        CurrentAmount = Money.Zero(targetAmount.Currency);
        TargetDate = targetDate;
        IsAchieved = false;
    }

    public void UpdateProgress(Money currentAmount)
    {
        CurrentAmount = currentAmount;
        
        if (CurrentAmount.Amount >= TargetAmount.Amount && !IsAchieved)
        {
            IsAchieved = true;
            AchievedDate = DateTime.UtcNow;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetProgressPercentage()
    {
        if (TargetAmount.Amount == 0) return 0;
        return (CurrentAmount.Amount / TargetAmount.Amount) * 100m;
    }
}
