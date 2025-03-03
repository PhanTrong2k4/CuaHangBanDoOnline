using CuaHangBanDoOnline.Models;
using System.Collections.Generic;
using System.Linq;

namespace CuaHangBanDoOnline.Repository
{
    public class HoadonRepository : IHoadonRepository
    {
        private readonly CuaHangDbContext _context;

        public HoadonRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public IEnumerable<HoaDon> GetHoadons()
        {
            return _context.HoaDons.ToList();
        }

        public HoaDon GetHoadon(int maHoadon)
        {
            return _context.HoaDons.FirstOrDefault(hd => hd.MaHoaDon == maHoadon);
        }

        public HoaDon AddHoadon(HoaDon hoadon)
        {
            _context.HoaDons.Add(hoadon);
            _context.SaveChanges();
            return hoadon;
        }

        public HoaDon UpdateHoadon(HoaDon hoadon)
        {
            _context.HoaDons.Update(hoadon);
            _context.SaveChanges();
            return hoadon;
        }

        public HoaDon DeleteHoadon(int maHoadon)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.MaHoaDon == maHoadon);
            if (hoadon != null)
            {
                _context.HoaDons.Remove(hoadon);
                _context.SaveChanges();
            }
            return hoadon;
        }
    }
}
