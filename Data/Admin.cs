using System;
using System.Collections.Generic;

namespace TechGearShop.Data;

public partial class Admin
{
    public int MaAdmin { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string? MatKhau { get; set; }

    public string? Email { get; set; }
}
