using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string SupplierCode { get; set; } = null!;

    public string? SupplierName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Decription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDelete { get; set; }
}
