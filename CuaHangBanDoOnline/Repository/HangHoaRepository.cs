using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public class HangHoaRepository : IHangHoaRepository
{
    private readonly CuaHangDbContext _context;

    public HangHoaRepository(CuaHangDbContext context)
    {
        _context = context;
    }

    public IEnumerable<HangHoa> GetHangHoas()
    {
        return _context.HangHoas.ToList();
    }

    public IEnumerable<DanhMuc> GetDanhMucs()
    {
        return _context.DanhMucs.ToList();
    }

    public HangHoa GetHangHoa(int maHangHoa)
    {
        return _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc) // Load thông tin danh mục
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);
    }


    public HangHoa AddHangHoa(HangHoa hangHoa, List<int> danhMucIds, IFormFile HinhAnh)
    {
        // Xử lý ảnh nếu có tải lên
        if (HinhAnh != null && HinhAnh.Length > 0)
        {
            var fileName = Path.GetFileName(HinhAnh.FileName);
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Tạo thư mục images nếu chưa tồn tại
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            var filePath = Path.Combine(imagesFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                HinhAnh.CopyTo(stream);
            }

            hangHoa.Hinh = $"/images/{fileName}";
        }

        // Thêm hàng hóa vào cơ sở dữ liệu
        _context.HangHoas.Add(hangHoa);
        _context.SaveChanges();

        // Tạo danh sách HangHoaDanhMuc
        if (danhMucIds != null && danhMucIds.Any())
        {
            foreach (var danhMucId in danhMucIds)
            {
                var hangHoaDanhMuc = new HangHoaDanhMuc
                {
                    MaHangHoa = hangHoa.MaHangHoa,
                    MaDanhMuc = danhMucId
                };
                _context.HangHoaDanhMucs.Add(hangHoaDanhMuc);
            }
            _context.SaveChanges();
        }

        return hangHoa;
    }
    public HangHoa UpdateHangHoa(HangHoa hangHoa)
    {
        _context.HangHoas.Update(hangHoa);
        _context.SaveChanges();
        return hangHoa;
    }
    public HangHoa ThemDanhMuc(int maHangHoa, int maDanhMuc)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc) // Load danh mục của hàng hóa
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);

        if (hangHoa == null)
        {
            throw new Exception($"Không tìm thấy hàng hóa với ID: {maHangHoa}");
        }

        var danhMuc = _context.DanhMucs.Find(maDanhMuc);
        if (danhMuc == null)
        {
            throw new Exception($"Không tìm thấy danh mục với ID: {maDanhMuc}");
        }

        // Kiểm tra nếu chưa có danh mục này trong danh sách thì mới thêm
        if (!hangHoa.HangHoaDanhMucs.Any(dm => dm.MaDanhMuc == maDanhMuc))
        {
            var newDanhMuc = new HangHoaDanhMuc
            {
                MaHangHoa = maHangHoa,
                MaDanhMuc = maDanhMuc
            };

            _context.HangHoaDanhMucs.Add(newDanhMuc); // Thêm vào DbSet trực tiếp

            int rowsAffected = _context.SaveChanges(); // Lưu vào database

            if (rowsAffected == 0)
            {
                throw new Exception("Lưu dữ liệu thất bại! Không có bản ghi nào được cập nhật.");
            }
        }
        else
        {
            throw new Exception("Danh mục này đã tồn tại trong hàng hóa.");
        }

        // Load lại danh sách danh mục để đảm bảo dữ liệu mới nhất
        _context.Entry(hangHoa).Collection(h => h.HangHoaDanhMucs).Load();

        return hangHoa; // Trả về hàng hóa đã cập nhật
    }



    public HangHoa DeleteHangHoa(int maHangHoa)
    {
        var hangHoa = _context.HangHoas.Find(maHangHoa);
        if (hangHoa != null)
        {
            hangHoa.TenHangHoa = "Đã xóa";
            _context.SaveChanges();
        }
        return hangHoa;
    }

    
}
