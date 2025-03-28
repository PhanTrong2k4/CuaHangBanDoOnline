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

            // Chuẩn bị danh sách chi tiết đơn hàng
            var chiTietDonHangs = donHang.ChiTietDonHangs.Select(ct => (ct.MaHangHoa, ct.GiaBan, ct.SoLuong, ct.HangHoa.TenHangHoa)).ToList();

            // Tạo Key Game cho từng sản phẩm
            var thanhToans = _thanhToanRepository.ThanhToanDonHangWithKeyGames(donHang.MaDonHang, chiTietDonHangs.Select(ct => (ct.MaHangHoa, ct.GiaBan, ct.SoLuong)).ToList(), "ChuyenKhoan");
            if (thanhToans == null)
            {
                TempData["Error"] = "Không thể duyệt đơn hàng.";
                return RedirectToAction("ManageOrders");
            }

            // Nhóm Key Game theo tên sản phẩm
            var keyGamesByProduct = new Dictionary<string, List<string>>();
            foreach (var chiTiet in chiTietDonHangs)
            {
                var keyGamesForProduct = thanhToans
                    .Where(t => t.MaDonHang == donHang.MaDonHang && t.SoTien == chiTiet.GiaBan) // Giả định SoTien khớp với GiaBan của sản phẩm
                    .Take(chiTiet.SoLuong) // Lấy đúng số lượng Key Game
                    .Select(t => t.KeyGame)
                    .ToList();
                keyGamesByProduct[chiTiet.TenHangHoa] = keyGamesForProduct;
            }

            // Tạo nội dung email với Key Game nhóm theo sản phẩm
            var keyGameContent = "";
            foreach (var product in keyGamesByProduct)
            {
                keyGameContent += $"<p><strong>{product.Key}:</strong></p>";
                keyGameContent += string.Join("<br>", product.Value.Select(k => $"- {k}"));
                keyGameContent += "<br>";
            }

            // Gửi email thông báo
            var user = _userRepository.GetUserById(donHang.MaNguoiDung);
            var emailContent = $@"
        <h2>Đơn hàng #{donHang.MaDonHang} đã được duyệt</h2>
        <p>Cảm ơn bạn đã mua hàng! Dưới đây là chi tiết đơn hàng:</p>
        <p><strong>Tổng tiền:</strong> {donHang.TongTien.ToString("#,##0")} VNĐ</p>
        <p><strong>Danh sách Key Game:</strong></p>
        {keyGameContent}
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