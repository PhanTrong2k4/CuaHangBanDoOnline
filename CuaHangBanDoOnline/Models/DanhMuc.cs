namespace CuaHangBanDoOnline.Models
{
    public class DanhMuc
    {
        public int MaDanhMuc { get; set; } // Khóa chính
        public string TenDanhMuc { get; set; } = string.Empty; // Tên danh mục
        public string MoTa { get; set; } = string.Empty; // Mô tả danh mục

        // Danh sách hàng hóa thuộc danh mục
        public ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
    }
}
