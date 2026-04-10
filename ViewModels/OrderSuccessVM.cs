using System;
using System.Collections.Generic;

namespace EcommerceMVC.ViewModels
{
    public class OrderSuccessVM
    {
        public int MaHd { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        public string HoTen { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;

        public string CachVanChuyen { get; set; } = string.Empty;
        public string CachThanhToan { get; set; } = string.Empty;

        public double PhiVanChuyen { get; set; }
        public double TongTienHang { get; set; }
        public double GiamGia { get; set; }
        public double TongThanhToan => TongTienHang - GiamGia + PhiVanChuyen;

        public List<OrderSuccessItemVM> Items { get; set; } = new();
    }

    public class OrderSuccessItemVM
    {
        public int MaHh { get; set; }
        public string TenHh { get; set; } = string.Empty;
        public string Hinh { get; set; } = string.Empty;

        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien => DonGia * SoLuong;
    }
}