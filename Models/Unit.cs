using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Unit
{
    public int Id { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitName { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
