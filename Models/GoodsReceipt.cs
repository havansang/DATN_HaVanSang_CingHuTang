using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class GoodsReceipt
{
    public int Id { get; set; }

    public int AccoutId { get; set; }

    public int SupplierId { get; set; }

    public DateTime ReceiptedDate { get; set; }

    public string GoodsReceiptCode { get; set; } = null!;

    public string? Decription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDelete { get; set; }
}
