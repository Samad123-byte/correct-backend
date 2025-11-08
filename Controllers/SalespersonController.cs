using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalespersonController : ControllerBase
    {
        private readonly ISalespersonService _salespersonService;

        public SalespersonController(ISalespersonService salespersonService)
        {
            _salespersonService = salespersonService;
        }

        [HttpGet("getall")]
        public async Task<ActionResult<PaginatedResponse<SalespersonResponse>>> GetAll([FromQuery] PaginationRequest request)
        {
            var response = await _salespersonService.GetAllSalespersonsAsync(request);
            response.Success = true;
            response.Message = "Salespersons fetched successfully.";
            return Ok(response);
        }

        [HttpPost("getById")]
        public async Task<ActionResult<ApiResponse<SalespersonResponse>>> GetById([FromBody] GetByIdRequest request)
        {
            var response = await _salespersonService.GetSalespersonByIdAsync(request.Id);
            response.Success = true;
            response.Message = "Salesperson fetched successfully.";
            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<SalespersonResponse>>> Add([FromBody] CreateSalespersonRequest request)
        {
            var response = await _salespersonService.CreateSalespersonAsync(request);
            response.Success = true;
            response.Message = "Salesperson added successfully.";
            return Ok(response);
        }

        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse<SalespersonResponse>>> Update([FromBody] UpdateSalespersonRequest request)
        {
            var response = await _salespersonService.UpdateSalespersonAsync(request);
            response.Success = true;
            response.Message = "Salesperson updated successfully.";
            return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<ActionResult<ApiResponse<object>>> Delete([FromBody] DeleteRequest request)
        {
            var response = await _salespersonService.DeleteSalespersonAsync(request);
            response.Success = true;
            response.Message = "Salesperson deleted successfully.";
            return Ok(response);
        }
    }
}