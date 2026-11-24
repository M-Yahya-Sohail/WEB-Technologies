using Project.Models;

namespace Project.Repositories;

public interface IOrderRepository
{
    Task<int?> CheckoutAsync(string userId);
    Task<List<OrderViewModel>> GetOrdersAsync(string? userId);
    Task CompleteAsync(int id);
}