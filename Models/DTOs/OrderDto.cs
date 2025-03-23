namespace CingHuTang.Models.DTOs
{
    public class OrderDto : Order
    {
       public List<OrderDetailDTO>? Details { get; set; }
       public string? StatusText { get; set; }
       public decimal TotalMoney { get; set; }
        public string DateFormat { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
