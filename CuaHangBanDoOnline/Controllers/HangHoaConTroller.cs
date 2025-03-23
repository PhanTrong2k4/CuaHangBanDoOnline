using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly IHangHoaRepository _repository;

        public HangHoaController(IHangHoaRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var hangHoas = _repository.GetHangHoas();
            var currentDate = DateTime.Now;

            foreach (var hangHoa in hangHoas)
            {
                var khuyenMai = hangHoa.KhuyenMais
                    .FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);

                if (khuyenMai != null)
                {
                    // Tính giá đã giảm dựa trên khuyến mãi
                    hangHoa.GiaBan = hangHoa.GiaGoc * (1 - khuyenMai.PhanTramGiamGia / 100);
                    ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = khuyenMai.PhanTramGiamGia;
                }
                else
                {
                    // Giữ nguyên GiaBan từ cơ sở dữ liệu, không đặt lại bằng GiaGoc
                    if (hangHoa.GiaBan < hangHoa.GiaGoc)
                    {
                        // Tính phần trăm giảm giá dựa trên GiaBan và GiaGoc
                        decimal phanTramGiamGia = ((hangHoa.GiaGoc - hangHoa.GiaBan) / hangHoa.GiaGoc) * 100;
                        ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = phanTramGiamGia;
                    }
                    else
                    {
                        ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = 0m;
                    }
                }
            }

            return View(hangHoas);
        }

        public IActionResult Detail(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();

            // Tính giá đã giảm và phần trăm giảm giá
            var currentDate = DateTime.Now;
            var khuyenMai = hangHoa.KhuyenMais
                ?.FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);

            if (khuyenMai != null)
            {
                // Có khuyến mãi hợp lệ, tính giá đã giảm dựa trên khuyến mãi
                hangHoa.GiaBan = hangHoa.GiaGoc * (1 - khuyenMai.PhanTramGiamGia / 100);
                ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = khuyenMai.PhanTramGiamGia;
            }
            else
            {
                // Không có khuyến mãi hợp lệ, kiểm tra xem GiaBan có nhỏ hơn GiaGoc không
                if (hangHoa.GiaBan < hangHoa.GiaGoc)
                {
                    // Nếu GiaBan nhỏ hơn GiaGoc, tính phần trăm giảm giá dựa trên GiaBan và GiaGoc
                    decimal phanTramGiamGia = ((hangHoa.GiaGoc - hangHoa.GiaBan) / hangHoa.GiaGoc) * 100;
                    ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = phanTramGiamGia;
                }
                else
                {
                    // Nếu không có giảm giá, đặt phần trăm giảm giá là 0
                    ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = 0m;
                }
            }

            return View(hangHoa);
        }

        public IActionResult Create()
        {
            ViewBag.DanhMucs = new MultiSelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(new HangHoa());
        }

        [HttpPost]
        public IActionResult Create(HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (ModelState.IsValid)
            {
                List<int> danhMucIds = Request.Form["DanhMucIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(int.Parse)
                    .ToList();

                _repository.AddHangHoa(hangHoa, danhMucIds, HinhAnh);
                return RedirectToAction("Index");
            }
            ViewBag.DanhMucs = new MultiSelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(hangHoa);
        }

        public IActionResult Edit(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();

            // Lấy danh sách tên danh mục hiện tại của sản phẩm
            var danhMucHienTai = hangHoa.HangHoaDanhMucs?
                .Select(hdm => hdm.DanhMuc.TenDanhMuc)
                .ToList();

            ViewBag.DanhMucHienTai = danhMucHienTai ?? new List<string>();
            ViewBag.DanhMucs = new SelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");

            return View(hangHoa);
        }

        [HttpPost]
        public IActionResult Themdanhmuc(int maHangHoa, int maDanhMuc)
        {
            try
            {
                _repository.ThemDanhMuc(maHangHoa, maDanhMuc);
                return RedirectToAction("Edit", new { id = maHangHoa });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Edit", new { id = maHangHoa });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (id != hangHoa.MaHangHoa) return BadRequest();

            if (ModelState.IsValid)
            {
                // Lấy thông tin hàng hóa hiện tại từ repository
                var existingHangHoa = _repository.GetHangHoa(id);
                if (existingHangHoa == null) return NotFound();

                // Cập nhật các thuộc tính từ form
                existingHangHoa.TenHangHoa = hangHoa.TenHangHoa;
                existingHangHoa.GiaGoc = hangHoa.GiaGoc;
                existingHangHoa.GiaBan = hangHoa.GiaBan; // Giá bán có thể đã được cập nhật từ giảm giá
                existingHangHoa.MoTa = hangHoa.MoTa;
                existingHangHoa.SoLuongTon = hangHoa.SoLuongTon;

                // Xử lý hình ảnh mới nếu có
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(existingHangHoa.Hinh))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHangHoa.Hinh.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lưu ảnh mới
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        HinhAnh.CopyTo(stream);
                    }
                    existingHangHoa.Hinh = "/images/" + fileName;
                }

                _repository.UpdateHangHoa(existingHangHoa);
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, trả lại View với dữ liệu cần thiết
            var danhMucHienTai = hangHoa.HangHoaDanhMucs?
                .Select(hdm => hdm.DanhMuc.TenDanhMuc)
                .ToList();
            ViewBag.DanhMucHienTai = danhMucHienTai ?? new List<string>();
            ViewBag.DanhMucs = new SelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(hangHoa);
        }

        public IActionResult Delete(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();
            return View(hangHoa);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteHangHoa(id);
            return RedirectToAction("Index");
        }

        // Action để hiển thị form thêm giảm giá
        [HttpGet]
        public IActionResult ThemGiamGia(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();

            ViewBag.HangHoa = hangHoa;
            return View();
        }

        // Action để xử lý thêm giảm giá
        [HttpPost]
        public IActionResult ThemGiamGia(int id, decimal phanTramGiamGia, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                _repository.ThemGiamGia(id, phanTramGiamGia, ngayBatDau, ngayKetThuc);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var hangHoa = _repository.GetHangHoa(id);
                ViewBag.HangHoa = hangHoa;
                return View();
            }
        }

        // Action để xóa giảm giá
        [HttpPost]
        public IActionResult XoaGiamGia(int id)
        {
            try
            {
                _repository.XoaGiamGia(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}