namespace BudgetWeb.Domain.Enums;

public enum BudgetStatus
{
    Draft,
    Active,
    Completed,
    Archived,
    OnHold
}

public enum BudgetPeriodType
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly,
    Custom
}

public enum CategoryType
{
    Income,
    Expense,
    Asset,
    Liability,
    Equity
}

public enum TransactionType
{
    Income,
    Expense,
    Transfer,
    Adjustment
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Cancelled,
    Failed
}

public enum ProjectStatus
{
    Planning,
    Active,
    OnHold,
    Completed,
    Cancelled
}

public enum ProjectPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum AlertType
{
    BudgetExceeded,
    BudgetNearLimit,
    UnusualTransaction,
    UpcomingPayment,
    GoalReached
}

public enum ReportType
{
    BudgetSummary,
    CashFlowAnalysis,
    VarianceReport,
    ForecastReport,
    ProjectCostReport,
    ROIAnalysis,
    AuditReport
}

public enum AuditAction
{
    Created,
    Updated,
    Deleted,
    StatusChanged,
    AmountAdjusted
}
