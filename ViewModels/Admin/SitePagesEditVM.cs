using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.ViewModels.Admin;

public class SitePagesEditVM
{
    [Display(Name = "Giới thiệu doanh nghiệp")]
    public string? GioiThieuDoanhNghiepHtml { get; set; }

    [Display(Name = "Về chúng tôi")]
    public string? VeChungToiHtml { get; set; }

    [Display(Name = "Tuyển dụng")]
    public string? TuyenDungHtml { get; set; }

    [Display(Name = "Hỗ trợ khách hàng")]
    public string? HoTroKhachHangHtml { get; set; }

    [Display(Name = "Liên hệ")]
    public string? LienHeHtml { get; set; }
}