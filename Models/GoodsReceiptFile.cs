using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class GoodsReceiptFile
{
    public int Id { get; set; }

    public int GoodsReceiptId { get; set; }

    public string? FileUrl { get; set; }

    public string? FileName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
