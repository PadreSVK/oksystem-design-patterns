namespace HandsOn019.OrderAggregate;

// =============================================================================
// Task 1 Solution — Money Value Object
// =============================================================================

public readonly record struct Money(decimal Amount, string Currency)
{
    public static readonly Money Zero = new(0, "USD");

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot add different currencies.");

        return new Money(a.Amount + b.Amount, a.Currency);
    }
}
