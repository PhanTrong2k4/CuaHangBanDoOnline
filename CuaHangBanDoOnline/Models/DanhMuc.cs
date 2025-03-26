namespace CuaHangBanDoOnline.Models
{
    public class DanhMuc
    {
        public int MaDanhMuc { get; set; } 
        public string TenDanhMuc { get; set; } = string.Empty; 
        public string MoTa { get; set; } = string.Empty;
        public decimal PhanTramGiamGia { get; set; }
        public List<HangHoaDanhMuc> HangHoaDanhMucs { get; set; } = new();
    }
}
