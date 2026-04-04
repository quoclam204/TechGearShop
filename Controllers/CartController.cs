using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechGearShop.ViewModels;

namespace EcommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context)
        {
            db = context;
        }

        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        [Authorize]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims
                    .SingleOrDefault(x => x.Type == MySetting.CLAIM_CUSTOMERID)?.Value;

                var khachHang = new KhachHang();

                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs
                        .SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang?.HoTen,
                    DiaChi = model.DiaChi ?? khachHang?.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang?.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                using (var transaction = db.Database.BeginTransaction()) // ✅ đúng cách
                {
                    try
                    {
                        // 1. Lưu hóa đơn
                        db.Add(hoaDon);
                        db.SaveChanges();

                        // 2. Lưu chi tiết hóa đơn
                        var cthds = new List<ChiTietHd>();
                        foreach (var item in Cart)
                        {
                            cthds.Add(new ChiTietHd
                            {
                                MaHd = hoaDon.MaHd,
                                MaHh = item.MaHh,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia,
                                GiamGia = 0
                            });
                        }

                        db.AddRange(cthds);
                        db.SaveChanges();

                        // 3. Commit sau cùng
                        transaction.Commit(); // ✅ đúng chỗ

                        // 4. Xóa giỏ hàng
                        HttpContext.Session.Set<List<CartItem>>(
                            MySetting.CART_KEY,
                            new List<CartItem>()
                        );

                        return View("Success");
                    }
                    catch (Exception)
                    {
                        transaction.Rollback(); // ✅ rollback nếu lỗi
                        throw; // nên throw để debug
                    }
                }
            }

            return View(Cart);
        }
    }
}

