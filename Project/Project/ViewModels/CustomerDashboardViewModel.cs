namespace Project.Models;

public class CustomerDashboardViewModel
{
    public string FullName { get; set; } = "";

    public List<OrderViewModel> Orders { get; set; } = new();

    public int TotalOrders => Orders.Count;

    public int PendingOrders =>
        Orders.Count(o => o.Status == "Pending");

    public int CompletedOrders =>
        Orders.Count(o => o.Status == "Completed");
}