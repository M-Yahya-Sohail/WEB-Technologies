using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Project.Models;

// Identity account stored in the D2-only LocalDB database.
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = "";
}

// D2 ViewModels; these are not Entity Framework database tables.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = "";
    public string ImageAlt { get; set; } = "";
}

public class DemoOrder
{
    public string Number { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string Details { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pending";
}

public class CustomerDashboardViewModel
{
    public string FullName { get; set; } = "";
    public List<DemoOrder> SampleOrders { get; set; } = new();
    public int TotalOrders => SampleOrders.Count;
    public int PendingOrders => SampleOrders.Count(o => o.Status == "Pending");
    public int CompletedOrders => SampleOrders.Count(o => o.Status == "Completed");
}

public class AdminDashboardViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<DemoOrder> SampleOrders { get; set; } = new();
    public int PendingOrders => SampleOrders.Count(o => o.Status == "Pending");
    public int CompletedOrders => SampleOrders.Count(o => o.Status == "Completed");
    public int LowStockProducts => Products.Count(p => p.Stock <= 5);
}

public class CatalogViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string? Search { get; set; }
    public string? SelectedCategory { get; set; }
}
