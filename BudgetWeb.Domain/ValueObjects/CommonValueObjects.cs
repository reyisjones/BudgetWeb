namespace BudgetWeb.Domain.ValueObjects;

using BudgetWeb.Domain.Common;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { Currency = "USD"; } // For EF Core

    public Money(decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier) => new(Amount * multiplier, Currency);

    public bool IsPositive() => Amount > 0;
    public bool IsNegative() => Amount < 0;
    public bool IsZero() => Amount == 0;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}

public class DateRange : ValueObject
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private DateRange() { } // For EF Core

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before or equal to end date");

        StartDate = startDate;
        EndDate = endDate;
    }

    public int DaysCount => (EndDate - StartDate).Days + 1;
    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;
    public bool Overlaps(DateRange other) => StartDate <= other.EndDate && other.StartDate <= EndDate;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}

public class Address : ValueObject
{
    public string Street { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;

    private Address() { } // For EF Core

    public Address(string street, string city, string state, string country, string postalCode)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
    }

    public override string ToString() => $"{Street}, {City}, {State} {PostalCode}, {Country}";
}

public class Percentage : ValueObject
{
    public decimal Value { get; private set; }

    private Percentage() { } // For EF Core

    public Percentage(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Percentage must be between 0 and 100", nameof(value));

        Value = value;
    }

    public decimal AsDecimal() => Value / 100m;

    public static Percentage FromDecimal(decimal decimalValue)
    {
        return new Percentage(decimalValue * 100m);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value}%";
}
