namespace CingHuTang.Models.DTOs
{
    public class OrderDetailDTO : OrderDetail
    {
        public List<OrderDetailsTopping> LstTopping{ get; set; }
    }
}
