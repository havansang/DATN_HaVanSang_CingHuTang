using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class ProductType
{
    public int Id { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    public int? GroupTypeId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
