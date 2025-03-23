using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class GoodsIssue
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public string? GoodIssueCode { get; set; }

    public string? Decription { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
