namespace CuaHangBanDoOnline.Models
{
    public class PageContent
    {
        public int Id { get; set; }
        public string PageName { get; set; } // Tên trang: "Privacy", "Contact", "AboutUs"
        public string Content { get; set; }  // Nội dung HTML của trang
        public DateTime LastUpdated { get; set; } // Thời gian cập nhật cuối cùng
    }
}