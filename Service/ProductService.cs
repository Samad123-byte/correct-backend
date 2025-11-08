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
          
            var products = await _productRepository.GetAllProductsAsync(request.StartIndex, request.EndIndex);

            return new PaginatedResponse<ProductResponse>
            {
                Data = products.Data.Select(MapToResponse).ToList(),
                StartIndex = products.StartIndex,
                EndIndex = products.EndIndex,
                TotalRecords = products.TotalRecords
            };
        }

        public async Task<ApiResponse<ProductResponse>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found.");

            return new ApiResponse<ProductResponse>
            {
                Data = MapToResponse(product)
            };
        }

        public async Task<ApiResponse<ProductResponse>> CreateProductAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Code = request.Code,
                ImageURL = request.ImageURL,
                CostPrice = request.CostPrice,
                RetailPrice = request.RetailPrice
            };

            var result = await _productRepository.CreateProductAsync(product);

            if (result == -1)
                throw new Exception("Duplicate product Name or Code already exists.");

            if (result <= 0)
                throw new Exception("Failed to create product.");

            // Get the newly created product with all details
            var createdProduct = await _productRepository.GetProductByIdAsync(result);
            if (createdProduct == null)
                throw new Exception("Product created but could not retrieve details.");

            return new ApiResponse<ProductResponse>
            {
                Success = true,
                Data = MapToResponse(createdProduct)
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
                throw new Exception("Duplicate product Code already exists.");

            if (result == 0)
                throw new Exception("Product not found.");

            var updatedProduct = await _productRepository.GetProductByIdAsync(request.ProductId);

            return new ApiResponse<ProductResponse>
            {
                Success = true,
                Data = MapToResponse(updatedProduct!)
            };
        }

        public async Task<ApiResponse<object>> DeleteProductAsync(DeleteRequest request)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(request.Id);
            if (existingProduct == null)
                throw new Exception("Product not found.");

            var result = await _productRepository.DeleteProductAsync(request.Id);

            if (result == -1)
                throw new Exception("Cannot delete product — it is used in sales records.");

            if (result == 0)
                throw new Exception("Product not found.");

            return new ApiResponse<object>
            {
                Success = true,
                Data = null
            };
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