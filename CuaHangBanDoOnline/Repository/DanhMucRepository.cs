using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuaHangBanDoOnline.Repository
{
    public class DanhMucRepository : IDanhMucRepository
    {
        private readonly CuaHangDbContext _context;

        public DanhMucRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public IEnumerable<DanhMuc> GetDanhMucs()
        {
            return _context.DanhMucs
                .Include(d => d.HangHoaDanhMucs)
                .ThenInclude(hhdm => hhdm.HangHoa)
                .ThenInclude(hh => hh.KhuyenMais)
                .Where(d => d.TenDanhMuc != "Đã xóa") // Lọc bỏ các danh mục đã xóa
                .ToList();
        }

        public DanhMuc GetDanhMuc(int maDanhMuc)
        {
            return _context.DanhMucs
                .Include(d => d.HangHoaDanhMucs)
                .ThenInclude(hhdm => hhdm.HangHoa)
                .ThenInclude(hh => hh.KhuyenMais)
                .FirstOrDefault(d => d.MaDanhMuc == maDanhMuc && d.TenDanhMuc != "Đã xóa"); // Lọc bỏ danh mục đã xóa
        }

        public DanhMuc AddDanhMuc(DanhMuc danhMuc)
        {
            if (danhMuc.TenDanhMuc.Trim().Equals("Đã xóa", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Không được phép tạo danh mục với tên 'Đã xóa'.");
            }

            if (_context.DanhMucs.Any(d => d.TenDanhMuc == danhMuc.TenDanhMuc && d.TenDanhMuc != "Đã xóa"))
            {
                throw new Exception("Danh mục với tên này đã tồn tại.");
            }

            _context.DanhMucs.Add(danhMuc);
            _context.SaveChanges();
            return danhMuc;
        }

        public DanhMuc UpdateDanhMuc(DanhMuc danhMuc)
        {
            if (danhMuc.TenDanhMuc.Trim().Equals("Đã xóa", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Không được phép cập nhật danh mục thành 'Đã xóa'.");
            }

            if (_context.DanhMucs.Any(d => d.TenDanhMuc == danhMuc.TenDanhMuc && d.MaDanhMuc != danhMuc.MaDanhMuc && d.TenDanhMuc != "Đã xóa"))
            {
                throw new Exception("Danh mục với tên này đã tồn tại.");
            }

            _context.DanhMucs.Update(danhMuc);
            _context.SaveChanges();
            return danhMuc;
        }

        public DanhMuc DeleteDanhMuc(int maDanhMuc)
        {
            var danhMuc = _context.DanhMucs
                .Include(d => d.HangHoaDanhMucs)
                .FirstOrDefault(d => d.MaDanhMuc == maDanhMuc);

            if (danhMuc == null)
            {
                return null;
            }

            // Đánh dấu là "Đã xóa" bằng cách đổi tên
            danhMuc.TenDanhMuc = "Đã xóa";

            // Không xóa bản ghi, chỉ cập nhật
            _context.DanhMucs.Update(danhMuc);
            _context.SaveChanges();

            return danhMuc;
        }

    }
}