using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Project.Models;

namespace Project.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string missing.");
    }

    public async Task<int?> CheckoutAsync(string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction =
            connection.BeginTransaction(IsolationLevel.Serializable);

        string cartQuery =
            "SELECT c.Id, c.ProductId, c.Quantity, " +
            "p.Name, p.Price, p.Stock, p.IsActive " +
            "FROM CartItems c WITH (UPDLOCK, HOLDLOCK) " +
            "INNER JOIN Products p WITH (UPDLOCK, HOLDLOCK) " +
            "ON p.Id = c.ProductId " +
            "WHERE c.UserId = @UserId ORDER BY p.Id";

        var items = (await connection.QueryAsync<CartItemViewModel>(
            cartQuery, new { UserId = userId }, transaction)).ToList();

        if (items.Count == 0 ||
            items.Any(i => !i.IsActive ||
                           i.Quantity < 1 ||
                           i.Quantity > i.Stock))
        {
            return null;
        }

        decimal total = items.Sum(i => i.Price * i.Quantity);

        string orderQuery =
            "INSERT INTO Orders (UserId, PlacedAt, Status, Total) " +
            "OUTPUT INSERTED.Id " +
            "VALUES (@UserId, @PlacedAt, @Status, @Total)";

        int orderId = await connection.QuerySingleAsync<int>(
            orderQuery,
            new
            {
                UserId = userId,
                PlacedAt = DateTime.UtcNow,
                Status = "Pending",
                Total = total
            },
            transaction);

        foreach (var item in items)
        {
            string stockQuery =
                "UPDATE Products SET Stock = Stock - @Quantity " +
                "WHERE Id = @ProductId AND IsActive = 1 " +
                "AND Stock >= @Quantity";

            int updated = await connection.ExecuteAsync(
                stockQuery,
                new { item.ProductId, item.Quantity },
                transaction);

            if (updated != 1)
                return null;

            string itemQuery =
                "INSERT INTO OrderItems " +
                "(OrderId, ProductId, ProductName, UnitPrice, Quantity) " +
                "VALUES " +
                "(@OrderId, @ProductId, @ProductName, @UnitPrice, @Quantity)";

            await connection.ExecuteAsync(
                itemQuery,
                new
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    ProductName = item.Name,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity
                },
                transaction);
        }

        await connection.ExecuteAsync(
            "DELETE FROM CartItems WHERE UserId = @UserId",
            new { UserId = userId },
            transaction);

        transaction.Commit();
        return orderId;
    }

    public async Task<List<OrderViewModel>> GetOrdersAsync(string? userId)
    {
        string query =
            "SELECT o.Id, o.PlacedAt, o.Status, o.Total, " +
            "u.FullName AS CustomerName " +
            "FROM Orders o INNER JOIN AspNetUsers u ON u.Id = o.UserId " +
            "WHERE (@UserId IS NULL OR o.UserId = @UserId) " +
            "ORDER BY o.Id DESC; " +

            "SELECT i.* FROM OrderItems i " +
            "INNER JOIN Orders o ON o.Id = i.OrderId " +
            "WHERE (@UserId IS NULL OR o.UserId = @UserId);";

        using var connection = new SqlConnection(_connectionString);

        using var results = await connection.QueryMultipleAsync(
            query, new { UserId = userId });

        var orders = (await results.ReadAsync<OrderViewModel>()).ToList();
        var items = (await results.ReadAsync<OrderItem>()).ToLookup(i => i.OrderId);

        foreach (var order in orders)
            order.Items = items[order.Id].ToList();

        return orders;
    }

    public async Task CompleteAsync(int id)
    {
        string query =
            "UPDATE Orders SET Status = 'Completed' " +
            "WHERE Id = @Id AND Status = 'Pending'";

        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(query, new { Id = id });
    }
}