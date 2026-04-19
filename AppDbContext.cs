using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Ekz.Models;

namespace Ekz.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService()
    {
        _connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234";
    }

    // ==================== Products ====================
    
    public async Task<List<Product>> GetProducts()
    {
        var products = new List<Product>();
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "SELECT id, Name, Price, Quantity FROM Products ORDER BY id";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Quantity = reader.GetInt32(3)
            });
        }
        return products;
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "UPDATE Products SET Name = @name, Price = @price, Quantity = @quantity WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@quantity", product.Quantity);
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<int> AddProduct(Product product)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "INSERT INTO Products (Name, Price, Quantity) VALUES (@name, @price, @quantity) RETURNING id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@quantity", product.Quantity);
        var newId = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(newId);
    }

    public async Task<bool> DeleteProduct(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "DELETE FROM Products WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    // ==================== Zakaz ====================
    
    public async Task<List<Zakaz>> GetZakaz()
    {
        var zakazList = new List<Zakaz>();
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = @"
            SELECT z.id_заказа, z.KlId, z.Summa, z.PrId, p.Name as ProductName 
            FROM Zakaz z
            LEFT JOIN Products p ON z.PrId = p.id
            ORDER BY z.id_заказа";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            zakazList.Add(new Zakaz
            {
                IdZakaza = reader.GetInt32(0),
                KlId = reader.GetInt32(1),
                Summa = reader.GetInt32(2),
                PrId = reader.GetInt32(3),
                ProductName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            });
        }
        return zakazList;
    }

    public async Task<bool> UpdateZakaz(Zakaz zakaz)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "UPDATE Zakaz SET KlId = @klId, Summa = @summa, PrId = @prId WHERE id_заказа = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", zakaz.IdZakaza);
        cmd.Parameters.AddWithValue("@klId", zakaz.KlId);
        cmd.Parameters.AddWithValue("@summa", zakaz.Summa);
        cmd.Parameters.AddWithValue("@prId", zakaz.PrId);
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<int> AddZakaz(Zakaz zakaz)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "INSERT INTO Zakaz (KlId, Summa, PrId) VALUES (@klId, @summa, @prId) RETURNING id_заказа";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@klId", zakaz.KlId);
        cmd.Parameters.AddWithValue("@summa", zakaz.Summa);
        cmd.Parameters.AddWithValue("@prId", zakaz.PrId);
        var newId = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(newId);
    }

    public async Task<bool> DeleteZakaz(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "DELETE FROM Zakaz WHERE id_заказа = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    // Получение списка продуктов для выпадающего списка (PrId)
    public async Task<List<Product>> GetProductsForCombo()
    {
        var products = new List<Product>();
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        string sql = "SELECT id, Name FROM Products ORDER BY Name";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }
        return products;
    }
}