using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Repository
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly CuaHangDbContext _context;

        public GioHangRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public GioHang GetGioHangByUserId(int maNguoiDung)
        {
            try
            {
                var gioHang = _context.GioHangs
                    .Include(g => g.ChiTietGioHangs)
                    .ThenInclude(c => c.HangHoa)
                    .FirstOrDefault(g => g.MaNguoiDung == maNguoiDung);
                if (gioHang != null && gioHang.ChiTietGioHangs == null)
                {
                    gioHang.ChiTietGioHangs = new List<ChiTietGioHang>();
                }
                Console.WriteLine($"GetGioHangByUserId: MaNguoiDung={maNguoiDung}, Found={gioHang != null}, ChiTietCount={(gioHang != null ? gioHang.ChiTietGioHangs.Count : 0)}");
                return gioHang;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong GetGioHangByUserId: {ex.Message}");
                return null;
            }
        }

        public int GetCartCountByUserId(int maNguoiDung)
        {
            try
            {
                var gioHang = _context.GioHangs
                    .Include(g => g.ChiTietGioHangs)
                    .FirstOrDefault(g => g.MaNguoiDung == maNguoiDung);
                int count = gioHang != null && gioHang.ChiTietGioHangs != null
                    ? gioHang.ChiTietGioHangs.Sum(item => item.SoLuong)
                    : 0;
                Console.WriteLine($"GetCartCountByUserId: MaNguoiDung={maNguoiDung}, Count={count}");
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong GetCartCountByUserId: {ex.Message}");
                return 0;
            }
        }

        public GioHang CreateGioHang(int maNguoiDung)
        {
            try
            {
                var gioHang = new GioHang
                {
                    MaNguoiDung = maNguoiDung,
                    ChiTietGioHangs = new List<ChiTietGioHang>()
                };
                _context.GioHangs.Add(gioHang);
                _context.SaveChanges();
                Console.WriteLine($"CreateGioHang: MaNguoiDung={maNguoiDung}, MaGioHang={gioHang.MaGioHang}");
                return gioHang;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong CreateGioHang: {ex.Message}");
                return null;
            }
        }

        public void UpdateGioHang(GioHang gioHang)
        {
            try
            {
                _context.GioHangs.Update(gioHang);
                int changes = _context.SaveChanges();
                Console.WriteLine($"UpdateGioHang: MaGioHang={gioHang.MaGioHang}, Changes={changes}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong UpdateGioHang: {ex.Message}");
            }
        }

        public void DeleteGioHang(int maGioHang)
        {
            try
            {
                var gioHang = _context.GioHangs
                    .Include(g => g.ChiTietGioHangs)
                    .FirstOrDefault(g => g.MaGioHang == maGioHang);
                if (gioHang != null)
                {
                    _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
                    _context.GioHangs.Remove(gioHang);
                    int changes = _context.SaveChanges();
                    Console.WriteLine($"DeleteGioHang: MaGioHang={maGioHang}, Changes={changes}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong DeleteGioHang: {ex.Message}");
            }
        }
    }
}