using EcommerceMVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;


namespace EcommerceMVC.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [Authorize(Roles = "Admin")]
    public class HomeAdminController : Controller
    {
        private readonly Hshop2023Context db;

        public HomeAdminController(Hshop2023Context context)
        {
            db = context;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Danhmucsanpham")]
        //public IActionResult DanhMucSanPham()
        //{
        //	var lstSanPham = db.HangHoas.ToList();
        //	return View(lstSanPham);

        //}

        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstsanpham = db.HangHoas.AsNoTracking().OrderBy(x => x.MaHh);
            IPagedList<HangHoa> lst = new PagedList<HangHoa>(lstsanpham, pageNumber, pageSize);
            return View(lst);

        }
        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            var danhSachLoai = db.Loais.ToList();
            var danhSachNCC = db.NhaCungCaps.ToList();

            // Kiểm tra nếu không có dữ liệu
            if (!danhSachLoai.Any() || !danhSachNCC.Any())
            {
                // Trả về một lỗi hoặc thông báo cho người dùng
                ViewBag.ErrorMessage = "Danh sách loại hoặc nhà cung cấp không có dữ liệu.";
            }
            else
            {
                ViewBag.MaLoai = new SelectList(danhSachLoai, "MaLoai", "TenLoai");
                ViewBag.MaNCC = new SelectList(danhSachNCC, "MaNcc", "TenCongTy");
            }
            return View();
        }
        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ThemSanPhamMoi(HangHoa sanPham, IFormFile HinhUpload)
        {
            if (ModelState.IsValid)
            {
                if (HinhUpload != null)
                {
                    var fileName = Path.GetFileName(HinhUpload.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Hinh/HangHoa", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await HinhUpload.CopyToAsync(stream);
                    }

                    sanPham.Hinh = fileName; // lưu tên ảnh vào DB
                }

                db.HangHoas.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }

            return View(sanPham);
        }

        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int maSanPham)  // Thay đổi tham số sang kiểu int
        {
            // Tìm sản phẩm theo mã số
            var sanPham = db.HangHoas.Find(maSanPham);  // Không cần kiểm tra kiểu dữ liệu vì đã là int
            if (sanPham == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            ViewBag.MaLoai = new SelectList(db.Loais.AsNoTracking().ToList(), "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.AsNoTracking().ToList(), "MaNcc", "TenCongTy", sanPham.MaNcc);

            return View(sanPham);
        }

        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaSanPham(HangHoa sanPham, IFormFile? HinhUpload)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaLoai = new SelectList(db.Loais.AsNoTracking().ToList(), "MaLoai", "TenLoai", sanPham.MaLoai);
                ViewBag.MaNCC = new SelectList(db.NhaCungCaps.AsNoTracking().ToList(), "MaNcc", "TenCongTy", sanPham.MaNcc);
                return View(sanPham);
            }

            var existing = await db.HangHoas.FirstOrDefaultAsync(x => x.MaHh == sanPham.MaHh);
            if (existing == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            // Upload hình mới (nếu có)
            if (HinhUpload is { Length: > 0 })
            {
                var fileName = Path.GetFileName(HinhUpload.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa");
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, fileName);

                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await HinhUpload.CopyToAsync(stream);
                }

                existing.Hinh = fileName;
            }

            // Update các field
            existing.TenHh = sanPham.TenHh;
            existing.TenAlias = sanPham.TenAlias;
            existing.MaLoai = sanPham.MaLoai;
            existing.MoTaDonVi = sanPham.MoTaDonVi;
            existing.DonGia = sanPham.DonGia;
            existing.NgaySx = sanPham.NgaySx;
            existing.GiamGia = sanPham.GiamGia;
            existing.SoLanXem = sanPham.SoLanXem;
            existing.MoTa = sanPham.MoTa;
            existing.MaNcc = sanPham.MaNcc;

            await db.SaveChangesAsync();
            return RedirectToAction("DanhMucSanPham");
        }
        
        [Route("XoaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]  // Đảm bảo bảo mật CSRF
        public IActionResult XoaSanPham(int maSanPham)
        {
            // Tìm sản phẩm theo mã sản phẩm (maSanPham)
            var hangHoa = db.HangHoas.Find(maSanPham);
            if (hangHoa == null)
            {
                // Nếu không tìm thấy sản phẩm, trả về lỗi
                return NotFound("Không tìm thấy sản phẩm cần xóa.");
            }

            // Xóa sản phẩm khỏi cơ sở dữ liệu
            db.HangHoas.Remove(hangHoa);
            db.SaveChanges();

            // Sau khi xóa, chuyển hướng về danh mục sản phẩm
            return RedirectToAction("DanhMucSanPham");
        }


    }
}
