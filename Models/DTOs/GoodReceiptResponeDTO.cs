namespace CingHuTang.Models.DTOs
{
    public class GoodReceiptResponeDTO : GoodsReceipt
    {
        public string FullName { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalMoney { get; set; }
    }
}
