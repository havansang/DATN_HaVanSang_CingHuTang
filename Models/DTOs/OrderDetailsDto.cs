namespace CingHuTang.Models.DTOs
{
    public class OrderDetailsDto
    {
        public int OrderDetailID { get; set; }
        public string? ProductName { get; set; }
        public string? SizeName { get; set; }
        public decimal TotalMoney { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<OrderDetailsToppingDTO>? lstTopping { get; set; }
    }
}
