using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class HangHoaDanhMuc
    {
        [ForeignKey(nameof(DanhMuc))]
        public int MaDanhMuc { get; set; }
        public DanhMuc DanhMuc { get; set; }  

        [ForeignKey(nameof(HangHoa))]
        public int MaHangHoa { get; set; }
        public HangHoa HangHoa { get; set; }  
    }
}
