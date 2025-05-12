using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Review
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? AccountId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; }
}
