namespace Backend.DTOs.Responses
{
    // ============ BASE RESPONSES ============
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public List<T> Data { get; set; } = new List<T>();

        // Pagination info
        public int StartIndex { get; set; }      // first row of current page
        public int EndIndex { get; set; }        // last row of current page
        public int TotalRecords { get; set; }    // total rows in table
    }


    // ============ PRODUCT RESPONSES ============
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? ImageURL { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    // ============ SALESPERSON RESPONSES ============
    public class SalespersonResponse
    {
        public int SalespersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime? EnteredDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    // ============ SALE RESPONSES ============
    public class SaleResponse
    {
        public int SaleId { get; set; }
        public decimal Total { get; set; }
        public DateTime? SaleDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? SalespersonId { get; set; }
        public string? SalespersonName { get; set; }
        public string? Comments { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<SaleDetailResponse>? SaleDetails { get; set; }
    }

    public class SaleDetailResponse
    {
        public int? SaleDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal RetailPrice { get; set; }
        public int Quantity { get; set; }
        public decimal? Discount { get; set; }
        public string RowState { get; set; } = "Unchanged";
    }
}