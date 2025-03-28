namespace CuaHangBanDoOnline.Models
{
    public class UserInfoViewModel
    {
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }
        public string FullName { get; set; }
        public string SoDienThoai { get; set; }
        public string AvatarPath { get; set; }

        public UserInfoViewModel(User user, UserProfile userProfile)
        {
            MaNguoiDung = user.MaNguoiDung;
            TenDangNhap = user.TenDangNhap;
            Email = user.Email;
            VaiTro = user.VaiTro?.TenVaiTro ?? "Không xác định";
            FullName = userProfile?.FullName ?? "Chưa cập nhật";
            SoDienThoai = userProfile?.SoDienThoai ?? "Chưa cập nhật";
            AvatarPath = userProfile?.AvatarPath ?? "/images/users/usericon.png";
        }
    }
}