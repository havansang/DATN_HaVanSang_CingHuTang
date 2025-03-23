using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? ProductSizeId { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
