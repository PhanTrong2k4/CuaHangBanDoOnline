using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class ChiTietDonHang
    {
        [Key] // Đánh dấu là khóa chính
        public int MaChiTietDonHang { get; set; }

        [ForeignKey(nameof(DonHang))] // Khóa ngoại đến bảng Đơn hàng
        public int MaDonHang { get; set; }
        public DonHang DonHang { get; set; } = null!; // Thuộc tính điều hướng

        [ForeignKey(nameof(HangHoa))] // Khóa ngoại đến bảng Hàng hóa
        public int MaHangHoa { get; set; }
        public HangHoa HangHoa { get; set; } = null!; // Thuộc tính điều hướng

        public int SoLuong { get; set; } // Số lượng
        [Column(TypeName = "decimal(18,2)")] // Định dạng cột giá trị decimal
        public decimal GiaBan { get; set; } // Giá bán đơn vị của hàng hóa
    }
}
