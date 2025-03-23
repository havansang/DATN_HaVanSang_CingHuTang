using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }

    public int? ProductTypeId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
