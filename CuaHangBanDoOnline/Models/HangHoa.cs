namespace CuaHangBanDoOnline.Models
{
    public class HangHoa
    {
        public int MaDanhMuc { get; set; } // Khóa ngoại
        public DanhMuc DanhMuc { get; set; } // Điều hướng (Navigation Property)
        public int MaHangHoa { get; set; } // Khóa chính
        public string TenHangHoa { get; set; } = string.Empty; // Tên hàng hóa
        public decimal GiaBan { get; set; } // Giá bán
        public int SoLuongTon { get; set; } // Số lượng tồn kho
        public string MoTa { get; set; } = string.Empty; // Mô tả hàng hóa
    }

}
