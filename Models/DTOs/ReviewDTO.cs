namespace CingHuTang.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
