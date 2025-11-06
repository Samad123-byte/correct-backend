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
            var salespersons = await _salespersonRepository.GetAllSalespersonsAsync(request.PageNumber, request.PageSize);

            return new PaginatedResponse<SalespersonResponse>
            {
                Success = true,
                Message = "Salespersons fetched successfully.",
                Data = salespersons.Data.Select(s => MapToResponse(s)).ToList(),
                CurrentPage = salespersons.CurrentPage,
                PageSize = salespersons.PageSize,
                TotalRecords = salespersons.TotalRecords,
                TotalPages = salespersons.TotalPages
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> GetSalespersonByIdAsync(int id)
        {
            var salesperson = await _salespersonRepository.GetSalespersonByIdAsync(id);

            if (salesperson == null)
            {
                return new ApiResponse<SalespersonResponse>
                {
                    Success = false,
                    Message = "Salesperson not found.",
                    Data = null
                };
            }

            return new ApiResponse<SalespersonResponse>
            {
                Success = true,
                Message = "Salesperson fetched successfully.",
                Data = MapToResponse(salesperson)
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> CreateSalespersonAsync(CreateSalespersonRequest request)
        {
            var allSalespersons = await _salespersonRepository.GetAllSalespersonsAsync(1, int.MaxValue);
            var duplicateExists = allSalespersons.Data.Any(s =>
                s.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase) ||
                s.Code.Equals(request.Code, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                return new ApiResponse<SalespersonResponse>
                {
                    Success = false,
                    Message = "Duplicate Name or Code.",
                    Data = null
                };
            }

            var salesperson = new Salesperson
            {
                Name = request.Name,
                Code = request.Code,
                EnteredDate = request.EnteredDate
            };

            var result = await _salespersonRepository.CreateSalespersonAsync(salesperson);

            if (result <= 0)
            {
                return new ApiResponse<SalespersonResponse>
                {
                    Success = false,
                    Message = "Failed to create salesperson.",
                    Data = null
                };
            }

            salesperson.SalespersonId = result;

            return new ApiResponse<SalespersonResponse>
            {
                Success = true,
                Message = "Salesperson added successfully.",
                Data = MapToResponse(salesperson)
            };
        }

        public async Task<ApiResponse<SalespersonResponse>> UpdateSalespersonAsync(UpdateSalespersonRequest request)
        {
            var existingSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId);
            if (existingSalesperson == null)
            {
                return new ApiResponse<SalespersonResponse>
                {
                    Success = false,
                    Message = "Salesperson not found.",
                    Data = null
                };
            }

            var salesperson = new Salesperson
            {
                SalespersonId = request.SalespersonId,
                Name = request.Name,
                Code = request.Code,
                EnteredDate = request.EnteredDate
            };

            var result = await _salespersonRepository.UpdateSalespersonAsync(salesperson);

            if (result == -1)
            {
                return new ApiResponse<SalespersonResponse>
                {
                    Success = false,
                    Message = "Duplicate Name or Code.",
                    Data = null
                };
            }

            if (result == 0)
            {
                return new ApiResponse<SalespersonResponse>
                {
                    Success = false,
                    Message = "Salesperson not found.",
                    Data = null
                };
            }

            var updated = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId);

            return new ApiResponse<SalespersonResponse>
            {
                Success = true,
                Message = "Salesperson updated successfully.",
                Data = MapToResponse(updated!)
            };
        }

        public async Task<ApiResponse<object>> DeleteSalespersonAsync(DeleteRequest request)
        {
            var existingSalesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.Id);
            if (existingSalesperson == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Salesperson not found.",
                    Data = null
                };
            }

            var result = await _salespersonRepository.DeleteSalespersonAsync(request.Id);

            if (result == 1)
            {
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Salesperson deleted successfully.",
                    Data = null
                };
            }

            if (result == -1)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cannot delete salesperson — it has related sales records.",
                    Data = null
                };
            }

            return new ApiResponse<object>
            {
                Success = false,
                Message = "Failed to delete salesperson.",
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