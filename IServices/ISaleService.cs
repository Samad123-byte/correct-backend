using Backend.DTOs.Requests;
using Backend.DTOs.Responses;

namespace Backend.IServices
{
    public interface ISaleService
    {
        Task<PaginatedResponse<SaleResponse>> GetAllSalesAsync(PaginationRequest request);
        Task<ApiResponse<SaleResponse>> GetSaleByIdAsync(int id);
        Task<ApiResponse<SaleResponse>> CreateSaleAsync(CreateSaleRequest request);
        Task<ApiResponse<SaleResponse>> UpdateSaleAsync(UpdateSaleRequest request);
        Task<ApiResponse<object>> DeleteSaleAsync(DeleteRequest request);
        Task<ApiResponse<SaleResponse>> GetSaleWithDetailsAsync(int id);
    }
}