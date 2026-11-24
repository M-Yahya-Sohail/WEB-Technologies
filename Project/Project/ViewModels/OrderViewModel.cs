namespace Project.Models;

public class OrderViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public DateTime PlacedAt { get; set; }
    public string Status { get; set; } = "";
    public decimal Total { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}