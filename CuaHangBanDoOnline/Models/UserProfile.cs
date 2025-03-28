using System.ComponentModel.DataAnnotations;

namespace CuaHangBanDoOnline.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        public int MaNguoiDung { get; set; }
        public string FullName { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string AvatarPath { get; set; } = "/images/users/usericon.png"; // Không có [Required]

        public User User { get; set; }
    }
}