using System;
using System.Collections.Generic;

namespace CingHuTang.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public string? OrderCode { get; set; }

    public string? CustomerName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    /// <summary>
    /// 0: Chờ xác nhận; 1: Đang giao; 2: Thành công; 3: Hủy hàng
    /// </summary>
    public int? Status { get; set; }

    public string? ReasonCancel { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
