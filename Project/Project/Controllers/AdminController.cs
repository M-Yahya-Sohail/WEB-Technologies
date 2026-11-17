using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Services;

namespace Project.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly DemoStore _store;
    public AdminController(DemoStore store) => _store = store;

    // D2: dynamic Razor dashboard using demo ViewModel, no product/order CRUD yet.
    public IActionResult Index() => View(_store.GetAdmin());
}
