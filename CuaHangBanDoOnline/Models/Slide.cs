namespace CuaHangBanDoOnline.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Image { get; set; } // Đường dẫn đến hình ảnh
        public string Title { get; set; } // Tiêu đề của slide
        public string Link { get; set; }  // Liên kết khi nhấn "Mua Ngay"
    }
}