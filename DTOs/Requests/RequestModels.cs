namespace Backend.DTOs.Requests
{
    // ============ PRODUCT REQUESTS ============
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? ImageURL { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
    }

    public class UpdateProductRequest
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? ImageURL { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }

  
        public class PaginationRequest
        {
            public int StartIndex { get; set; } = 0;   
            public int EndIndex { get; set; } = 10;   
        }

    

    // ============ SALESPERSON REQUESTS ============
    public class CreateSalespersonRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime? EnteredDate { get; set; }
    }

    public class UpdateSalespersonRequest
    {
        public int SalespersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime? EnteredDate { get; set; }
    }

    // ============ SALE REQUESTS ============
    public class CreateSaleRequest
    {
        public int? SalespersonId { get; set; }
        public string? Comments { get; set; }
        public DateTime? SaleDate { get; set; }
        public List<SaleDetailRequest>? SaleDetails { get; set; }
    }

    public class UpdateSaleRequest
    {
        public int SaleId { get; set; }
        public int? SalespersonId { get; set; }
        public string? Comments { get; set; }
        public DateTime? SaleDate { get; set; }
        public List<SaleDetailRequest>? SaleDetails { get; set; }
    }

    public class SaleDetailRequest
    {
        public int ProductId { get; set; }
        public decimal RetailPrice { get; set; }
        public int Quantity { get; set; }
        public decimal? Discount { get; set; }
        public string RowState { get; set; } = "Unchanged";
    }


    public class GetByIdRequest
    {
        public int Id { get; set; }
    }
}