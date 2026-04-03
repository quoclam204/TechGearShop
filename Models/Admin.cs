using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechGearShop.Models;

[Table("_Admin")]
public partial class Admin
{
    [Key]
    [Column("ma_admin")]
    public int MaAdmin { get; set; }

    [Column("ten_dang_nhap")]
    [StringLength(50)]
    public string TenDangNhap { get; set; } = null!;

    [Column("mat_khau")]
    [StringLength(20)]
    [Unicode(false)]
    public string? MatKhau { get; set; }

    [Column("email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }
}
