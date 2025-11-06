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

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<SaleResponse>>> GetSales([FromQuery] PaginationRequest request)
        {
            var response = await _saleService.GetAllSalesAsync(request);
            return Ok(response);
        }

        [HttpPost("getById")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> GetSale([FromBody] GetByIdRequest request)
        {
            var response = await _saleService.GetSaleByIdAsync(request.Id);
            return Ok(response);
        }

        [HttpPost("getByIdWithDetails")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> GetSaleWithDetails([FromBody] GetByIdRequest request)
        {
            var response = await _saleService.GetSaleWithDetailsAsync(request.Id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> CreateSale([FromBody] CreateSaleRequest request)
        {
            var response = await _saleService.CreateSaleAsync(request);
            return Ok(response);
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<SaleResponse>>> UpdateSale([FromBody] UpdateSaleRequest request)
        {
            var response = await _saleService.UpdateSaleAsync(request);
            return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSale([FromBody] DeleteRequest request)
        {
            var response = await _saleService.DeleteSaleAsync(request);
            return Ok(response);
        }
    }
}