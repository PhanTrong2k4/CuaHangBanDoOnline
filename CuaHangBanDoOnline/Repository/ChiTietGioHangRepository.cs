using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Repository
{
    public class ChiTietGioHangRepository : IChiTietGioHangRepository
    {
        private readonly CuaHangDbContext _context;

        public ChiTietGioHangRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ChiTietGioHang> GetChiTietGioHangs()
        {
            return _context.ChiTietGioHangs
                .Include(ctgh => ctgh.HangHoa)
                .ToList();
        }

        public IEnumerable<ChiTietGioHang> GetChiTietGioHangsByGioHangId(int maGioHang)
        {
            return _context.ChiTietGioHangs
                .Include(ctgh => ctgh.HangHoa)
                .Where(ctgh => ctgh.MaGioHang == maGioHang)
                .ToList();
        }

        public ChiTietGioHang GetChiTietGioHangByHangHoaId(int maGioHang, int maHangHoa)
        {
            return _context.ChiTietGioHangs
                .Include(ctgh => ctgh.HangHoa)
                .FirstOrDefault(ctgh => ctgh.MaGioHang == maGioHang && ctgh.MaHangHoa == maHangHoa);
        }

        public ChiTietGioHang GetChiTietGioHang(int maChiTietGioHang)
        {
            return _context.ChiTietGioHangs
                .Include(ctgh => ctgh.HangHoa)
                .FirstOrDefault(ctgh => ctgh.MaChiTietGioHang == maChiTietGioHang);
        }

        public ChiTietGioHang AddChiTietGioHang(ChiTietGioHang chiTietGioHang)
        {
            _context.ChiTietGioHangs.Add(chiTietGioHang);
            _context.SaveChanges();
            return chiTietGioHang;
        }

        public ChiTietGioHang UpdateChiTietGioHang(ChiTietGioHang chiTietGioHang)
        {
            _context.ChiTietGioHangs.Update(chiTietGioHang);
            _context.SaveChanges();
            return chiTietGioHang;
        }

        public void DeleteChiTietGioHang(int maChiTietGioHang)
        {
            var chiTietGioHang = _context.ChiTietGioHangs
                .FirstOrDefault(ctgh => ctgh.MaChiTietGioHang == maChiTietGioHang);
            if (chiTietGioHang != null)
            {
                _context.ChiTietGioHangs.Remove(chiTietGioHang);
                _context.SaveChanges();
            }
        }
    }
}