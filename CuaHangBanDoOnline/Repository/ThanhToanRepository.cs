using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Repository
{
    public class ThanhToanRepository : IThanhToanRepository
    {
        private readonly CuaHangDbContext _context;

        public ThanhToanRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ThanhToan> GetThanhToans()
        {
            return _context.ThanhToans.Include(tt => tt.DonHang).ToList();
        }

        public ThanhToan GetThanhToan(int maThanhToan)
        {
            return _context.ThanhToans.Include(tt => tt.DonHang).FirstOrDefault(tt => tt.MaThanhToan == maThanhToan);
        }

        public ThanhToan AddThanhToan(ThanhToan thanhToan)
        {
            _context.ThanhToans.Add(thanhToan);
            var donHang = _context.DonHangs.Find(thanhToan.MaDonHang);
            if (donHang != null && donHang.TrangThai == "ChoDuyet" && !string.IsNullOrEmpty(thanhToan.KeyGame))
            {
                donHang.TrangThai = "DaThanhToan";
                _context.DonHangs.Update(donHang);
            }
            _context.SaveChanges();
            return thanhToan;
        }

        public ThanhToan UpdateThanhToan(ThanhToan thanhToan)
        {
            _context.ThanhToans.Update(thanhToan);
            _context.SaveChanges();
            return thanhToan;
        }

        public ThanhToan DeleteThanhToan(int maThanhToan)
        {
            var thanhToan = _context.ThanhToans.Find(maThanhToan);
            if (thanhToan != null)
            {
                thanhToan.PhuongThucThanhToan = "DaXoa";
                _context.SaveChanges();
            }
            return thanhToan;
        }

        public ThanhToan ThanhToanDonHang(int maDonHang, string phuongThucThanhToan, decimal soTien)
        {
            var donHang = _context.DonHangs.Find(maDonHang);
            if (donHang == null || donHang.TrangThai != "ChoDuyet")
                return null;

            var thanhToan = new ThanhToan
            {
                MaDonHang = maDonHang,
                SoTien = soTien,
                PhuongThucThanhToan = phuongThucThanhToan,
                NgayThanhToan = DateTime.Now
            };

            _context.ThanhToans.Add(thanhToan);
            _context.SaveChanges();
            return thanhToan;
        }

        // Phương thức mới để tạo Key Game cho từng sản phẩm
        public List<ThanhToan> ThanhToanDonHangWithKeyGames(int maDonHang, List<(int MaHangHoa, decimal GiaBan, int SoLuong)> chiTietDonHangs, string phuongThucThanhToan)
        {
            var donHang = _context.DonHangs.Find(maDonHang);
            if (donHang == null || donHang.TrangThai != "ChoDuyet")
                return null;

            var thanhToans = new List<ThanhToan>();
            foreach (var chiTiet in chiTietDonHangs)
            {
                for (int i = 0; i < chiTiet.SoLuong; i++)
                {
                    var thanhToan = new ThanhToan
                    {
                        MaDonHang = maDonHang,
                        MaHangHoa = chiTiet.MaHangHoa, // Gán MaHangHoa để biết Key Game thuộc sản phẩm nào
                        SoTien = chiTiet.GiaBan,
                        PhuongThucThanhToan = phuongThucThanhToan,
                        NgayThanhToan = DateTime.Now,
                        KeyGame = GenerateApiKey()
                    };
                    thanhToans.Add(thanhToan);
                    _context.ThanhToans.Add(thanhToan);
                }
            }

            donHang.TrangThai = "DaThanhToan";
            _context.DonHangs.Update(donHang);
            _context.SaveChanges();

            return thanhToans;
        }

        // Hàm tạo API Key tự động
        private string GenerateApiKey()
        {
            string prefix = "TGAME-";
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
            return $"{prefix}{uniqueId}";
        }
    }
}