using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public decimal Total { get; set; }
        public DateTime? SaleDate { get; set; }
        public int? SalespersonId { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // For frontend display
        public string? SalespersonName { get; set; }

        // Sale details
        public List<SaleDetailDto>? SaleDetails { get; set; }

        public class SaleDetailDto
        {
            public int ProductId { get; set; }
            public decimal RetailPrice { get; set; }
            public int Quantity { get; set; }
            public decimal? Discount { get; set; }

            // For frontend row state management
            public string RowState { get; set; } = "Unchanged";
        }
    }
}
