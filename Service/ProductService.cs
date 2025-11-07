using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IRepository;
using Backend.IServices;
using Backend.Models;

namespace Backend.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PaginatedResponse<ProductResponse>> GetAllProductsAsync(PaginationRequest request)
        {
            var products = await _productRepository.GetAllProductsAsync(request.PageNumber, request.PageSize);

            return new PaginatedResponse<ProductResponse>
            {
                Success = true,
                Message = "Products fetched successfully.",
                Data = products.Data.Select(MapToResponse).ToList(),
                CurrentPage = products.CurrentPage,
                PageSize = products.PageSize,
                TotalRecords = products.TotalRecords,
                TotalPages = products.TotalPages
            };
        }

        public async Task<ApiResponse<ProductResponse>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found.");

            return new ApiResponse<ProductResponse>
            {
                Success = true,
                Message = "Product fetched successfully.",
                Data = MapToResponse(product)
            };
        }

        public async Task<ApiResponse<ProductResponse>> CreateProductAsync(CreateProductRequest request)
        {
            var allProducts = await _productRepository.GetAllProductsAsync(1, int.MaxValue);
            var duplicateExists = allProducts.Data.Any(p =>
                p.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(request.Code) && p.Code == request.Code));

            if (duplicateExists)
                throw new Exception("Duplicate product Name or Code.");

            var product = new Product
            {
                Name = request.Name,
                Code = request.Code,
                ImageURL = request.ImageURL,
                CostPrice = request.CostPrice,
                RetailPrice = request.RetailPrice
            };

            var result = await _productRepository.CreateProductAsync(product);
            if (result <= 0)
                throw new Exception("Failed to create product.");

            product.ProductId = result;

            return new ApiResponse<ProductResponse>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = MapToResponse(product)
            };
        }

        public async Task<ApiResponse<ProductResponse>> UpdateProductAsync(UpdateProductRequest request)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (existingProduct == null)
                throw new Exception("Product not found.");

            var product = new Product
            {
                ProductId = request.ProductId,
                Name = request.Name,
                Code = request.Code,
                ImageURL = request.ImageURL,
                CostPrice = request.CostPrice,
                RetailPrice = request.RetailPrice
            };

            var result = await _productRepository.UpdateProductAsync(product);

            if (result == -1)
                throw new Exception("Duplicate product Name or Code.");
            if (result == 0)
                throw new Exception("Product not found.");

            var updatedProduct = await _productRepository.GetProductByIdAsync(request.ProductId);

            return new ApiResponse<ProductResponse>
            {
                Success = true,
                Message = "Product updated successfully.",
                Data = MapToResponse(updatedProduct!)
            };
        }

        public async Task<ApiResponse<object>> DeleteProductAsync(DeleteRequest request)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(request.Id);
            if (existingProduct == null)
                throw new Exception("Product not found.");

            var result = await _productRepository.DeleteProductAsync(request.Id);

            if (result == 1)
                return new ApiResponse<object> { Success = true, Message = "Product deleted successfully." };

            if (result == -1)
                throw new Exception("Cannot delete product — it may be used in sales records.");

            throw new Exception("Failed to delete product.");
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Code = product.Code,
                ImageURL = product.ImageURL,
                CostPrice = product.CostPrice,
                RetailPrice = product.RetailPrice,
                CreationDate = product.CreationDate,
                UpdatedDate = product.UpdatedDate
            };
        }
    }
}
