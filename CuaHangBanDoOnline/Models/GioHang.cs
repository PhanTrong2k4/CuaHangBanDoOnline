namespace CuaHangBanDoOnline.Models
{
    public class GioHang
    {
        public int MaGioHang { get; set; } // Khóa chính
        public int MaNguoiDung { get; set; } // Khóa ngoại đến bảng Người dùng
        public User NguoiDung { get; set; } = null!; // Thuộc tính điều hướng
        public decimal TongTien { get; set; } // Tổng tiền
        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>(); // Danh sách chi tiết giỏ hàng
    }

}
