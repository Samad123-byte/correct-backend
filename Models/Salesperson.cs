namespace Backend.Models
{
    public class Salesperson
    {
        public int SalespersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime? EnteredDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation property to Sales
        public virtual ICollection<Sale>? Sales { get; set; }
    }
}
