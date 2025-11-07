using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IRepository;
using Backend.IServices;
using Backend.Models;

namespace Backend.Service
{
    public class SalespersonService : ISalespersonService
    {
        private readonly ISalespersonRepository _salespersonRepository;

        public SalespersonService(ISalespersonRepository salespersonRepository)
        {
            _salespersonRepository = salespersonRepository;
        }

        public async Task<PaginatedResponse<SalespersonResponse>> GetAllSalespersonsAsync(PaginationRequest request)
        {
            var salespersons = await _salespersonRepository.GetAllSalespersonsAsync();

            return new PaginatedResponse<SalespersonResponse>
            {
                Data = salespersons.Data.Select(MapToResponse).ToList(),
                TotalRecords = salespersons.TotalRecords,
                StartIndex = salespersons.StartIndex,
                EndIndex = salespersons.EndIndex
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> GetSalespersonByIdAsync(int id)
        {
            var salesperson = await _salespersonRepository.GetSalespersonByIdAsync(id);
           

            return new ApiResponse<SalespersonResponse>
            {
                Data = MapToResponse(salesperson)
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> CreateSalespersonAsync(CreateSalespersonRequest request)
        {
            

            var salesperson = new Salesperson
            {
                Name = request.Name,
                Code = request.Code,
                EnteredDate = request.EnteredDate
            };

            var result = await _salespersonRepository.CreateSalespersonAsync(salesperson);
            if (result <= 0) throw new Exception("Failed to create salesperson.");

            salesperson.SalespersonId = result;

            return new ApiResponse<SalespersonResponse>
            {
                Data = MapToResponse(salesperson)
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> UpdateSalespersonAsync(UpdateSalespersonRequest request)
        {
            var existingSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId);
           
            var salesperson = new Salesperson
            {
                SalespersonId = request.SalespersonId,
                Name = request.Name,
                Code = request.Code,
                EnteredDate = request.EnteredDate
            };

            var result = await _salespersonRepository.UpdateSalespersonAsync(salesperson);
            if (result == -1) throw new Exception("Duplicate Name or Code.");
            if (result == 0) throw new Exception("Salesperson not found.");

            var updated = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId);

            return new ApiResponse<SalespersonResponse>
            {
                Data = MapToResponse(updated!)
            };
        }

        public async Task<ApiResponse<object>> DeleteSalespersonAsync(DeleteRequest request)
        {
            var existingSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.Id);
           

            var result = await _salespersonRepository.DeleteSalespersonAsync(request.Id);
            if (result == -2) throw new Exception("Cannot delete salesperson — it has related sales records.");
            if (result != 1) throw new Exception("Failed to delete salesperson.");

            return new ApiResponse<object> { Data = null };
        }

        private static SalespersonResponse MapToResponse(Salesperson salesperson)
        {
            return new SalespersonResponse
            {
                SalespersonId = salesperson.SalespersonId,
                Name = salesperson.Name,
                Code = salesperson.Code,
                EnteredDate = salesperson.EnteredDate,
                UpdatedDate = salesperson.UpdatedDate
            };
        }
    }
}
