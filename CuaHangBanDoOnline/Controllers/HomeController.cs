using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using CuaHangBanDoOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuaHangBanDoOnline.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHangHoaRepository _hangHoaRepository;
        private readonly IDanhMucRepository _danhMucRepository;
        private readonly IChitietdonhangRepository _chiTietDonHangRepository;
        private readonly ISlideRepository _slideRepository;
        private readonly EmailService _emailService; // Thêm EmailService
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IHangHoaRepository hangHoaRepository,
            IDanhMucRepository danhMucRepository,
            IChitietdonhangRepository chiTietDonHangRepository,
            ISlideRepository slideRepository,
            EmailService emailService, // Thêm dependency injection
            ILogger<HomeController> logger,
            IWishlistRepository wishlistRepository,
            IGioHangRepository gioHangRepository,
            IMemoryCache memoryCache)
            : base(wishlistRepository, gioHangRepository, memoryCache)
        {
            _hangHoaRepository = hangHoaRepository;
            _danhMucRepository = danhMucRepository;
            _chiTietDonHangRepository = chiTietDonHangRepository;
            _slideRepository = slideRepository;
            _emailService = emailService; // Khởi tạo
            _logger = logger;
        }

        public IActionResult Index(string search, string category, string priceRange, string sortBy, int page = 1)
        {
            try
            {
                // Lấy danh sách slides từ repository
                var slides = _slideRepository.GetSlides();
                ViewBag.Slides = slides;

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

                var topSellingProducts = _chiTietDonHangRepository.GetAllChiTietDonHangs()
                    .GroupBy(ct => ct.HangHoa)
                    .Select(g => new
                    {
                        HangHoa = g.Key,
                        TotalSold = g.Sum(ct => ct.SoLuong)
                    })
                    .OrderByDescending(x => x.TotalSold)
                    .Take(4)
                    .Select(x => x.HangHoa)
                    .ToList();

                ViewBag.TopSellingProducts = topSellingProducts;

                var filteredHangHoas = _hangHoaRepository.GetHangHoasFiltered(search, category, priceRange, null, null, sortBy).ToList();

                var currentDate = DateTime.Now;
                foreach (var hangHoa in filteredHangHoas)
                {
                    decimal khuyenMaiGiamGia = 0m;
                    var khuyenMai = hangHoa.KhuyenMais
                        ?.FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);

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

                const int pageSize = 12;
                var pagedHangHoas = filteredHangHoas.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.FilteredHangHoas = pagedHangHoas;
                ViewBag.TotalPages = (int)Math.Ceiling((double)filteredHangHoas.Count / pageSize);
                ViewBag.CurrentPage = page;

                ViewBag.Search = search;
                ViewBag.SelectedCategory = category;
                ViewBag.PriceRange = priceRange;
                ViewBag.SortBy = sortBy;

                return View(danhMucs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong action Index");
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return View(new List<DanhMuc>());
            }
        }

        public IActionResult SanPhamTheoDanhMuc(int id, string search, string priceRange, string sortBy, int page = 1)
        {
            try
            {
                var danhMuc = _danhMucRepository.GetDanhMuc(id);
                if (danhMuc == null)
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
                return View("Details", new DanhMuc { MaDanhMuc = id, TenDanhMuc = "Danh mục không xác định", HangHoaDanhMucs = new List<HangHoaDanhMuc>() });
            }
        }

        // Action GET: Hiển thị form liên hệ
        public IActionResult Contact()
        {
            return View();
        }

        // Action POST: Xử lý form liên hệ và gửi email
        [HttpPost]
        public async Task<IActionResult> Contact(string name, string email, string subject, string message)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin." });
                }

                // Tạo nội dung email
                var emailBody = new StringBuilder();
                emailBody.AppendLine("<h2>Thông tin liên hệ từ khách hàng</h2>");
                emailBody.AppendLine($"<p><strong>Tên:</strong> {name}</p>");
                emailBody.AppendLine($"<p><strong>Email:</strong> {email}</p>");
                emailBody.AppendLine($"<p><strong>Chủ đề:</strong> {subject}</p>");
                emailBody.AppendLine($"<p><strong>Nội dung:</strong></p>");
                emailBody.AppendLine($"<p>{message}</p>");

                // Gửi email đến tinchumvip123@gmail.com
                await _emailService.SendEmailAsync("tinchumvip123@gmail.com", $"Liên hệ từ khách hàng: {subject}", emailBody.ToString());

                return Json(new { success = true, message = "Tin nhắn của bạn đã được gửi thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email liên hệ");
                return Json(new { success = false, message = $"Lỗi khi gửi tin nhắn: {ex.Message}" });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}