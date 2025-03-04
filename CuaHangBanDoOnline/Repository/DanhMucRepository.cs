using CuaHangBanDoOnline.Models;
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
            return _context.DanhMucs.ToList();
        }

        public DanhMuc GetDanhMuc(int maDanhMuc)
        {
            return _context.DanhMucs.Find(maDanhMuc);
        }

        public DanhMuc AddDanhMuc(DanhMuc danhMuc)
        {
            _context.DanhMucs.Add(danhMuc);
            _context.SaveChanges();
            return danhMuc;
        }

        public DanhMuc UpdateDanhMuc(DanhMuc danhMuc)
        {
            _context.DanhMucs.Update(danhMuc);
            _context.SaveChanges();
            return danhMuc;
        }

        public DanhMuc DeleteDanhMuc(int maDanhMuc)
        {
            var danhMuc = _context.DanhMucs.Find(maDanhMuc);
            if (danhMuc != null)
            {
                danhMuc.TenDanhMuc = "Đã xóa";
                _context.SaveChanges();
            }
            return danhMuc;
        }
    }
}
