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
                    
                    hangHoa.GiaBan = hangHoa.GiaGoc * (1 - khuyenMai.PhanTramGiamGia / 100);
                    ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = khuyenMai.PhanTramGiamGia;
                }
                else
                {
                   
                    if (hangHoa.GiaBan < hangHoa.GiaGoc)
                    {
                        
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

            
            var currentDate = DateTime.Now;
            var khuyenMai = hangHoa.KhuyenMais
                ?.FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);

            if (khuyenMai != null)
            {
                
                hangHoa.GiaBan = hangHoa.GiaGoc * (1 - khuyenMai.PhanTramGiamGia / 100);
                ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = khuyenMai.PhanTramGiamGia;
            }
            else
            {
                
                if (hangHoa.GiaBan < hangHoa.GiaGoc)
                {
                    
                    decimal phanTramGiamGia = ((hangHoa.GiaGoc - hangHoa.GiaBan) / hangHoa.GiaGoc) * 100;
                    ViewData[$"PhanTramGiamGia_{hangHoa.MaHangHoa}"] = phanTramGiamGia;
                }
                else
                {
                    
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
                return RedirectToAction("Index");
            }

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

        [HttpGet]
        public IActionResult ThemGiamGia(int id)
        {
            var hangHoa = _repository.GetHangHoa(id);
            if (hangHoa == null) return NotFound();

            ViewBag.HangHoa = hangHoa;
            return View();
        }
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