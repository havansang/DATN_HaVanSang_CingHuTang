namespace CingHuTang.Models.DTOs
{
    public class MaterialDTO : Material
    {
        public string SupplierName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal ToltalQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
