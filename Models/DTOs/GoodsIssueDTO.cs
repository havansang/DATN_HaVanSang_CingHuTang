namespace CingHuTang.Models.DTOs
{
    public class GoodsIssueDTO : GoodsIssue
    {
        public List<GoodsIssueDetail> LstDetails { get; set; }
        public string FullName { get; set; }

    }
}
