using Backend.DTOs.Responses;
using Backend.Models;

namespace Backend.IRepository
{
    public interface ISaleRepository
    {
        Task<PaginatedResponse<Sale>> GetAllSalesAsync(int startIndex, int endIndex);
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<Sale> CreateSaleAsync(Sale sale);
        Task<bool> UpdateSaleAsync(Sale sale);
        Task<int> DeleteSaleAsync(int id);
        Task<Sale?> GetSaleWithDetailsAsync(int id);
    }
}