namespace EcommerceMVC.ViewModels
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
    }
    public class ChiTietHangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
        public string ChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }
        public int MaLoai { get; set; } // ✅ Thêm để lấy mã loại
        public List<HangHoaVM> SanPhamLienQuan { get; set; } = new List<HangHoaVM>(); // ✅ Thêm danh sách sản phẩm liên quan
    }
}
