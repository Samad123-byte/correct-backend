using Backend.DTOs.Requests;
using Backend.DTOs.Responses;

namespace Backend.IServices
{
    public interface ISalespersonService
    {
        Task<PaginatedResponse<SalespersonResponse>> GetAllSalespersonsAsync(PaginationRequest request);
        Task<ApiResponse<SalespersonResponse>> GetSalespersonByIdAsync(int id);
        Task<ApiResponse<SalespersonResponse>> CreateSalespersonAsync(CreateSalespersonRequest request);
        Task<ApiResponse<SalespersonResponse>> UpdateSalespersonAsync(UpdateSalespersonRequest request);
        Task<ApiResponse<object>> DeleteSalespersonAsync(DeleteRequest request);
    }
}