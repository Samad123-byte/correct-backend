using Backend.DTOs.Responses;
using Backend.Models;

namespace Backend.IRepository
{
    public interface IProductRepository
    {
        Task<PaginatedResponse<Product>> GetAllProductsAsync(int startIndex, int endIndex);
        Task<Product?> GetProductByIdAsync(int id);
        Task<int> CreateProductAsync(Product product);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(int id);
    }
}