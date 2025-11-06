using Backend.DTOs.Responses;
using Backend.IRepository;
using Backend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Backend.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<PaginatedResponse<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = new PaginatedResponse<Product> { Data = new List<Product>() };
            int totalRecords = 0;

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllProducts", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (totalRecords == 0)
                    totalRecords = reader.GetInt32("TotalRecords");

                response.Data.Add(new Product
                {
                    ProductId = reader.GetInt32("ProductId"),
                    Name = reader.GetString("Name"),
                    Code = reader.IsDBNull("Code") ? null : reader.GetString("Code"),
                    ImageURL = reader.IsDBNull("ImageURL") ? null : reader.GetString("ImageURL"),
                    CostPrice = reader.IsDBNull("CostPrice") ? null : reader.GetDecimal("CostPrice"),
                    RetailPrice = reader.IsDBNull("RetailPrice") ? null : reader.GetDecimal("RetailPrice"),
                    CreationDate = reader.IsDBNull("CreationDate") ? null : reader.GetDateTime("CreationDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                });
            }

            response.CurrentPage = pageNumber;
            response.PageSize = pageSize;
            response.TotalRecords = totalRecords;
            response.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return response;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetProductById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@ProductId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Product
                {
                    ProductId = reader.GetInt32("ProductId"),
                    Name = reader.GetString("Name"),
                    Code = reader.IsDBNull("Code") ? null : reader.GetString("Code"),
                    ImageURL = reader.IsDBNull("ImageURL") ? null : reader.GetString("ImageURL"),
                    CostPrice = reader.IsDBNull("CostPrice") ? null : reader.GetDecimal("CostPrice"),
                    RetailPrice = reader.IsDBNull("RetailPrice") ? null : reader.GetDecimal("RetailPrice"),
                    CreationDate = reader.IsDBNull("CreationDate") ? null : reader.GetDateTime("CreationDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                };
            }

            return null;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateProduct", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Code", (object?)product.Code ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImageURL", (object?)product.ImageURL ?? DBNull.Value);
            command.Parameters.AddWithValue("@CostPrice", (object?)product.CostPrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@RetailPrice", (object?)product.RetailPrice ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpdateProduct", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ProductId", product.ProductId);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Code", (object?)product.Code ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImageURL", (object?)product.ImageURL ?? DBNull.Value);
            command.Parameters.AddWithValue("@CostPrice", (object?)product.CostPrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@RetailPrice", (object?)product.RetailPrice ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteProduct", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@ProductId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return reader.GetInt32("Result");

            return 0;
        }
    }
}