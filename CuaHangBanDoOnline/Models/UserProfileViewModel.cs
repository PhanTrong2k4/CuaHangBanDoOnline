using System.ComponentModel.DataAnnotations;

namespace CuaHangBanDoOnline.Models
{
    public class UserProfileViewModel
    {
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string VaiTro { get; set; }
        public string AvatarPath { get; set; }
        public bool IsEditing { get; set; }
        // Không cần MatKhau và ConfirmMatKhau trong form chính nữa, sẽ xử lý trong modal

        public UserProfileViewModel() { }

        public UserProfileViewModel(User user, UserProfile userProfile)
        {
            MaNguoiDung = user.MaNguoiDung;
            TenDangNhap = user.TenDangNhap;
            Email = user.Email;
            FullName = userProfile.FullName;
            SoDienThoai = userProfile.SoDienThoai;
            VaiTro = user.VaiTro?.TenVaiTro;
            AvatarPath = userProfile.AvatarPath ?? "/images/users/usericon.png";
            IsEditing = false;
        }
    }
}