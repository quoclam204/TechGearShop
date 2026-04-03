using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechGearShop.Models;

[Table("ChiTietHD")]
public partial class ChiTietHd
{
    [Key]
    [Column("MaCT")]
    public int MaCt { get; set; }

    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column("MaHH")]
    public int MaHh { get; set; }

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public double GiamGia { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("ChiTietHds")]
    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaHh")]
    [InverseProperty("ChiTietHds")]
    public virtual HangHoa MaHhNavigation { get; set; } = null!;
}
