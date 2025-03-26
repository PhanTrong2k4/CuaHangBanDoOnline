using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuaHangBanDoOnline.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly IDanhMucRepository _danhMucRepository;
        private readonly IHangHoaRepository _hangHoaRepository;

        public DanhMucController(IDanhMucRepository danhMucRepository, IHangHoaRepository hangHoaRepository)
        {
            _danhMucRepository = danhMucRepository;
            _hangHoaRepository = hangHoaRepository;
        }

        public IActionResult Index()
        {
            try
            {
                var danhMucs = _danhMucRepository.GetDanhMucs();

                if (danhMucs == null)
                {
                    TempData["Error"] = "Danh sách danh mục là null";
                    return View(new List<DanhMuc>());
                }

                if (!danhMucs.Any())
                {
                    TempData["Error"] = "Danh sách danh mục rỗng (không có phần tử)";
                    return View(danhMucs);
                }

                foreach (var danhMuc in danhMucs)
                {
                    if (danhMuc.HangHoaDanhMucs == null)
                    {
                        danhMuc.HangHoaDanhMucs = new List<HangHoaDanhMuc>();
                    }
                }

                return View(danhMucs);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return View(new List<DanhMuc>());
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var danhMuc = _danhMucRepository.GetDanhMuc(id);
                if (danhMuc == null || danhMuc.TenDanhMuc == "Đã xóa")
                {
                    return NotFound();
                }

                return View("ChiTietDanhMuc", danhMuc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult SanPhamTheoDanhMuc(int id, string search, string priceRange, string sortBy, int page = 1)
        {
            try
            {
                var danhMuc = _danhMucRepository.GetDanhMuc(id);
                if (danhMuc == null || danhMuc.TenDanhMuc == "Đã xóa")
                {
                    return NotFound();
                }

                if (danhMuc.HangHoaDanhMucs == null)
                {
                    danhMuc.HangHoaDanhMucs = new List<HangHoaDanhMuc>();
                }

                var filteredHangHoas = _hangHoaRepository.GetHangHoasFiltered(search, null, priceRange, null, null, sortBy)
                    .Where(hh => hh.HangHoaDanhMucs.Any(hdm => hdm.MaDanhMuc == id))
                    .Select(hh => new HangHoaDanhMuc { HangHoa = hh })
                    .ToList();

                var currentDate = DateTime.Now;
                foreach (var hangHoaDanhMuc in filteredHangHoas)
                {
                    var hangHoa = hangHoaDanhMuc.HangHoa;

                    decimal khuyenMaiGiamGia = 0m;
                    var khuyenMai = hangHoa.KhuyenMais
                        ?.FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);

                    if (khuyenMai != null)
                    {
                        khuyenMaiGiamGia = khuyenMai.PhanTramGiamGia;
                    }

                    decimal danhMucGiamGia = danhMuc.PhanTramGiamGia;
                    decimal phanTramGiamGia = Math.Max(khuyenMaiGiamGia, danhMucGiamGia);
                    if (phanTramGiamGia > 0)
                    {
                        hangHoa.GiaBan = hangHoa.GiaGoc * (1 - phanTramGiamGia / 100);
                        ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = phanTramGiamGia;
                        ViewData[$"NguonGiamGia_{hangHoa.MaHangHoa}"] = khuyenMaiGiamGia > danhMucGiamGia ? "KhuyenMai" : "DanhMuc";
                    }
                    else
                    {
                        hangHoa.GiaBan = hangHoa.GiaGoc;
                        ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = 0m;
                        ViewData[$"NguonGiamGia_{hangHoa.MaHangHoa}"] = "None";
                    }
                }

                const int pageSize = 12;
                var pagedHangHoas = filteredHangHoas.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                danhMuc.HangHoaDanhMucs = pagedHangHoas;
                ViewBag.TotalPages = (int)Math.Ceiling((double)filteredHangHoas.Count / pageSize);
                ViewBag.CurrentPage = page;

                ViewBag.DanhMucs = _danhMucRepository.GetDanhMucs();
                ViewBag.Search = search;
                ViewBag.PriceRange = priceRange;
                ViewBag.SortBy = sortBy;

                return View("Details", danhMuc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _danhMucRepository.AddDanhMuc(danhMuc);
                    TempData["Success"] = "Thêm danh mục thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi thêm danh mục: {ex.Message}");
                }
            }
            return View(danhMuc);
        }

        public IActionResult Edit(int id)
        {
            var danhMuc = _danhMucRepository.GetDanhMuc(id);
            if (danhMuc == null || danhMuc.TenDanhMuc == "Đã xóa")
            {
                TempData["Error"] = "Không tìm thấy danh mục.";
                return RedirectToAction("Index");
            }
            return View(danhMuc);
        }

        [HttpPost]
        public IActionResult Edit(int id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDanhMuc)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _danhMucRepository.UpdateDanhMuc(danhMuc);
                    TempData["Success"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật danh mục: {ex.Message}");
                }
            }
            return View(danhMuc);
        }

        public IActionResult Delete(int id)
        {
            var danhMuc = _danhMucRepository.GetDanhMuc(id);
            if (danhMuc == null || danhMuc.TenDanhMuc == "Đã xóa")
            {
                TempData["Error"] = "Không tìm thấy danh mục.";
                return RedirectToAction("Index");
            }
            return View(danhMuc);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var danhMuc = _danhMucRepository.DeleteDanhMuc(id);
                if (danhMuc != null)
                {
                    TempData["Success"] = "Danh mục đã được đánh dấu là 'Đã xóa'.";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy danh mục để xóa.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa danh mục: {ex.Message}";
            }
            return RedirectToAction("Index");
        }
    }
}