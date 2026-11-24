using System.ComponentModel.DataAnnotations;

namespace Project.Models;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending";
    public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}