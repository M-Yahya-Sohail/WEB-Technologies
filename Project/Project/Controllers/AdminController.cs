using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;

namespace Project.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;

    public AdminController( IProductRepository products, IOrderRepository orders)
    {
        _products = products;
        _orders = orders;
    }

    public async Task<IActionResult> Index()
    {
        return View(new AdminDashboardViewModel
        {
            Products = await _products.GetAllAsync(),
            Orders = await _orders.GetOrdersAsync(null)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        await _orders.CompleteAsync(id);

        TempData["Message"] = "Order marked Completed.";
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Create()
    {
        return View("Edit", new ProductFormViewModel());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _products.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return View(new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            ImageAlt = product.ImageAlt,
            IsActive = product.IsActive
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save(ProductFormViewModel input)
    {
        if (!ModelState.IsValid)
            return View("Edit", input);

        var product = new Product
        {
            Id = input.Id,
            Name = input.Name.Trim(),
            Category = input.Category.Trim(),
            Description = input.Description?.Trim() ?? "",
            Price = input.Price,
            Stock = input.Stock,
            ImageUrl = input.ImageUrl?.Trim() ?? "",
            ImageAlt = input.ImageAlt?.Trim() ?? "",
            IsActive = input.IsActive
        };

        if (product.Id == 0)
        {
            await _products.AddAsync(product);
        }
        else
        {
            var updated = await _products.UpdateAsync(product);

            if (!updated)
                return NotFound();
        }

        TempData["Message"] = "Product saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Archive(int id)
    {
        await _products.ArchiveAsync(id);

        TempData["Message"] = "Product archived.";
        return RedirectToAction(nameof(Index));
    }
}