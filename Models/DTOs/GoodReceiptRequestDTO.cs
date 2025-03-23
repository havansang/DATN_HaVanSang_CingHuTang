namespace CingHuTang.Models.DTOs
{
    public class GoodReceiptRequestDTO
    {
        public string Request { get; set; }
        public int PageNumber { get; set; } = 1;
        public int AccountID { get; set; }
        public DateTime DateStart { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime DateEnd { get; set; } = DateTime.Now;
    }
}
