using Backend.DTOs.Requests;
using Backend.DTOs.Responses;
using Backend.IRepository;
using Backend.IServices;
using Backend.Models;

namespace Backend.Service
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ISalespersonRepository _salespersonRepository;

        public SaleService(ISaleRepository saleRepository, ISalespersonRepository salespersonRepository)
        {
            _saleRepository = saleRepository;
            _salespersonRepository = salespersonRepository;
        }

        public async Task<PaginatedResponse<SaleResponse>> GetAllSalesAsync(PaginationRequest request)
        {
            var sales = await _saleRepository.GetAllSalesAsync(request.PageNumber, request.PageSize);

            return new PaginatedResponse<SaleResponse>
            {
                Success = true,
                Message = "Fetched sales successfully.",
                Data = sales.Data.Select(s => MapToResponse(s)).ToList(),
                CurrentPage = sales.CurrentPage,
                PageSize = sales.PageSize,
                TotalRecords = sales.TotalRecords,
                TotalPages = sales.TotalPages
            };
        }

        public async Task<ApiResponse<SaleResponse>> GetSaleByIdAsync(int id)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(id);

            if (sale == null)
            {
                return new ApiResponse<SaleResponse>
                {
                    Success = false,
                    Message = $"Sale with ID {id} not found.",
                    Data = null
                };
            }

            return new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Fetched sale successfully.",
                Data = MapToResponse(sale)
            };
        }

        public async Task<ApiResponse<SaleResponse>> CreateSaleAsync(CreateSaleRequest request)
        {
            if (request.SalespersonId.HasValue)
            {
                var salesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId.Value);
                if (salesperson == null)
                {
                    return new ApiResponse<SaleResponse>
                    {
                        Success = false,
                        Message = "Salesperson not found.",
                        Data = null
                    };
                }
            }

            var sale = new Sale
            {
                SalespersonId = request.SalespersonId,
                Comments = request.Comments,
                SaleDate = request.SaleDate ?? DateTime.Now,
                SaleDetails = request.SaleDetails?.Select(d => new Sale.SaleDetailDto
                {
                    ProductId = d.ProductId,
                    RetailPrice = d.RetailPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount,
                    RowState = "Added"
                }).ToList()
            };

            var createdSale = await _saleRepository.CreateSaleAsync(sale);

            if (createdSale == null)
            {
                return new ApiResponse<SaleResponse>
                {
                    Success = false,
                    Message = "Failed to create sale.",
                    Data = null
                };
            }

            return new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Sale created successfully.",
                Data = MapToResponse(createdSale)
            };
        }

        public async Task<ApiResponse<SaleResponse>> UpdateSaleAsync(UpdateSaleRequest request)
        {
            var existingSale = await _saleRepository.GetSaleByIdAsync(request.SaleId);
            if (existingSale == null)
            {
                return new ApiResponse<SaleResponse>
                {
                    Success = false,
                    Message = "Sale not found.",
                    Data = null
                };
            }

            if (request.SalespersonId.HasValue)
            {
                var salesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId.Value);
                if (salesperson == null)
                {
                    return new ApiResponse<SaleResponse>
                    {
                        Success = false,
                        Message = "Salesperson not found.",
                        Data = null
                    };
                }
            }

            var sale = new Sale
            {
                SaleId = request.SaleId,
                SalespersonId = request.SalespersonId,
                Comments = request.Comments,
                SaleDate = request.SaleDate,
                SaleDetails = request.SaleDetails?.Select(d => new Sale.SaleDetailDto
                {
                    ProductId = d.ProductId,
                    RetailPrice = d.RetailPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount,
                    RowState = d.RowState
                }).ToList()
            };

            var success = await _saleRepository.UpdateSaleAsync(sale);

            if (!success)
            {
                return new ApiResponse<SaleResponse>
                {
                    Success = false,
                    Message = "Failed to update sale.",
                    Data = null
                };
            }

            var updatedSale = await _saleRepository.GetSaleWithDetailsAsync(request.SaleId);

            return new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully.",
                Data = MapToResponse(updatedSale!)
            };
        }

        public async Task<ApiResponse<object>> DeleteSaleAsync(DeleteRequest request)
        {
            var existingSale = await _saleRepository.GetSaleByIdAsync(request.Id);
            if (existingSale == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Sale not found.",
                    Data = null
                };
            }

            var result = await _saleRepository.DeleteSaleAsync(request.Id);

            if (result == 1)
            {
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Sale deleted successfully.",
                    Data = null
                };
            }

            return new ApiResponse<object>
            {
                Success = false,
                Message = "Failed to delete sale.",
                Data = null
            };
        }

        public async Task<ApiResponse<SaleResponse>> GetSaleWithDetailsAsync(int id)
        {
            var sale = await _saleRepository.GetSaleWithDetailsAsync(id);

            if (sale == null)
            {
                return new ApiResponse<SaleResponse>
                {
                    Success = false,
                    Message = "Sale not found.",
                    Data = null
                };
            }

            return new ApiResponse<SaleResponse>
            {
                Success = true,
                Message = "Fetched sale with details successfully.",
                Data = MapToResponse(sale)
            };
        }

        private static SaleResponse MapToResponse(Sale sale)
        {
            return new SaleResponse
            {
                SaleId = sale.SaleId,
                Total = sale.Total,
                SaleDate = sale.SaleDate,
                CreatedDate = sale.CreatedDate,
                SalespersonId = sale.SalespersonId,
                SalespersonName = sale.SalespersonName,
                Comments = sale.Comments,
                UpdatedDate = sale.UpdatedDate,
                SaleDetails = sale.SaleDetails?.Select(d => new SaleDetailResponse
                {
                    ProductId = d.ProductId,
                    RetailPrice = d.RetailPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount,
                    RowState = d.RowState
                }).ToList()
            };
        }
    }
}