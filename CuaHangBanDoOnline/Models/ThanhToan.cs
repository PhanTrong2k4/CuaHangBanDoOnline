namespace CuaHangBanDoOnline.Models
{
    public class ThanhToan
    {
        public int MaThanhToan { get; set; } // Khóa chính
        public int MaDonHang { get; set; } // Khóa ngoại đến bảng Đơn hàng
        public DonHang DonHang { get; set; } = null!; // Thuộc tính điều hướng
        public int? MaHangHoa { get; set; } // Thêm để liên kết với sản phẩm
        public decimal SoTien { get; set; } // Số tiền thanh toán
        public string PhuongThucThanhToan { get; set; } = "TienMat"; // Phương thức thanh toán (mặc định: Tiền mặt)
        public DateTime NgayThanhToan { get; set; } = DateTime.Now; // Ngày thanh toán
        public string? KeyGame { get; set; } // Thêm KeyGame
    }
}