using Project.Models;

namespace Project.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task ArchiveAsync(int id);
    Task<List<Product>> SearchAsync(string? search, string? category);
}