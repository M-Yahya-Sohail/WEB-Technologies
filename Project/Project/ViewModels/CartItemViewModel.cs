namespace Project.Models;

public class CartItemViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; }

    public decimal Subtotal => Price * Quantity;
}