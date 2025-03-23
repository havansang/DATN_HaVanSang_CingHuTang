using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class ProductSize
{
    public int Id { get; set; }

    public string? SizeCode { get; set; }

    public string? SizeName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
