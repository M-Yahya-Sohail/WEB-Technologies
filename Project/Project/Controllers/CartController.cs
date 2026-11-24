using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;

namespace Project.Controllers;

[Authorize(Roles = "Customer,Admin")]
public class CartController : Controller
{
    private readonly ICartRepository _cart;
    private readonly IOrderRepository _orders;
    private readonly UserManager<ApplicationUser> _users;

    public CartController( ICartRepository cart, UserManager<ApplicationUser> users, IOrderRepository orders)
    {
        _cart = cart;
        _users = users;
        _orders = orders;
    }

    public async Task<IActionResult> Index()
    {
        string userId = _users.GetUserId(User)!;
        return View(await _cart.GetAsync(userId));
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        bool saved = await _cart.SetQuantityAsync(
            _users.GetUserId(User)!, productId, quantity, true);

        TempData["Message"] = saved
            ? "Product added to cart."
            : "Requested quantity is not available.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        bool saved = await _cart.SetQuantityAsync(
            _users.GetUserId(User)!, productId, quantity, false);

        TempData["Message"] = saved
            ? "Quantity updated."
            : "Invalid quantity or insufficient stock.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        await _cart.RemoveAsync(_users.GetUserId(User)!, productId);

        TempData["Message"] = "Product removed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        int? orderId;

        try
        {
            orderId = await _orders.CheckoutAsync(_users.GetUserId(User)!);
        }
        catch (SqlException ex) when (ex.Number == 1205)
        {
            TempData["Message"] =
                "The shop is busy. Please review your cart and try again.";

            return RedirectToAction(nameof(Index));
        }

        if (orderId == null)
        {
            TempData["Message"] =
                "Your cart is empty or stock has changed. Review your items.";

            return RedirectToAction(nameof(Index));
        }

        TempData["Message"] =
            $"Order SH-{orderId} placed. Pay cash when collecting at the shop.";

        return RedirectToAction("Customer", "Home");
    }
}