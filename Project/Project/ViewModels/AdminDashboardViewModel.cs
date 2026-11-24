namespace Project.Models;

public class AdminDashboardViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<OrderViewModel> Orders { get; set; } = new();

    public int PendingOrders =>
        Orders.Count(o => o.Status == "Pending");

    public int CompletedOrders =>
        Orders.Count(o => o.Status == "Completed");

    public int LowStockProducts =>
        Products.Count(p => p.IsActive && p.Stock <= 5);
}