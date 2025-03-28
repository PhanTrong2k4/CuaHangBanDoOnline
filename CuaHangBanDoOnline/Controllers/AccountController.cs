using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using CuaHangBanDoOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _environment;

        public AccountController(IUserRepository userRepository, IConfiguration configuration, EmailService emailService, IWebHostEnvironment environment, CuaHangDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _environment = environment;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string LoginInput, string Password)
        {
            if (string.IsNullOrEmpty(LoginInput) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Vui lòng nhập email/tên đăng nhập và mật khẩu.";
                return View();
            }

            var user = _userRepository.GetUserByEmailOrUsername(LoginInput);
            if (user == null)
            {
                ViewBag.Error = "Sai email/tên đăng nhập hoặc mật khẩu.";
                return View();
            }

            bool isPasswordValid;
            if (user.MatKhau.StartsWith("$2")) // Kiểm tra xem có phải hash BCrypt
            {
                try
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.MatKhau);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    ViewBag.Error = "Lỗi định dạng mật khẩu trong hệ thống.";
                    return View();
                }
            }
            else
            {
                // Nếu là văn bản thô, so sánh trực tiếp và mã hóa lại
                isPasswordValid = user.MatKhau == Password;
                if (isPasswordValid)
                {
                    user.MatKhau = BCrypt.Net.BCrypt.HashPassword(Password);
                    _userRepository.UpdateUser(user);
                }
            }

            if (!isPasswordValid)
            {
                ViewBag.Error = "Sai email/tên đăng nhập hoặc mật khẩu.";
                return View();
            }

            if (!user.IsEmailConfirmed)
            {
                ViewBag.Error = "Tài khoản chưa xác nhận email.";
                return View();
            }

            var token = GenerateJwtToken(user);
            Response.Cookies.Append("JWToken", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string TenDangNhap, string Email, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp.";
                ViewBag.Message = null;
                return View();
            }

            if (_userRepository.UserExists(TenDangNhap))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                ViewBag.Message = null;
                return View();
            }

            if (_userRepository.EmailExists(Email))
            {
                ViewBag.Error = "Email đã được sử dụng.";
                ViewBag.Message = null;
                return View();
            }

            var user = new User
            {
                TenDangNhap = TenDangNhap,
                Email = Email,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(Password),
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };

            _userRepository.AddUser(user);
            await _emailService.SendConfirmationEmail(user.Email, user.EmailConfirmationToken);

            ViewBag.Message = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận.";
            ViewBag.Error = null;
            return View();
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        public IActionResult ConfirmEmail(string token)
        {
            var user = _userRepository.GetUserByConfirmationToken(token);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ.";
                return View();
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            _userRepository.UpdateUser(user);

            ViewBag.Message = "Xác nhận email thành công! Bạn có thể đăng nhập.";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWToken");
            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _userRepository.GetUserByEmailOrUsername(email);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại.";
                return View();
            }

            var resetToken = Guid.NewGuid().ToString();
            _userRepository.AddPasswordResetToken(user.MaNguoiDung, resetToken, DateTime.UtcNow.AddHours(1));

            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Scheme);
            await _emailService.SendResetPasswordEmail(user.Email, resetLink);

            ViewBag.Message = "Đã gửi link đặt lại mật khẩu qua email. Vui lòng kiểm tra.";
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var user = _userRepository.GetUserByResetToken(token);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return View();
            }
            return View(new ResetPasswordViewModel { Token = token });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp.";
                return View(model);
            }

            var user = _userRepository.GetUserByResetToken(model.Token);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return View(model);
            }

            // Mã hóa mật khẩu mới bằng BCrypt trước khi lưu
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.Password);
            _userRepository.UpdateUser(user);

            var resetToken = _userRepository.GetResetToken(model.Token);
            if (resetToken != null)
            {
                resetToken.IsUsed = true;
                _userRepository.UpdateResetToken(resetToken);
            }

            ViewBag.Message = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập.";
            return View();
        }

        // GET: /Account/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userProfile = _userRepository.GetUserProfile(user.MaNguoiDung);
            var viewModel = new UserProfileViewModel(user, userProfile);
            return View(viewModel);
        }

        // POST: /Account/EnableEdit
        [HttpPost]
        public IActionResult EnableEdit()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userProfile = _userRepository.GetUserProfile(user.MaNguoiDung);
            var viewModel = new UserProfileViewModel(user, userProfile) { IsEditing = true };
            return View("Profile", viewModel);
        }

        // POST: /Account/UpdateProfile (Chỉ giữ lại phiên bản này)

        [HttpPost]
        public IActionResult UpdateProfile(UserProfileViewModel viewModel)
        {
            // Bỏ qua lỗi validation của AvatarPath
            if (ModelState.ContainsKey("AvatarPath"))
            {
                ModelState["AvatarPath"].Errors.Clear();
                ModelState["AvatarPath"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userProfile = _userRepository.GetUserProfile(user.MaNguoiDung);

            // Cập nhật thông tin hồ sơ (không xử lý mật khẩu ở đây nữa)
            userProfile.FullName = viewModel.FullName ?? userProfile.FullName;
            userProfile.SoDienThoai = viewModel.SoDienThoai ?? userProfile.SoDienThoai;

            try
            {
                _userRepository.UpdateUserProfile(userProfile);
                ViewBag.Message = "Cập nhật thông tin thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lưu thông tin: " + ex.Message;
                viewModel.IsEditing = true;
                return View("Profile", viewModel);
            }

            var updatedUserProfile = _userRepository.GetUserProfile(user.MaNguoiDung);
            var updatedViewModel = new UserProfileViewModel(user, updatedUserProfile) { IsEditing = false };
            return View("Profile", updatedViewModel);
        }
        // POST: /Account/UploadAvatar
        [HttpPost]
        public IActionResult UploadAvatar(IFormFile AvatarFile)
        {
            if (AvatarFile == null || AvatarFile.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn một file ảnh." });
            }

            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Phiên đăng nhập hết hạn." });
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }

            var userProfile = _userRepository.GetUserProfile(user.MaNguoiDung);

            // Kiểm tra kích thước file
            if (AvatarFile.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "Ảnh không được lớn hơn 5MB." });
            }

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/users");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(userProfile.AvatarPath) && userProfile.AvatarPath != "/images/usres/usericon.png")
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, userProfile.AvatarPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Lưu ảnh mới
                var uniqueFileName = $"{user.MaNguoiDung}_{Guid.NewGuid().ToString()}{Path.GetExtension(AvatarFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(fileStream);
                }

                // Kiểm tra xem file đã được lưu thành công chưa
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new { success = false, message = "Lỗi: Không thể lưu file ảnh." });
                }

                userProfile.AvatarPath = $"/images/users/{uniqueFileName}";
                _userRepository.UpdateUserProfile(userProfile);

                return Json(new { success = true, avatarPath = userProfile.AvatarPath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lưu ảnh: " + ex.Message });
            }
        }

        // POST: /Account/ChangeAvatar (Có thể xóa nếu không dùng)
        [HttpPost]
        public IActionResult ChangeAvatar(IFormFile AvatarFile)
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userProfile = _userRepository.GetUserProfile(user.MaNguoiDung);

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                if (AvatarFile.Length > 5 * 1024 * 1024)
                {
                    ViewBag.Error = "Ảnh không được lớn hơn 5MB.";
                    return View("Profile", new UserProfileViewModel(user, userProfile));
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/users");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!string.IsNullOrEmpty(userProfile.AvatarPath) && userProfile.AvatarPath != "/images/users/usericon.png")
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, userProfile.AvatarPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uniqueFileName = $"{user.MaNguoiDung}_{Guid.NewGuid().ToString()}{Path.GetExtension(AvatarFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(fileStream);
                }

                userProfile.AvatarPath = $"/images/users/{uniqueFileName}";

                try
                {
                    _userRepository.UpdateUserProfile(userProfile);
                    ViewBag.Message = "Thay đổi ảnh đại diện thành công!";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi khi lưu ảnh: " + ex.Message;
                }
            }

            return View("Profile", new UserProfileViewModel(user, userProfile));
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.TenDangNhap),
                new Claim(ClaimTypes.Role, user.VaiTro.TenVaiTro),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost]
        public IActionResult ChangePassword(string OldMatKhau, string NewMatKhau)
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Phiên đăng nhập hết hạn." });
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }

            // Kiểm tra mật khẩu hiện tại
            bool isPasswordValid;
            if (user.MatKhau.StartsWith("$2")) // Kiểm tra xem mật khẩu đã được mã hóa bằng BCrypt chưa
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(OldMatKhau, user.MatKhau);
            }
            else
            {
                // Nếu mật khẩu là văn bản thô (chưa mã hóa), so sánh trực tiếp
                isPasswordValid = user.MatKhau == OldMatKhau;
            }

            if (!isPasswordValid)
            {
                return Json(new { success = false, message = "Mật khẩu hiện tại không đúng." });
            }

            // Cập nhật mật khẩu mới và mã hóa bằng BCrypt
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(NewMatKhau);
            try
            {
                _userRepository.UpdateUser(user);
                return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi đổi mật khẩu: " + ex.Message });
            }

        }
        [HttpGet]
        public IActionResult Edit()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = _userRepository.GetUserByEmailOrUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy UserProfile và tạo UserProfileViewModel
            var userProfile = _userRepository.GetUserProfile(user.MaNguoiDung);
            var viewModel = new UserProfileViewModel(user, userProfile);

            return View(viewModel); // Truyền UserProfileViewModel thay vì User
        }

        [HttpGet]
        public IActionResult Index(string searchEmail)
        {
            var users = _userRepository.GetAllUsers();
            if (!string.IsNullOrEmpty(searchEmail))
            {
                users = users.Where(u => u.Email.Contains(searchEmail, StringComparison.OrdinalIgnoreCase));
            }

            var viewModels = users.Select(u => new UserInfoViewModel(u, _userRepository.GetUserProfile(u.MaNguoiDung))).ToList();

            ViewBag.SearchEmail = searchEmail;
            return View(viewModels); // Phải trả về viewModels, không phải users.ToList()
        }
        [HttpGet]
        [Route("generate-hash")]
        public IActionResult GenerateHash()
        {
            string password = "Admin@123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return Content($"Hashed Password: {hashedPassword}");
        }
    }
}