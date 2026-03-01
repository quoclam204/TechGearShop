using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.ViewComponents
{
    public class FeaturedProductsViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        public FeaturedProductsViewComponent(Hshop2023Context context)
        {
            db = context;
        }

        public IViewComponentResult Invoke(int count = 5)
        {
            // Top sản phẩm được xem nhiều nhất
            var featuredProducts = db.HangHoas
                .OrderByDescending(p => p.SoLanXem)
                .Take(count)
                .Select(p => new HangHoaVM
                {
                    MaHh = p.MaHh,
                    TenHH = p.TenHh,
                    DonGia = p.DonGia ?? 0,
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai
                })
                .ToList();

            return View(featuredProducts);
        }
    }
}