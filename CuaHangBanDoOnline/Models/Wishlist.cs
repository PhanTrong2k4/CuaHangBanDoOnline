using System.ComponentModel.DataAnnotations;

namespace CuaHangBanDoOnline.Models
{
    public class Wishlist
    {
        [Key]
        public int MaWishlist { get; set; } // Khóa chính của danh sách yêu thích

        public int MaNguoiDung { get; set; } // Khóa ngoại đến bảng User (Người dùng)
        public int MaHangHoa { get; set; } // Khóa ngoại đến bảng HangHoa (Hàng hóa)

        // Thuộc tính điều hướng
        public User NguoiDung { get; set; } = null!; // Người dùng sở hữu danh sách yêu thích
        public HangHoa HangHoa { get; set; } = null!; // Hàng hóa trong danh sách yêu thích
    }
}