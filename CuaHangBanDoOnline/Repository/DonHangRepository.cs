using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
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

        public DonHang CreateDonHang(DonHang donHang)
        {
            donHang.TongTien = donHang.ChiTietDonHangs.Sum(ctdh => ctdh.GiaBan * ctdh.SoLuong);
            donHang.NgayDatHang = DateTime.Now;
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();
            return donHang;
        }

        public IEnumerable<DonHang> GetDonHangsByUserId(int maNguoiDung)
        {
            var donHangs = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ctdh => ctdh.HangHoa)
                .Where(dh => dh.MaNguoiDung == maNguoiDung)
                .ToList();

            CheckAndDeleteExpiredOrders();
            return donHangs;
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
            donHang.TongTien = donHang.ChiTietDonHangs.Sum(ctdh => ctdh.GiaBan * ctdh.SoLuong);
            _context.DonHangs.Update(donHang);
            _context.SaveChanges();
        }

        public void DeleteDonHang(int maDonHang)
        {
            var donHang = GetDonHang(maDonHang);
            if (donHang != null)
            {
                RestoreStock(donHang);
                _context.ChiTietDonHangs.RemoveRange(donHang.ChiTietDonHangs);
                _context.DonHangs.Remove(donHang);
                _context.SaveChanges();
            }
        }

        private void CheckAndDeleteExpiredOrders()
        {
            var expiredOrders = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ctdh => ctdh.HangHoa)
                .Where(dh => dh.TrangThai == "ChoDuyet" && dh.NgayDatHang < DateTime.Now.AddDays(-1))
                .ToList();

            foreach (var order in expiredOrders)
            {
                RestoreStock(order);
                _context.ChiTietDonHangs.RemoveRange(order.ChiTietDonHangs);
                _context.DonHangs.Remove(order);
            }
            _context.SaveChanges();
        }

        private void RestoreStock(DonHang donHang)
        {
            foreach (var chiTiet in donHang.ChiTietDonHangs)
            {
                var hangHoa = _context.HangHoas.Find(chiTiet.MaHangHoa);
                if (hangHoa != null)
                {
                    hangHoa.SoLuongTon += chiTiet.SoLuong;
                    _context.HangHoas.Update(hangHoa);
                }
            }
        }
        public IEnumerable<DonHang> GetAllDonHangs()
        {
            return _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ctdh => ctdh.HangHoa)
                .Include(dh => dh.NguoiDung)
                .ToList();
        }
    }
}