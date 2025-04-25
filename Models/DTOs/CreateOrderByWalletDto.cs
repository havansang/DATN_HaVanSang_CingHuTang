namespace CingHuTang.Models.DTOs
{
    public class CreateOrderByWalletDto
    {
        public OrderDto Content { get; set; } // content từ JS
        public double Togntien { get; set; }  // tổng tiền
    }
}
