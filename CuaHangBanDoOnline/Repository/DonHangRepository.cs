using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Repository
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly CuaHangDbContext _context;

        public DonHangRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public DonHang CreateDonHang(DonHang donHang)
        {
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();
            return donHang;
        }

        public IEnumerable<DonHang> GetDonHangsByUserId(int maNguoiDung)
        {
            return _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ctdh => ctdh.HangHoa)
                .Where(dh => dh.MaNguoiDung == maNguoiDung)
                .ToList();
        }

        public DonHang GetDonHang(int maDonHang)
        {
            return _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ctdh => ctdh.HangHoa)
                .FirstOrDefault(dh => dh.MaDonHang == maDonHang);
        }

        public void UpdateDonHang(DonHang donHang)
        {
            _context.DonHangs.Update(donHang);
            _context.SaveChanges();
        }

        public void DeleteDonHang(int maDonHang)
        {
            var donHang = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefault(dh => dh.MaDonHang == maDonHang);
            if (donHang != null)
            {
                _context.ChiTietDonHangs.RemoveRange(donHang.ChiTietDonHangs);
                _context.DonHangs.Remove(donHang);
                _context.SaveChanges();
            }
        }
    }
}