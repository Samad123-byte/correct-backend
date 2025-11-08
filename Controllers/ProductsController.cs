using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<PaginatedResponse<ProductResponse>>> GetProducts([FromQuery] PaginationRequest request)
        {
            var response = await _productService.GetAllProductsAsync(request);
            response.Success = true;
            response.Message = "Products fetched successfully.";
            return Ok(response);
        }

        [HttpPost("getById")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProduct([FromBody] GetByIdRequest request)
        {
            var response = await _productService.GetProductByIdAsync(request.Id);
            response.Success = true;
            response.Message = "Product fetched successfully.";
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> CreateProduct([FromBody] CreateProductRequest request)
        {
            var response = await _productService.CreateProductAsync(request);
            response.Success = true;
            response.Message = "Product created successfully.";
            return Ok(response);
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            var response = await _productService.UpdateProductAsync(request);
            response.Success = true;
            response.Message = "Product updated successfully.";
            return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<ActionResult<ApiResponse<object>>> Delete([FromBody] DeleteRequest request)
        {
            var response = await _productService.DeleteProductAsync(request);
            response.Success = true;
            response.Message = "Product deleted successfully.";
            return Ok(response);
        }
    }
}