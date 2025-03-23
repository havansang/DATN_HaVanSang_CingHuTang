using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
