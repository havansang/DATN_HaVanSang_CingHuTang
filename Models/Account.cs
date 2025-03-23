using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>
    /// 1: Khách hàng, 2: admin, 3: nhân viên, 4: chủ cửa hàng
    /// </summary>
    public int Role { get; set; }

    public string? FullName { get; set; }

    /// <summary>
    /// 0: Nam, 1: Nữ
    /// </summary>
    public int? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public int IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDelete { get; set; }
}
