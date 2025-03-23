using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBanDoOnline.Models
{
    public class KhuyenMai
    {
        [Key] 
        public int MaKhuyenMai { get; set; }

        public int MaHangHoa { get; set; }

        [ForeignKey("MaHangHoa")]
        public HangHoa HangHoa { get; set; }

        public decimal PhanTramGiamGia { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }
}
