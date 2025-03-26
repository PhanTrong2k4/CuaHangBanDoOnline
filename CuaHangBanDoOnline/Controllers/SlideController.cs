using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace CuaHangBanDoOnline.Controllers
{
    public class SlideController : Controller
    {
        private readonly CuaHangDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHangHoaRepository _hangHoaRepository;

        public SlideController(CuaHangDbContext context, IWebHostEnvironment webHostEnvironment, IHangHoaRepository hangHoaRepository)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _hangHoaRepository = hangHoaRepository;
        }

        // GET: Slide/Index - Hiển thị danh sách slides
        public IActionResult Index()
        {
            var slides = _context.Slides.ToList();
            return View(slides);
        }

        // GET: Slide/Create - Hiển thị form để tạo slide mới
        public IActionResult Create()
        {
            var hangHoas = _hangHoaRepository.GetHangHoas();
            ViewBag.HangHoas = hangHoas;
            return View();
        }

        // POST: Slide/Create - Xử lý thêm slide mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int hangHoaId, IFormFile imageFile)
        {
            // Kiểm tra nếu hangHoaId không hợp lệ
            if (hangHoaId <= 0)
            {
                TempData["Error"] = "Vui lòng chọn một sản phẩm.";
                ViewBag.HangHoas = _hangHoaRepository.GetHangHoas();
                return View(new Slide());
            }

            // Tạo một slide mới
            var slide = new Slide();

            // Lấy thông tin sản phẩm dựa trên hangHoaId
            var hangHoa = _hangHoaRepository.GetHangHoa(hangHoaId);
            if (hangHoa == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
                ViewBag.HangHoas = _hangHoaRepository.GetHangHoas();
                return View(slide);
            }

            // Điền thông tin slide từ sản phẩm
            slide.Title = hangHoa.TenHangHoa;
            slide.Link = $"/HangHoa/Detail/{hangHoa.MaHangHoa}";

            // Nếu sản phẩm có hình ảnh, sử dụng hình ảnh của sản phẩm
            if (!string.IsNullOrEmpty(hangHoa.Hinh))
            {
                slide.Image = hangHoa.Hinh;
            }
            else
            {
                // Nếu không có hình ảnh từ sản phẩm, cho phép upload hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    slide.Image = "/images/" + uniqueFileName;
                }
                else
                {
                    TempData["Error"] = "Vui lòng chọn một hình ảnh nếu sản phẩm không có hình ảnh.";
                    ViewBag.HangHoas = _hangHoaRepository.GetHangHoas();
                    return View(slide);
                }
            }

            _context.Slides.Add(slide);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm slide thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Slide/Edit/5 - Hiển thị form để chỉnh sửa slide
        public IActionResult Edit(int id)
        {
            var slide = _context.Slides.Find(id);
            if (slide == null)
            {
                return NotFound();
            }
            return View(slide);
        }

        // POST: Slide/Edit/5 - Xử lý cập nhật slide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Slide slide, IFormFile imageFile)
        {
            if (id != slide.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSlide = await _context.Slides.FindAsync(id);
                    if (existingSlide == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingSlide.Image))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingSlide.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        slide.Image = "/images/" + uniqueFileName;
                    }
                    else
                    {
                        slide.Image = existingSlide.Image;
                    }

                    existingSlide.Title = slide.Title;
                    existingSlide.Link = slide.Link;
                    existingSlide.Image = slide.Image;

                    _context.Slides.Update(existingSlide);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật slide thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Slides.Any(e => e.Id == slide.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Có lỗi xảy ra khi cập nhật slide. Vui lòng kiểm tra lại.";
            return View(slide);
        }

        // GET: Slide/Delete/5 - Xác nhận xóa slide
        public IActionResult Delete(int id)
        {
            var slide = _context.Slides.Find(id);
            if (slide == null)
            {
                return NotFound();
            }
            return View(slide);
        }

        // POST: Slide/Delete/5 - Xử lý xóa slide
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(slide.Image))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, slide.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa slide thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}