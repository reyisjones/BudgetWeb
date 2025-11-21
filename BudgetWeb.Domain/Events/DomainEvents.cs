namespace BudgetWeb.Domain.Events;

using BudgetWeb.Domain.Common;
using BudgetWeb.Domain.Entities;
using BudgetWeb.Domain.Enums;

public class BudgetCreatedEvent : DomainEventBase
{
    public Budget Budget { get; }

    public BudgetCreatedEvent(Budget budget)
    {
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
    }
}

public class BudgetStatusChangedEvent : DomainEventBase
{
    public Budget Budget { get; }
    public BudgetStatus OldStatus { get; }
    public BudgetStatus NewStatus { get; }

    public BudgetStatusChangedEvent(Budget budget, BudgetStatus oldStatus, BudgetStatus newStatus)
    {
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public class BudgetThresholdExceededEvent : DomainEventBase
{
    public Budget Budget { get; }
    public decimal UtilizationRate { get; }

    public BudgetThresholdExceededEvent(Budget budget, decimal utilizationRate)
    {
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        UtilizationRate = utilizationRate;
    }
}

public class TransactionCreatedEvent : DomainEventBase
{
    public Transaction Transaction { get; }

    public TransactionCreatedEvent(Transaction transaction)
    {
        Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }
}

public class TransactionCompletedEvent : DomainEventBase
{
    public Transaction Transaction { get; }

    public TransactionCompletedEvent(Transaction transaction)
    {
        Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }
}

public class ProjectCreatedEvent : DomainEventBase
{
    public Project Project { get; }

    public ProjectCreatedEvent(Project project)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
    }
}

public class ProjectCompletedEvent : DomainEventBase
{
    public Project Project { get; }

    public ProjectCompletedEvent(Project project)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
    }
}

public class BudgetGoalAchievedEvent : DomainEventBase
{
    public Guid BudgetId { get; }
    public Guid GoalId { get; }
    public string GoalDescription { get; }

    public BudgetGoalAchievedEvent(Guid budgetId, Guid goalId, string goalDescription)
    {
        BudgetId = budgetId;
        GoalId = goalId;
        GoalDescription = goalDescription ?? throw new ArgumentNullException(nameof(goalDescription));
    }
}
