namespace Project.Models;

public class CatalogViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string? Search { get; set; }
    public string? SelectedCategory { get; set; }
}