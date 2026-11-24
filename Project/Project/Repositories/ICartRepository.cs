using Project.Models;

namespace Project.Repositories;

public interface ICartRepository
{
    Task<List<CartItemViewModel>> GetAsync(string userId);

    Task<bool> SetQuantityAsync(
        string userId, int productId, int quantity, bool add);

    Task RemoveAsync(string userId, int productId);
}