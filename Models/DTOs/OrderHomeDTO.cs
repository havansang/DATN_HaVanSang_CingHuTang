namespace CingHuTang.Models.DTOs
{
    public class OrderHomeDTO
    {
        public decimal MoneyLastWeek { get; set; }
        public decimal MoneyCurrentWeek { get; set; }
        public decimal TotalLastWeek { get; set; }
        public decimal TotalCurrentWeek { get; set; }
        public decimal TotalUnProcess { get; set; }
    }
}
