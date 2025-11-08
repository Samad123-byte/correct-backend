using Backend.DTOs.Responses;
using Backend.IRepository;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace Backend.Repositories
{
    public class SaleRepository : ISaleRepository
    {

        private readonly string _connectionString;

        public SaleRepository(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<PaginatedResponse<Sale>> GetAllSalesAsync(int startIndex, int endIndex)
        {
            var response = new PaginatedResponse<Sale> { Data = new List<Sale>() };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllSales", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartIndex", startIndex);
            command.Parameters.AddWithValue("@EndIndex", endIndex);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (response.Data.Count == 0)
                {
                    response.TotalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
                    response.StartIndex = startIndex;
                    response.EndIndex = endIndex;
                }

                response.Data.Add(new Sale
                {
                    SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                    SalespersonName = reader.IsDBNull(reader.GetOrdinal("SalespersonName")) ? null : reader.GetString(reader.GetOrdinal("SalespersonName")),
                    SalespersonId = reader.IsDBNull(reader.GetOrdinal("SalespersonId")) ? null : reader.GetInt32(reader.GetOrdinal("SalespersonId")),
                    Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                    SaleDetails = new List<Sale.SaleDetailDto>()
                });
            }

            return response;
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSaleById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Sale
                {
                    SaleId = reader.GetInt32("SaleId"),
                    Total = reader.GetDecimal("Total"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                    Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                    SaleDetails = new List<Sale.SaleDetailDto>()
                };
            }
            return null;
        }

        public async Task<int> DeleteSaleAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteSale", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateSaleJson", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@SalespersonId", (object?)sale.SalespersonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);
            command.Parameters.Add("@SaleDate", SqlDbType.DateTime).Value =
                (object?)sale.SaleDate?.ToUniversalTime() ?? DBNull.Value;

            foreach (var detail in sale.SaleDetails ?? new List<Sale.SaleDetailDto>())
                detail.RowState = "Added";

            var json = JsonConvert.SerializeObject(sale.SaleDetails ?? new List<Sale.SaleDetailDto>());
            command.Parameters.AddWithValue("@SaleDetailsJson", json);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Failed to create sale.");

            var createdSale = new Sale
            {
                SaleId = reader.GetInt32("SaleId"),
                Total = reader.GetDecimal("Total"),
                SaleDate = reader.GetDateTime("SaleDate"),
                SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                SaleDetails = new List<Sale.SaleDetailDto>()
            };

            if (await reader.NextResultAsync())
            {
                var details = new List<Sale.SaleDetailDto>();
                while (await reader.ReadAsync())
                {
                    details.Add(new Sale.SaleDetailDto
                    {
                        SaleDetailId = reader.GetInt32("SaleDetailId"),
                        ProductId = reader.GetInt32("ProductId"),
                        RetailPrice = reader.GetDecimal("RetailPrice"),
                        Quantity = reader.GetInt32("Quantity"),
                        Discount = reader.GetDecimal("Discount")
                    });
                }
                createdSale.SaleDetails = details;
            }

            return createdSale;
        }

        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand("sp_UpdateSaleJson", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SaleId", sale.SaleId);
            cmd.Parameters.AddWithValue("@SalespersonId", (object?)sale.SalespersonId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);
            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime).Value =
                (object?)sale.SaleDate?.ToUniversalTime() ?? DBNull.Value;

            var json = JsonConvert.SerializeObject(sale.SaleDetails ?? new List<Sale.SaleDetailDto>());
            cmd.Parameters.AddWithValue("@SaleDetailsJson", json);

            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        // ✅ FIXED: GetSaleWithDetailsAsync in SaleRepository.cs
        public async Task<Sale?> GetSaleWithDetailsAsync(int id)
        {
            var sale = await GetSaleByIdAsync(id);
            if (sale == null) return null;

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSaleDetailsBySaleId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var details = new List<Sale.SaleDetailDto>();
            while (await reader.ReadAsync())
            {
                details.Add(new Sale.SaleDetailDto
                {
                    SaleDetailId = reader.GetInt32("SaleDetailId"), // ✅ INCLUDE SaleDetailId
                    ProductId = reader.GetInt32("ProductId"),
                    RetailPrice = reader.GetDecimal("RetailPrice"),
                    Quantity = reader.GetInt32("Quantity"),
                    Discount = reader.GetDecimal("Discount"),
                    RowState = "Unchanged" // ✅ Default state for existing items
                });
            }

            sale.SaleDetails = details;
            return sale;
        }
    }
}