using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using CuaHangBanDoOnline.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace CuaHangBanDoOnline.Controllers
{
    public class DonHangController : Controller
    {
        private readonly IDonHangRepository _donHangRepository;
        private readonly IThanhToanRepository _thanhToanRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public DonHangController(
            IDonHangRepository donHangRepository,
            IThanhToanRepository thanhToanRepository,
            IUserRepository userRepository,
            EmailService emailService)
        {
            _donHangRepository = donHangRepository;
            _thanhToanRepository = thanhToanRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var donHangs = _donHangRepository.GetDonHangsByUserId(user.MaNguoiDung);
            return View(donHangs);
        }

        public IActionResult Details(int id)
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var donHang = _donHangRepository.GetDonHang(id);
            if (donHang == null || donHang.MaNguoiDung != user.MaNguoiDung)
                return NotFound();

            return View(donHang);
        }
        // ... Các action hiện có (Index, Details, ThanhToanChiTiet) ...

        [Authorize(Roles = "Staff,Admin")]
        public IActionResult ManageOrders(string searchMaDonHang = null)
        {
            var donHangs = _donHangRepository.GetAllDonHangs();
            if (!string.IsNullOrEmpty(searchMaDonHang))
            {
                donHangs = donHangs.Where(dh => dh.MaDonHang.ToString().Contains(searchMaDonHang));
            }
            return View(donHangs);
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpPost]
        public async Task<IActionResult> DuyetDonHang(int id)
        {
            var donHang = _donHangRepository.GetDonHang(id);
            if (donHang == null || donHang.TrangThai != "ChoDuyet")
                return NotFound();

            // Tạo Key Game tự động (API Key)
            string keyGame = GenerateApiKey(); // Hàm tự động tạo Key Game

            // Cập nhật trạng thái và lưu Key Game
            donHang.TrangThai = "DaThanhToan";
            var thanhToan = new ThanhToan
            {
                MaDonHang = donHang.MaDonHang,
                SoTien = donHang.TongTien,
                PhuongThucThanhToan = "ChuyenKhoan", // Hoặc QRCode tùy bạn
                NgayThanhToan = DateTime.Now,
                KeyGame = keyGame
            };
            _thanhToanRepository.AddThanhToan(thanhToan);

            // Gửi email thông báo
            var user = _userRepository.GetUserById(donHang.MaNguoiDung);
            var emailContent = $@"
                <h2>Đơn hàng #{donHang.MaDonHang} đã được duyệt</h2>
                <p>Cảm ơn bạn đã mua hàng! Dưới đây là chi tiết đơn hàng:</p>
                <p><strong>Tổng tiền:</strong> {donHang.TongTien.ToString("#,##0")} VNĐ</p>
                <p><strong>Key Game:</strong> {keyGame}</p>
                <p>Vui lòng kiểm tra Key Game trong trang chi tiết đơn hàng của bạn.</p>";
            await _emailService.SendEmailAsync(user.Email, "Đơn hàng đã được duyệt", emailContent);

            TempData["Success"] = "Đơn hàng đã được duyệt thành công!";
            return RedirectToAction("ManageOrders");
        }

        // Hàm tạo API Key tự động
        private string GenerateApiKey()
        {
            // Tạo chuỗi ngẫu nhiên dạng API Key (GUID + tiền tố tùy chỉnh)
            string prefix = "TGAME-";
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
            return $"{prefix}{uniqueId}";
            // Ví dụ: TGAME-4A7B9C2D1E3F5G6H
        }
        [HttpPost]
        public IActionResult ThanhToanChiTiet(int id, string paymentMethod)
        {
            var donHang = _donHangRepository.GetDonHang(id);
            if (donHang == null || donHang.TrangThai != "ChoDuyet")
                return NotFound();

            ThanhToan thanhToan = null;
            if (paymentMethod == "QRCode")
            {
                thanhToan = _thanhToanRepository.ThanhToanDonHang(id, "QRCode", donHang.TongTien);
                TempData["Success"] = "Vui lòng quét mã QR để thanh toán. Đơn hàng sẽ được duyệt sau khi xác nhận.";
            }
            else if (paymentMethod == "ChuyenKhoan")
            {
                thanhToan = _thanhToanRepository.ThanhToanDonHang(id, "ChuyenKhoan", donHang.TongTien);
                TempData["Success"] = "Vui lòng chuyển khoản theo thông tin ngân hàng. Đơn hàng sẽ được duyệt sau khi xác nhận.";
            }

            if (thanhToan == null)
            {
                TempData["Error"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("Details", new { id });
            }

            return RedirectToAction("Details", new { id });
        }
    }

}