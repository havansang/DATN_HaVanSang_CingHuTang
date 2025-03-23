using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class CartTopping
{
    public int Id { get; set; }

    public int? CartId { get; set; }

    public int? ToppingId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
