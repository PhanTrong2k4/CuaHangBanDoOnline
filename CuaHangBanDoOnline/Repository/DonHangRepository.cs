using CuaHangBanDoOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuaHangBanDoOnline.Repository
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly CuaHangDbContext _context;

        public DonHangRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public IEnumerable<DonHang> GetDonHangs()
        {
            return _context.DonHangs.ToList();
        }

        public DonHang GetDonHang(int maDonHang)
        {
            return _context.DonHangs.Find(maDonHang);
        }

        public DonHang AddDonHang(DonHang donHang)
        {
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();
            return donHang;
        }

        public DonHang UpdateDonHang(DonHang donHang)
        {
            _context.DonHangs.Update(donHang);
            _context.SaveChanges();
            return donHang;
        }

        public DonHang DeleteDonHang(int maDonHang)
        {
            var donHang = _context.DonHangs.Find(maDonHang);
            if (donHang != null)
            {
                donHang.TrangThai = "Đã xóa";
                _context.SaveChanges();
            }
            return donHang;
        }
    }
}
