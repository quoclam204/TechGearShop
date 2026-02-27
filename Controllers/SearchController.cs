using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly Hshop2023Context db;

        public SearchController(Hshop2023Context context)
        {
            db = context;
        }

        // URL: /Search?query=abc
        public IActionResult Index(string? query)
        {
            // Trim và validate
            query = query?.Trim();
            
            var hangHoas = db.HangHoas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            // Truyền thông tin tìm kiếm
            ViewBag.SearchQuery = query;
            ViewBag.TotalResults = result.Count();
            
            return View(result);
        }
    }
}