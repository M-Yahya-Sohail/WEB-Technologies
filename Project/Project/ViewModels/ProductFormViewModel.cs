using System.ComponentModel.DataAnnotations;

namespace Project.Models;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(60)]
    public string Category { get; set; } = "";

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    [StringLength(200)]
    public string? ImageAlt { get; set; }

    public bool IsActive { get; set; } = true;
}