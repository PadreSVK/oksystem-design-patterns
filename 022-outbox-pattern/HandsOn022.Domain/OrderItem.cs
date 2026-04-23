namespace HandsOn022.Domain;

public class OrderItem
{
    public Guid Id { get; private set; }
    public string ProductName { get; private set; } = "";
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem() { }

    public static OrderItem Create(string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.", nameof(productName));

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
