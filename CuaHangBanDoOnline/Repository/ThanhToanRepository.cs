using CuaHangBanDoOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return _context.ThanhToans.ToList();
        }

        public ThanhToan GetThanhToan(int maThanhToan)
        {
            return _context.ThanhToans.Find(maThanhToan);
        }

        public ThanhToan AddThanhToan(ThanhToan thanhToan)
        {
            _context.ThanhToans.Add(thanhToan);
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
                thanhToan.PhuongThucThanhToan = "Đã xóa";
                _context.SaveChanges();
            }
            return thanhToan;
        }
    }
}
