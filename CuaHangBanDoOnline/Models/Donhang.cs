namespace CuaHangBanDoOnline.Models
{
    public class DonHang
    {
        public int MaDonHang { get; set; } // Khóa chính
        public int MaNguoiDung { get; set; } // Khóa ngoại đến bảng Người dùng
        public User NguoiDung { get; set; } = null!; // Thuộc tính điều hướng
        public decimal TongTien { get; set; } // Tổng tiền
        public DateTime NgayDatHang { get; set; } // Ngày đặt hàng
        public string TrangThai { get; set; } = "ChoDuyet"; // Trạng thái mặc định là "Chờ Duyệt"

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>(); // Danh sách chi tiết đơn hàng
    }
}