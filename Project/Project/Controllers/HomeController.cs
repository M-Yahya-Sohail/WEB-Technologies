using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;

namespace Project.Controllers;

public class HomeController : Controller
{
    private readonly DemoStore _store;
    private readonly UserManager<ApplicationUser> _users;

    public HomeController(DemoStore store, UserManager<ApplicationUser> users)
    {
        _store = store;
        _users = users;
    }

    public IActionResult Index(string? search, string? category)
        => View(_store.GetCatalog(search, category));

    public IActionResult Privacy() => View();

    // Keep the D1-style routes functional, but use real Identity pages for accounts.
    public IActionResult Login() => RedirectToPage("/Account/Login", new { area = "Identity" });
    public IActionResult Signup() => RedirectToPage("/Account/Register", new { area = "Identity" });

    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Customer()
    {
        var user = await _users.GetUserAsync(User);
        var name = user?.FullName ?? User.Identity?.Name ?? "Customer";
        return View(_store.GetCustomer(name));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin() => RedirectToAction("Index", "Admin");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
