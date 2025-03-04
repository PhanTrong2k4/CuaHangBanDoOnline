using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class HangHoa
    {
        [Key] public int MaHangHoa { get; set; }

        public string TenHangHoa { get; set; } = string.Empty; 
        public decimal GiaBan { get; set; } 
        public int SoLuongTon { get; set; } 
        public string MoTa { get; set; } = string.Empty;
        public string Hinh { get; set; } = string.Empty;
        public List<HangHoaDanhMuc> HangHoaDanhMucs { get; set; } = new();
        [NotMapped]
        public List<int> DanhMucIds { get; set; } = new List<int>();

    }

}
