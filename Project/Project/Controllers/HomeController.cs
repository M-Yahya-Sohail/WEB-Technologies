using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;

namespace Project.Controllers;

public class HomeController : Controller
{
    private readonly IOrderRepository _orders;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IProductRepository _products;

    public HomeController( UserManager<ApplicationUser> users, IProductRepository products, IOrderRepository orders)
    {
        _users = users;
        _products = products;
        _orders = orders;
    }

    public async Task<IActionResult> Index(
    string? search, string? category)
    {
        var products = await _products.SearchAsync(search, category);

        var allProducts = await _products.SearchAsync(null, null);

        var model = new CatalogViewModel
        {
            Products = products,

            Categories = allProducts
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList(),

            Search = search,
            SelectedCategory = category
        };

        return View(model);
    }

    public IActionResult Privacy() => View();
    public IActionResult Login() => RedirectToPage("/Account/Login", new { area = "Identity" });
    public IActionResult Signup() => RedirectToPage("/Account/Register", new { area = "Identity" });

    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Customer()
    {
        var user = await _users.GetUserAsync(User);

        if (user == null)
            return Challenge();

        return View(new CustomerDashboardViewModel
        {
            FullName = user.FullName,
            Orders = await _orders.GetOrdersAsync(user.Id)
        });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin() => RedirectToAction("Index", "Admin");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });

    [HttpGet]
    public async Task<IActionResult> Products(
    string? search, string? category)
    {
        var products = await _products.SearchAsync(search, category);

        return PartialView("_ProductList", products);
    }
}
