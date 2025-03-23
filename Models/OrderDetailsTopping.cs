using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class OrderDetailsTopping
{
    public int Id { get; set; }

    public int OrderDetailsId { get; set; }

    public int ToppingId { get; set; }

    public decimal? ToppingPrice { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
