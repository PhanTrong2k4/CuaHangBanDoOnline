namespace CuaHangBanDoOnline.Models
{
    public class ChiTietGioHang
    {
        public int MaChiTietGioHang { get; set; } // Khóa chính
        public int MaGioHang { get; set; } // Khóa ngoại đến bảng Giỏ hàng
        public GioHang GioHang { get; set; } = null!; // Thuộc tính điều hướng

        public int MaHangHoa { get; set; } // Khóa ngoại đến bảng Hàng hóa
        public HangHoa HangHoa { get; set; } = null!; // Thuộc tính điều hướng

        public int SoLuong { get; set; } // Số lượng
        public decimal GiaBan { get; set; } // Giá bán của sản phẩm
    }

}
