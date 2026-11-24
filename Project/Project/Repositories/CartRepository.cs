using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Project.Models;

namespace Project.Repositories;

public class CartRepository : ICartRepository
{
    private readonly string _connectionString;

    public CartRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string missing.");
    }

    public async Task<List<CartItemViewModel>> GetAsync(string userId)
    {
        string query =
            "SELECT c.Id, c.ProductId, c.Quantity, " +
            "p.Name, p.Price, p.Stock, p.IsActive " +
            "FROM CartItems c " +
            "INNER JOIN Products p ON p.Id = c.ProductId " +
            "WHERE c.UserId = @UserId ORDER BY c.Id";

        using var connection = new SqlConnection(_connectionString);

        var items = await connection.QueryAsync<CartItemViewModel>(
            query, new { UserId = userId });

        return items.ToList();
    }

    public async Task<bool> SetQuantityAsync(
        string userId, int productId, int quantity, bool add)
    {
        if (quantity < 1 || quantity > 10000)
            return false;

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction =
            connection.BeginTransaction(IsolationLevel.Serializable);

        var product = await connection.QuerySingleOrDefaultAsync<Product>(
            "SELECT * FROM Products WITH (UPDLOCK, HOLDLOCK) WHERE Id = @Id",
            new { Id = productId }, transaction);

        if (product == null || !product.IsActive)
            return false;

        var current = await connection.QuerySingleOrDefaultAsync<int?>(
            "SELECT Quantity FROM CartItems " +
            "WHERE UserId = @UserId AND ProductId = @ProductId",
            new { UserId = userId, ProductId = productId },
            transaction);

        long requested = add
            ? (long)(current ?? 0) + quantity
            : quantity;

        if (requested > product.Stock || requested > 10000)
            return false;

        string query = current == null
            ? "INSERT INTO CartItems (UserId, ProductId, Quantity) " +
              "VALUES (@UserId, @ProductId, @Quantity)"
            : "UPDATE CartItems SET Quantity = @Quantity " +
              "WHERE UserId = @UserId AND ProductId = @ProductId";

        await connection.ExecuteAsync(query, new
        {
            UserId = userId,
            ProductId = productId,
            Quantity = (int)requested
        }, transaction);

        transaction.Commit();
        return true;
    }

    public async Task RemoveAsync(string userId, int productId)
    {
        string query =
            "DELETE FROM CartItems " +
            "WHERE UserId = @UserId AND ProductId = @ProductId";

        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(query, new
        {
            UserId = userId,
            ProductId = productId
        });
    }
}