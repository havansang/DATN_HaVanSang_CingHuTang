namespace CingHuTang.Models.DTOs
{
    public class AddToCartDTO 
    {
        public int ProductDetailID { get; set; }
        public int AccountID { get; set; }
        public int Quantity { get; set; }
        public List<int> ToppingIDs { get; set; }
    }
}
