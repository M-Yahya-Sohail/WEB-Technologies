using Dapper;
using Microsoft.Data.SqlClient;
using Project.Models;

namespace Project.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string missing.");
    }

    public async Task<List<Product>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);

        var products = await connection.QueryAsync<Product>(
            "SELECT * FROM Products ORDER BY Id DESC");

        return products.ToList();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Product>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id });
    }

    public async Task AddAsync(Product product)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync("""
            INSERT INTO Products
                (Name, Category, Description, Price, Stock,
                 ImageUrl, ImageAlt, IsActive)
            VALUES
                (@Name, @Category, @Description, @Price, @Stock,
                 @ImageUrl, @ImageAlt, @IsActive)
            """, product);
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var connection = new SqlConnection(_connectionString);

        var affected = await connection.ExecuteAsync("""
            UPDATE Products
            SET Name = @Name,
                Category = @Category,
                Description = @Description,
                Price = @Price,
                Stock = @Stock,
                ImageUrl = @ImageUrl,
                ImageAlt = @ImageAlt,
                IsActive = @IsActive
            WHERE Id = @Id
            """, product);

        return affected == 1;
    }

    public async Task ArchiveAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "UPDATE Products SET IsActive = 0 WHERE Id = @Id",
            new { Id = id });
    }
    public async Task<List<Product>> SearchAsync(string? search, string? category)
    {
        string query =
            "SELECT * FROM Products " +
            "WHERE IsActive = 1 " +
            "AND (@Search IS NULL " +
            "OR CHARINDEX(@Search, Name) > 0 " +
            "OR CHARINDEX(@Search, Description) > 0) " +
            "AND (@Category IS NULL OR Category = @Category) " +
            "ORDER BY Id DESC";

        using (SqlConnection connection =
            new SqlConnection(_connectionString))
        {
            var products = await connection.QueryAsync<Product>(
                query,
                new
                {
                    Search = string.IsNullOrWhiteSpace(search)
                        ? null : search.Trim(),

                    Category = string.IsNullOrWhiteSpace(category)
                        ? null : category.Trim()
                });

            return products.ToList();
        }
    }
}