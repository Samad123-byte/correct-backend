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
            var salespersons = await _salespersonRepository.GetAllSalespersonsAsync(request.StartIndex, request.EndIndex);

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
            if (salesperson == null)
                throw new Exception("Salesperson not found.");

            return new ApiResponse<SalespersonResponse>
            {
                Success = true,
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

            if (result == -1)
                throw new Exception("Duplicate Salesperson Code already exists.");

            if (result <= 0)
                throw new Exception("Failed to create salesperson.");

            // Get the newly created salesperson
            var createdSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(result);
            if (createdSalesperson == null)
                throw new Exception("Salesperson created but could not retrieve details.");

            return new ApiResponse<SalespersonResponse>
            {
                Success = true,
                Data = MapToResponse(createdSalesperson)
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> UpdateSalespersonAsync(UpdateSalespersonRequest request)
        {
            var existingSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId);
            if (existingSalesperson == null)
                throw new Exception("Salesperson not found.");

            var salesperson = new Salesperson
            {
                SalespersonId = request.SalespersonId,
                Name = request.Name,
                Code = request.Code,
                EnteredDate = request.EnteredDate
            };

            var result = await _salespersonRepository.UpdateSalespersonAsync(salesperson);

            if (result == -1)
                throw new Exception("Duplicate Salesperson Code already exists.");

            if (result == 0)
                throw new Exception("Salesperson not found.");

            var updated = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId);

            return new ApiResponse<SalespersonResponse>
            {
                Success = true,
                Data = MapToResponse(updated!)
            };
        }

        public async Task<ApiResponse<object>> DeleteSalespersonAsync(DeleteRequest request)
        {
            var existingSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.Id);
            if (existingSalesperson == null)
                throw new Exception("Salesperson not found.");

            var result = await _salespersonRepository.DeleteSalespersonAsync(request.Id);

            if (result == -2)
                throw new Exception("Cannot delete salesperson — it has related sales records.");

            if (result == -1)
                throw new Exception("Salesperson not found.");

            if (result != 1)
                throw new Exception("Failed to delete salesperson.");

            return new ApiResponse<object>
            {
                Success = true,
                Data = null
            };
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