using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }
        public int MaNguoiDung { get; set; } // Khóa ngoại liên kết với User
        public string Token { get; set; } = string.Empty; // Token reset mật khẩu
        public DateTime? Expires { get; set; } // Thời gian hết hạn
        public bool IsUsed { get; set; } = false; // Đánh dấu token đã dùng chưa

        [ForeignKey("MaNguoiDung")]
        public virtual User User { get; set; } // Quan hệ với User

    }
}