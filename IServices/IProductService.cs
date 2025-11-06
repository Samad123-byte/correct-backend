using Backend.DTOs.Requests;
using Backend.DTOs.Responses;

namespace Backend.IServices
{
    public interface IProductService
    {
        Task<PaginatedResponse<ProductResponse>> GetAllProductsAsync(PaginationRequest request);
        Task<ApiResponse<ProductResponse>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductResponse>> CreateProductAsync(CreateProductRequest request);
        Task<ApiResponse<ProductResponse>> UpdateProductAsync(UpdateProductRequest request);
        Task<ApiResponse<object>> DeleteProductAsync(DeleteRequest request);
    }
}