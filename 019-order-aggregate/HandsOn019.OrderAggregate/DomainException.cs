namespace HandsOn019.OrderAggregate;

/// <summary>
/// Base exception for domain rule violations.
/// </summary>
public class DomainException(string message) : Exception(message);
