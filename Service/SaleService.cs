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
            var sales = await _saleRepository.GetAllSalesAsync(request.StartIndex, request.EndIndex);

            return new PaginatedResponse<SaleResponse>
            {
                Data = sales.Data.Select(MapToResponse).ToList(),
                StartIndex = sales.StartIndex,
                EndIndex = sales.EndIndex,
                TotalRecords = sales.TotalRecords
            };
        }

        public async Task<ApiResponse<SaleResponse>> GetSaleByIdAsync(int id)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(id);
            if (sale == null) throw new Exception($"Sale with ID {id} not found.");

            return new ApiResponse<SaleResponse>
            {
                Data = MapToResponse(sale)
            };
        }

        public async Task<ApiResponse<SaleResponse>> CreateSaleAsync(CreateSaleRequest request)
        {
            if (request.SalespersonId.HasValue)
            {
                var salesperson = await _salespersonRepository.GetSalespersonByIdAsync(request.SalespersonId.Value);
            }

            var sale = new Sale
            {
                SalespersonId = request.SalespersonId,
                Comments = request.Comments,
                SaleDate = request.SaleDate ?? DateTime.Now,
                SaleDetails = request.SaleDetails?.Select(d => new Sale.SaleDetailDto
                {
                    SaleDetailId = d.SaleDetailId, // ✅ ADDED
                    ProductId = d.ProductId,
                    RetailPrice = d.RetailPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount ?? 0,
                    RowState = "Added"
                }).ToList()
            };

            var createdSale = await _saleRepository.CreateSaleAsync(sale);
            if (createdSale == null) throw new Exception("Failed to create sale.");

            return new ApiResponse<SaleResponse>
            {
                Data = MapToResponse(createdSale)
            };
        }

        public async Task<ApiResponse<SaleResponse>> UpdateSaleAsync(UpdateSaleRequest request)
        {
            var existingSale = await _saleRepository.GetSaleByIdAsync(request.SaleId);

            // ✅ Store the original rowStates from the request
            var requestedRowStates = request.SaleDetails?
                .ToDictionary(d => d.ProductId, d => d.RowState)
                ?? new Dictionary<int, string>();

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
            if (!success) throw new Exception("Failed to update sale.");

            var updatedSale = await _saleRepository.GetSaleWithDetailsAsync(request.SaleId);

            // ✅ Restore the rowStates in the response
            if (updatedSale?.SaleDetails != null)
            {
                foreach (var detail in updatedSale.SaleDetails)
                {
                    if (requestedRowStates.ContainsKey(detail.ProductId))
                    {
                        detail.RowState = requestedRowStates[detail.ProductId];
                    }
                    else
                    {
                        detail.RowState = "Unchanged"; // Default for items not in request
                    }
                }
            }

            return new ApiResponse<SaleResponse>
            {
                Data = MapToResponse(updatedSale!)
            };
        }

        public async Task<ApiResponse<object>> DeleteSaleAsync(DeleteRequest request)
        {
            var existingSale = await _saleRepository.GetSaleByIdAsync(request.Id);
            if (existingSale == null) throw new Exception("Sale not found.");

            var result = await _saleRepository.DeleteSaleAsync(request.Id);
            if (result != 1) throw new Exception("Failed to delete sale.");

            return new ApiResponse<object>
            {
                Data = null
            };
        }

        public async Task<ApiResponse<SaleResponse>> GetSaleWithDetailsAsync(int id)
        {
            var sale = await _saleRepository.GetSaleWithDetailsAsync(id);

            return new ApiResponse<SaleResponse>
            {
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
                    RowState = d.RowState ?? "Unchanged" // ✅ Ensure rowState is never null
                }).ToList()
            };
        }
    }
}