namespace BudgetWeb.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}

public abstract class DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}
