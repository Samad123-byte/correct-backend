using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IServices;
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

       
        [HttpPost("getAll")]
        public async Task<ActionResult<PaginatedResponse<SaleResponse>>> GetSales([FromBody] PaginationRequest request)
        {
            var response = await _saleService.GetAllSalesAsync(request);
            response.Success = true;
            response.Message = "Fetched sales successfully.";
            return Ok(response);
        }

        [HttpPost("getById")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> GetSale([FromBody] GetByIdRequest request)
        {
            var response = await _saleService.GetSaleByIdAsync(request.Id);
            response.Success = true;
            response.Message = "Fetched sale successfully.";
            return Ok(response);
        }

        [HttpPost("getByIdWithDetails")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> GetSaleWithDetails([FromBody] GetByIdRequest request)
        {
            var response = await _saleService.GetSaleWithDetailsAsync(request.Id);
            response.Success = true;
            response.Message = "Fetched sale with details successfully.";
            return Ok(response);
        }

        // ✅ FIXED: Added "create" route to differentiate from getAll
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> CreateSale([FromBody] CreateSaleRequest request)
        {
            var response = await _saleService.CreateSaleAsync(request);
            response.Success = true;
            response.Message = "Sale created successfully.";
            return Ok(response);
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> UpdateSale([FromBody] UpdateSaleRequest request)
        {
            var response = await _saleService.UpdateSaleAsync(request);
            response.Success = true;
            response.Message = "Sale updated successfully.";
            return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSale([FromBody] DeleteRequest request)
        {
            var response = await _saleService.DeleteSaleAsync(request);
            response.Success = true;
            response.Message = "Sale deleted successfully.";
            return Ok(response);
        }
    }
}
