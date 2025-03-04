using System.ComponentModel.DataAnnotations;

namespace CuaHangBanDoOnline.Models
{
    public class HoaDon
    {
        [Key]
        public int MaHoaDon { get; set; } // Khóa chính

        public int MaDonHang { get; set; } // Khóa ngoại đến bảng Đơn hàng
        public DonHang DonHang { get; set; } = null!; // Thuộc tính điều hướng

        public decimal TongTien { get; set; } // Tổng số tiền
        public DateTime NgayTao { get; set; } = DateTime.Now; // Mặc định là thời điểm hiện tại

        // Thêm khóa ngoại liên kết đến User (Khách hàng)
        public int MaNguoiDung { get; set; } // Khóa ngoại đến User
        public User NguoiDung { get; set; } = null!; // Thuộc tính điều hướng
    }
}
