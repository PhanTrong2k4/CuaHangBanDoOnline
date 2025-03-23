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
            return _context.GioHangs
                .Include(gh => gh.ChiTietGioHangs)
                .ThenInclude(ctgh => ctgh.HangHoa)
                .FirstOrDefault(gh => gh.MaNguoiDung == maNguoiDung);
        }

        public GioHang CreateGioHang(int maNguoiDung)
        {
            var gioHang = new GioHang
            {
                MaNguoiDung = maNguoiDung,
                ChiTietGioHangs = new List<ChiTietGioHang>()
            };
            _context.GioHangs.Add(gioHang);
            _context.SaveChanges();
            return gioHang;
        }

        public void UpdateGioHang(GioHang gioHang)
        {
            _context.GioHangs.Update(gioHang);
            _context.SaveChanges();
        }

        public void DeleteGioHang(int maGioHang)
        {
            var gioHang = _context.GioHangs
                .Include(gh => gh.ChiTietGioHangs)
                .FirstOrDefault(gh => gh.MaGioHang == maGioHang);
            if (gioHang != null)
            {
                _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
                _context.GioHangs.Remove(gioHang);
                _context.SaveChanges();
            }
        }
    }
}