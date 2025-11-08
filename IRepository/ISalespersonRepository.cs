using Backend.DTOs.Responses;
using Backend.Models;

namespace Backend.IRepository
{
    public interface ISalespersonRepository
    {
        Task<PaginatedResponse<Salesperson>> GetAllSalespersonsAsync(int startIndex, int endIndex);
        Task<Salesperson?> GetSalespersonByIdAsync(int id);
        Task<int> CreateSalespersonAsync(Salesperson salesperson);
        Task<int> UpdateSalespersonAsync(Salesperson salesperson);
        Task<int> DeleteSalespersonAsync(int id);
    }
}