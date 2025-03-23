using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class GoodsReceiptDetail
{
    public int Id { get; set; }

    public int GoodsReceiptId { get; set; }

    public int MaterialId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDelete { get; set; }
}
