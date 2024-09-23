namespace Caspel.Core.Models;

public class Order
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
