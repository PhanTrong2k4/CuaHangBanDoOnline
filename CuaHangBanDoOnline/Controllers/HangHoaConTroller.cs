using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Index()
        {
            var hangHoas = _repository.GetHangHoas();
            ApplyDiscounts(hangHoas);
            return View(hangHoas);
        }
        public IActionResult Detail(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();
            ApplyDiscounts(new List<HangHoa> { hangHoa });
            return View(hangHoa);
        }

        public IActionResult ViewSanPham(string search, string category, string price_range, string sort_by, int page = 1)
        {
            var hangHoas = _repository.GetHangHoas()
                .Where(hh => hh.TenHangHoa != "Đã xóa"); // Đảm bảo lọc bỏ các sản phẩm đã soft delete

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                hangHoas = hangHoas.Where(hh => hh.TenHangHoa.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                (hh.MoTa != null && hh.MoTa.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(category) && int.TryParse(category, out int categoryId))
            {
                hangHoas = hangHoas.Where(hh => hh.HangHoaDanhMucs.Any(hdm => hdm.MaDanhMuc == categoryId)).ToList();
            }

            // Lọc theo mức giá
            if (!string.IsNullOrEmpty(price_range))
            {
                var range = price_range.Split('-');
                if (range.Length == 2 && decimal.TryParse(range[0], out decimal minPrice) && decimal.TryParse(range[1], out decimal maxPrice))
                {
                    hangHoas = hangHoas.Where(hh => hh.GiaBan >= minPrice && hh.GiaBan <= maxPrice).ToList();
                }
            }

            // Sắp xếp
            if (!string.IsNullOrEmpty(sort_by))
            {
                switch (sort_by.ToLower())
                {
                    case "price_asc":
                        hangHoas = hangHoas.OrderBy(hh => hh.GiaBan).ToList();
                        break;
                    case "price_desc":
                        hangHoas = hangHoas.OrderByDescending(hh => hh.GiaBan).ToList();
                        break;
                    case "name_asc":
                        hangHoas = hangHoas.OrderBy(hh => hh.TenHangHoa).ToList();
                        break;
                    case "name_desc":
                        hangHoas = hangHoas.OrderByDescending(hh => hh.TenHangHoa).ToList();
                        break;
                }
            }

            // Áp dụng giảm giá
            ApplyDiscounts(hangHoas);

            // Phân trang
            const int pageSize = 12;
            var pagedHangHoas = hangHoas.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling((double)hangHoas.Count() / pageSize);
            ViewBag.CurrentPage = page;

            // Truyền danh sách danh mục vào ViewBag để hiển thị trong bộ lọc
            ViewBag.DanhMucs = _repository.GetDanhMucs();

            // Lưu các giá trị bộ lọc để giữ trạng thái khi tải lại trang
            ViewData["SearchQuery"] = search;
            ViewData["SelectedCategory"] = category;
            ViewData["SelectedPriceRange"] = price_range;
            ViewData["SelectedSortBy"] = sort_by;

            return View(pagedHangHoas);
        }
        [Authorize(Roles = "Admin,Staff,Customer")]
        public IActionResult Search(string query)
        {
            return RedirectToAction("ViewSanPham", new { search = query });
        }
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            ViewBag.DanhMucs = new MultiSelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(new HangHoa());
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create(HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (ModelState.IsValid)
            {
                List<int> danhMucIds = Request.Form["DanhMucIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(int.Parse)
                    .ToList();

                _repository.AddHangHoa(hangHoa, danhMucIds, HinhAnh);
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.DanhMucs = new MultiSelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(hangHoa);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Edit(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();
            ApplyDiscounts(new List<HangHoa> { hangHoa });

            var danhMucHienTai = hangHoa.HangHoaDanhMucs?
                .Select(hdm => (MaDanhMuc: hdm.DanhMuc.MaDanhMuc, TenDanhMuc: hdm.DanhMuc.TenDanhMuc))
                .ToList();
            ViewBag.DanhMucHienTai = danhMucHienTai ?? new List<(int MaDanhMuc, string TenDanhMuc)>();

            ViewBag.DanhMucs = new SelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");

            return View(hangHoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Themdanhmuc(int maHangHoa, int maDanhMuc)
        {
            try
            {
                if (maHangHoa <= 0 || maDanhMuc <= 0)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                var hangHoa = _repository.GetHangHoa(maHangHoa);
                if (hangHoa == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                var danhMuc = _repository.GetDanhMuc(maDanhMuc);
                if (danhMuc == null)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại." });
                }

                var existingAssociation = hangHoa.HangHoaDanhMucs?
                    .FirstOrDefault(hdm => hdm.MaDanhMuc == maDanhMuc);
                if (existingAssociation != null)
                {
                    return Json(new { success = false, message = "Sản phẩm đã thuộc danh mục này." });
                }

                _repository.ThemDanhMuc(maHangHoa, maDanhMuc);

                return Json(new { success = true, tenDanhMuc = danhMuc.TenDanhMuc, message = "Thêm danh mục thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi thêm danh mục: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult XoaDanhMuc(int maHangHoa, int maDanhMuc)
        {
            try
            {
                if (maHangHoa <= 0 || maDanhMuc <= 0)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                var hangHoa = _repository.GetHangHoa(maHangHoa);
                if (hangHoa == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                var existingAssociation = hangHoa.HangHoaDanhMucs?
                    .FirstOrDefault(hdm => hdm.MaDanhMuc == maDanhMuc);
                if (existingAssociation == null)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại trong sản phẩm này." });
                }

                _repository.XoaDanhMuc(maHangHoa, maDanhMuc);

                return Json(new { success = true, message = "Xóa danh mục thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa danh mục: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Edit(int id, HangHoa hangHoa, IFormFile HinhAnh)
        {
            if (id != hangHoa.MaHangHoa) return BadRequest();

            if (ModelState.IsValid)
            {
                var existingHangHoa = _repository.GetHangHoa(id);
                if (existingHangHoa == null) return NotFound();

                existingHangHoa.TenHangHoa = hangHoa.TenHangHoa;
                existingHangHoa.GiaGoc = hangHoa.GiaGoc;
                existingHangHoa.GiaBan = hangHoa.GiaBan;
                existingHangHoa.MoTa = hangHoa.MoTa;
                existingHangHoa.SoLuongTon = hangHoa.SoLuongTon;

                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingHangHoa.Hinh))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHangHoa.Hinh.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        HinhAnh.CopyTo(stream);
                    }
                    existingHangHoa.Hinh = "/images/" + fileName;
                }

                _repository.UpdateHangHoa(existingHangHoa);
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            var danhMucHienTai = hangHoa.HangHoaDanhMucs?
                .Select(hdm => (MaDanhMuc: hdm.DanhMuc.MaDanhMuc, TenDanhMuc: hdm.DanhMuc.TenDanhMuc))
                .ToList();
            ViewBag.DanhMucHienTai = danhMucHienTai ?? new List<(int MaDanhMuc, string TenDanhMuc)>();
            ViewBag.DanhMucs = new SelectList(_repository.GetDanhMucs(), "MaDanhMuc", "TenDanhMuc");
            return View(hangHoa);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Delete(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }
            return View(hangHoa);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult DeleteConfirmed(int id)
        {
            var hangHoa = _repository.DeleteHangHoa(id);
            if (hangHoa != null)
            {
                TempData["Success"] = "Sản phẩm đã được đánh dấu là 'Đã xóa'.";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm để xóa.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult ThemGiamGia(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();

            ViewBag.HangHoa = hangHoa;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult ThemGiamGia(int id, decimal phanTramGiamGia, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                _repository.ThemGiamGia(id, phanTramGiamGia, ngayBatDau, ngayKetThuc);
                TempData["Success"] = "Thêm khuyến mãi thành công!";
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

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult XoaGiamGia(int id)
        {
            try
            {
                _repository.XoaGiamGia(id);
                TempData["Success"] = "Xóa khuyến mãi thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        private void ApplyDiscounts(IEnumerable<HangHoa> hangHoas)
        {
            var currentDate = DateTime.Now;

            foreach (var hangHoa in hangHoas)
            {
                decimal khuyenMaiGiamGia = 0m;
                var khuyenMai = hangHoa.KhuyenMais
                    .FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);

                if (khuyenMai != null)
                {
                    khuyenMaiGiamGia = khuyenMai.PhanTramGiamGia;
                }

                decimal danhMucGiamGia = 0m;
                if (hangHoa.HangHoaDanhMucs != null && hangHoa.HangHoaDanhMucs.Any())
                {
                    danhMucGiamGia = hangHoa.HangHoaDanhMucs
                        .Select(hdm => hdm.DanhMuc.PhanTramGiamGia)
                        .DefaultIfEmpty(0m)
                        .Max();
                }

                decimal phanTramGiamGia = Math.Max(khuyenMaiGiamGia, danhMucGiamGia);
                if (phanTramGiamGia > 0)
                {
                    hangHoa.GiaBan = hangHoa.GiaGoc * (1 - phanTramGiamGia / 100);
                    ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = phanTramGiamGia;
                    ViewData[$"NguonGiamGia_{hangHoa.MaHangHoa}"] = khuyenMaiGiamGia > danhMucGiamGia ? "KhuyenMai" : "DanhMuc";
                }
                else
                {
                    if (hangHoa.GiaBan < hangHoa.GiaGoc)
                    {
                        phanTramGiamGia = ((hangHoa.GiaGoc - hangHoa.GiaBan) / hangHoa.GiaGoc) * 100;
                        ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = phanTramGiamGia;
                        ViewData[$"NguonGiamGia_{hangHoa.MaHangHoa}"] = "Manual";
                    }
                    else
                    {
                        ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = 0m;
                        ViewData[$"NguonGiamGia_{hangHoa.MaHangHoa}"] = "None";
                    }
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public IActionResult Autocomplete(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var hangHoas = _repository.GetHangHoas()
                .Where(hh => hh.TenHangHoa != "Đã xóa"); // Lọc bỏ các sản phẩm đã soft delete

            var suggestions = hangHoas
                .Where(hh => hh.TenHangHoa.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                             (hh.MoTa != null && hh.MoTa.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(hh => new
                {
                    label = hh.TenHangHoa,
                    value = hh.TenHangHoa,
                    id = hh.MaHangHoa
                })
                .Take(10)
                .ToList();

            Console.WriteLine($"Số lượng gợi ý: {suggestions.Count}");
            return Json(suggestions);
        }
    }
}