namespace Caspel.Core.Interfaces;

public interface OrderCreated
{
    Guid Id { get; set; }
    string ProductName { get; set; }
    int Quantity { get; set; }
    DateTimeOffset CreatedAt { get; set; }
}
