using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Add(hoaDon);
                        db.SaveChanges();

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

                        transaction.Commit();

                        // Xóa giỏ hàng
                        HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                        // Thông báo hiển thị 1 lần
                        TempData["CheckoutSuccess"] = "Thanh toán thành công. Cảm ơn bạn đã đặt hàng!";

                        // PRG pattern: redirect sang trang Success + truyền mã hóa đơn
                        return RedirectToAction(nameof(Success), new { id = hoaDon.MaHd });
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return View(Cart);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Success(int id)
        {
            var customerId = HttpContext.User.Claims
                .SingleOrDefault(x => x.Type == MySetting.CLAIM_CUSTOMERID)?.Value;

            var hoaDon = db.HoaDons
                .Include(x => x.MaTrangThaiNavigation)
                .Include(x => x.ChiTietHds)
                    .ThenInclude(ct => ct.MaHhNavigation)
                .SingleOrDefault(x => x.MaHd == id && x.MaKh == customerId);

            if (hoaDon == null)
            {
                TempData["ToastError"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem đơn này.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new OrderSuccessVM
            {
                MaHd = hoaDon.MaHd,
                NgayDat = hoaDon.NgayDat,
                TrangThai = hoaDon.MaTrangThaiNavigation?.TenTrangThai ?? "Đang xử lý",

                HoTen = hoaDon.HoTen ?? "",
                DienThoai = hoaDon.MaKhNavigation?.DienThoai ?? "", // nếu KhachHang có field này
                DiaChi = hoaDon.DiaChi,

                CachVanChuyen = hoaDon.CachVanChuyen,
                CachThanhToan = hoaDon.CachThanhToan,

                PhiVanChuyen = hoaDon.PhiVanChuyen,
                GiamGia = 0,

                Items = hoaDon.ChiTietHds.Select(ct => new OrderSuccessItemVM
                {
                    MaHh = ct.MaHh,
                    TenHh = ct.MaHhNavigation?.TenHh ?? $"SP #{ct.MaHh}",
                    Hinh = ct.MaHhNavigation?.Hinh ?? "",
                    DonGia = ct.DonGia,
                    SoLuong = ct.SoLuong
                }).ToList()
            };

            vm.TongTienHang = vm.Items.Sum(x => x.ThanhTien);

            return View(vm);
        }
    }
}

