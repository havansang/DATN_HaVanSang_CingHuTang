using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Material
{
    public int Id { get; set; }

    public int UnitId { get; set; }

    public string MaterialCode { get; set; } = null!;

    public string? MaterialName { get; set; }

    public decimal MinQuantity { get; set; }

    public string? Decription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDelete { get; set; }
}
