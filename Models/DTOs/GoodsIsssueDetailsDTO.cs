namespace CingHuTang.Models.DTOs
{
    public class GoodsIsssueDetailsDTO : GoodsIssueDetail
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
