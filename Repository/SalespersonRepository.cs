using Backend.DTOs.Responses;
using Backend.IRepository;
using Backend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Backend.Repository
{
    public class SalespersonRepository : ISalespersonRepository
    {
        private readonly string _connectionString;

        public SalespersonRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<PaginatedResponse<Salesperson>> GetAllSalespersonsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = new PaginatedResponse<Salesperson> { Data = new List<Salesperson>() };
            int totalRecords = 0;

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllSalespersons", connection)
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

                response.Data.Add(new Salesperson
                {
                    SalespersonId = reader.GetInt32("SalespersonId"),
                    Name = reader.GetString("Name"),
                    Code = reader.GetString("Code"),
                    EnteredDate = reader.IsDBNull("EnteredDate") ? null : reader.GetDateTime("EnteredDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                });
            }

            response.CurrentPage = pageNumber;
            response.PageSize = pageSize;
            response.TotalRecords = totalRecords;
            response.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return response;
        }

        public async Task<Salesperson?> GetSalespersonByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSalespersonById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SalespersonId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Salesperson
                {
                    SalespersonId = reader.GetInt32("SalespersonId"),
                    Name = reader.GetString("Name"),
                    Code = reader.GetString("Code"),
                    EnteredDate = reader.IsDBNull("EnteredDate") ? null : reader.GetDateTime("EnteredDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                };
            }

            return null;
        }

        public async Task<int> CreateSalespersonAsync(Salesperson salesperson)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_InsertSalesperson", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Name", salesperson.Name);
            command.Parameters.AddWithValue("@Code", salesperson.Code);
            command.Parameters.AddWithValue("@EnteredDate", (object?)salesperson.EnteredDate ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<int> UpdateSalespersonAsync(Salesperson salesperson)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpdateSalesperson", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@SalespersonId", salesperson.SalespersonId);
            command.Parameters.AddWithValue("@Name", salesperson.Name);
            command.Parameters.AddWithValue("@Code", salesperson.Code);
            command.Parameters.AddWithValue("@EnteredDate", (object?)salesperson.EnteredDate ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<int> DeleteSalespersonAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteSalesperson", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SalespersonId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return reader.GetInt32("Result");

            return 0;
        }
    }
}