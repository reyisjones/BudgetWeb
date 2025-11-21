namespace BudgetWeb.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class InvalidBudgetException : DomainException
{
    public InvalidBudgetException(string message) : base(message)
    {
    }
}

public class InvalidTransactionException : DomainException
{
    public InvalidTransactionException(string message) : base(message)
    {
    }
}

public class InvalidProjectException : DomainException
{
    public InvalidProjectException(string message) : base(message)
    {
    }
}

public class BudgetExceededException : DomainException
{
    public decimal Budget { get; }
    public decimal Spent { get; }

    public BudgetExceededException(decimal budget, decimal spent) 
        : base($"Budget exceeded: {spent} spent of {budget} budgeted")
    {
        Budget = budget;
        Spent = spent;
    }
}

public class CategoryNotFoundException : DomainException
{
    public Guid CategoryId { get; }

    public CategoryNotFoundException(Guid categoryId) 
        : base($"Category with ID {categoryId} not found")
    {
        CategoryId = categoryId;
    }
}

public class ProjectNotFoundException : DomainException
{
    public Guid ProjectId { get; }

    public ProjectNotFoundException(Guid projectId) 
        : base($"Project with ID {projectId} not found")
    {
        ProjectId = projectId;
    }
}

public class BudgetNotFoundException : DomainException
{
    public Guid BudgetId { get; }

    public BudgetNotFoundException(Guid budgetId) 
        : base($"Budget with ID {budgetId} not found")
    {
        BudgetId = budgetId;
    }
}
