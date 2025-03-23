namespace CingHuTang.Models.DTOs
{
    public class GoodsReceiptDetailsDTO : GoodsReceiptDetail
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal TotalMoney { get; set; }
    }
}
