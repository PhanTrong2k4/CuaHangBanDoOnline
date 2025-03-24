namespace CuaHangBanDoOnline.Models
{
    public class User
    {
        public int MaNguoiDung { get; set; } // Khóa chính
        public string TenDangNhap { get; set; } = string.Empty; // Tên đăng nhập
        public string MatKhau { get; set; } = string.Empty; // Mật khẩu
        public string Email { get; set; } = string.Empty; // Địa chỉ email

        // Khóa ngoại đến Vai trò
        public int MaVaiTro { get; set; } // Khóa ngoại đến bảng Vai trò
        public Role VaiTro { get; set; } = null!; // Thuộc tính điều hướng
        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
