using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class HangHoa
    {
        [Key]
        public int MaHangHoa { get; set; }

        [Required(ErrorMessage = "Tên hàng hóa là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên hàng hóa không được vượt quá 100 ký tự")]
        public string TenHangHoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá gốc là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn hoặc bằng 0")]
        public decimal GiaGoc { get; set; } // Thêm thuộc tính GiaGoc

        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal GiaBan { get; set; }

        [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; } = string.Empty;

        public string Hinh { get; set; } = string.Empty;

        // Quan hệ với HangHoaDanhMuc
        public List<HangHoaDanhMuc> HangHoaDanhMucs { get; set; } = new();

        // Quan hệ với KhuyenMai
        public List<KhuyenMai> KhuyenMais { get; set; } = new();

        [NotMapped]
        public List<int> DanhMucIds { get; set; } = new List<int>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}