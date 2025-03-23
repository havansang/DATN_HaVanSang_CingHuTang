using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalMoney { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
