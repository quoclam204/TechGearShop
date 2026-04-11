using EcommerceMVC.Data;
using EcommerceMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SitePagesAdminController : Controller
{
    private readonly Hshop2023Context _db;

    public SitePagesAdminController(Hshop2023Context db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Edit()
    {
        var vm = new SitePagesEditVM
        {
            GioiThieuDoanhNghiepHtml = GetHtml("gioi-thieu-doanh-nghiep"),
            VeChungToiHtml = GetHtml("ve-chung-toi"),
            TuyenDungHtml = GetHtml("tuyen-dung"),
            HoTroKhachHangHtml = GetHtml("ho-tro-khach-hang"),
            LienHeHtml = GetHtml("lien-he")
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(SitePagesEditVM vm)
    {
        Upsert("gioi-thieu-doanh-nghiep", "Giới thiệu doanh nghiệp", vm.GioiThieuDoanhNghiepHtml);
        Upsert("ve-chung-toi", "Về chúng tôi", vm.VeChungToiHtml);
        Upsert("tuyen-dung", "Tuyển dụng", vm.TuyenDungHtml);
        Upsert("ho-tro-khach-hang", "Hỗ trợ khách hàng", vm.HoTroKhachHangHtml);
        Upsert("lien-he", "Liên hệ", vm.LienHeHtml);

        _db.SaveChanges();

        TempData["ToastSuccess"] = "Đã lưu nội dung các trang footer.";
        return RedirectToAction(nameof(Edit));
    }

    private string? GetHtml(string slug)
        => _db.SitePages.SingleOrDefault(x => x.Slug == slug)?.ContentHtml;

    private void Upsert(string slug, string title, string? html)
    {
        var entity = _db.SitePages.SingleOrDefault(x => x.Slug == slug);
        if (entity == null)
        {
            entity = new SitePage
            {
                Slug = slug,
                Title = title,
                ContentHtml = html,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _db.SitePages.Add(entity);
            return;
        }

        entity.Title = title;
        entity.ContentHtml = html;
        entity.UpdatedAtUtc = DateTime.UtcNow;
    }
}