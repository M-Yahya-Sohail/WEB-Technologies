using Project.Models;

namespace Project.Services;

// D2 presentation data: controller + models + Razor loops, no product/order database.
// No order is placed or persisted in Deliverable 2.
public class DemoStore
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Spiral Notebook", Category = "Notebooks",
            Description = "Keep lecture notes, daily plans and ideas together in one handy notebook.",
            Price = 350, Stock = 4,
            ImageUrl = "https://images.pexels.com/photos/6651598/pexels-photo-6651598.jpeg?auto=compress&cs=tinysrgb&w=800",
            ImageAlt = "Open notebook with a pen on a marble desk" },
        new Product { Id = 2, Name = "Ballpoint Pen Set", Category = "Writing essentials",
            Description = "A set of three ballpoint pens for everyday writing and classroom notes.",
            Price = 240, Stock = 3,
            ImageUrl = "https://images.pexels.com/photos/983826/pexels-photo-983826.jpeg?auto=compress&cs=tinysrgb&w=800",
            ImageAlt = "Three colourful ballpoint pens" },
        new Product { Id = 3, Name = "Colourful Sticky Notes", Category = "Desk essentials",
            Description = "Mark pages, jot down reminders and organise your study tasks with ease.",
            Price = 180, Stock = 35,
            ImageUrl = "https://images.pexels.com/photos/6991354/pexels-photo-6991354.jpeg?auto=compress&cs=tinysrgb&w=800",
            ImageAlt = "Colourful sticky notes" },
        new Product { Id = 4, Name = "English-Spanish Dictionary", Category = "Books & academics",
            Description = "A handy reference for looking up words and building bilingual vocabulary.",
            Price = 850, Stock = 15,
            ImageUrl = "https://images.pexels.com/photos/13752245/pexels-photo-13752245.jpeg?auto=compress&cs=tinysrgb&w=800",
            ImageAlt = "Open dictionary showing word entries" },
    };

    private readonly List<DemoOrder> _orders = new()
    {
        new DemoOrder { Number = "SH-1003", CustomerName = "Alex Ahmed", Details = "Notebook & gel pens", Date = new DateTime(2026, 11, 8), Total = 590, Status = "Pending" },
        new DemoOrder { Number = "SH-1004", CustomerName = "Sara Ali", Details = "Stationery set", Date = new DateTime(2026, 11, 8), Total = 350, Status = "Pending" },
        new DemoOrder { Number = "SH-1005", CustomerName = "Hamza Khan", Details = "Academic books", Date = new DateTime(2026, 11, 8), Total = 1030, Status = "Pending" },
        new DemoOrder { Number = "SH-1002", CustomerName = "Alex Ahmed", Details = "Study reference book", Date = new DateTime(2026, 11, 5), Total = 850, Status = "Completed" },
        new DemoOrder { Number = "SH-1001", CustomerName = "Alex Ahmed", Details = "Pastel sticky notes", Date = new DateTime(2026, 11, 2), Total = 180, Status = "Completed" },
    };

    public CatalogViewModel GetCatalog(string? search, string? category)
    {
        IEnumerable<Product> query = _products;
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return new CatalogViewModel
        {
            Products = query.ToList(),
            Categories = _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList(),
            Search = search,
            SelectedCategory = category
        };
    }

    public CustomerDashboardViewModel GetCustomer(string fullName) => new()
    {
        FullName = fullName,
        SampleOrders = _orders.Where(o => o.CustomerName == "Alex Ahmed").ToList()
    };

    public AdminDashboardViewModel GetAdmin() => new()
    {
        Products = _products.ToList(),
        SampleOrders = _orders.ToList()
    };
}
