using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IServices;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<SaleResponse>>> GetSales([FromQuery] PaginationRequest request)
        {
            var response = await _saleService.GetAllSalesAsync(request);

            // Add success message for frontend
            response.Success = true;
            response.Message = "Fetched sales successfully.";

            return Ok(response);
        }

        [HttpPost("getById")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> GetSale([FromBody] GetByIdRequest request)
        {
            var response = await _saleService.GetSaleByIdAsync(request.Id);

            // Only add success message, data stays from service
            return Ok(new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Fetched sale successfully.",
                Data = response.Data
            });
        }

        [HttpPost("getByIdWithDetails")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> GetSaleWithDetails([FromBody] GetByIdRequest request)
        {
            var response = await _saleService.GetSaleWithDetailsAsync(request.Id);

            return Ok(new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Fetched sale with details successfully.",
                Data = response.Data
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> CreateSale([FromBody] CreateSaleRequest request)
        {
            var response = await _saleService.CreateSaleAsync(request);

            return Ok(new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Sale created successfully.",
                Data = response.Data
            });
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> UpdateSale([FromBody] UpdateSaleRequest request)
        {
            var response = await _saleService.UpdateSaleAsync(request);

            return Ok(new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully.",
                Data = response.Data
            });
        }

        [HttpPost("delete")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSale([FromBody] DeleteRequest request)
        {
            var response = await _saleService.DeleteSaleAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Sale deleted successfully.",
                Data = response.Data
            });
        }
    }
}
