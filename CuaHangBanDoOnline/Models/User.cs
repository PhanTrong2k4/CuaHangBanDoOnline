using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class User
    {
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int MaVaiTro { get; set; } = (int)RoleType.Customer;
        public string? EmailConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;

        [ForeignKey("MaVaiTro")]
        public Role VaiTro { get; set; }
        public UserProfile? UserProfile { get; set; }
    }
}