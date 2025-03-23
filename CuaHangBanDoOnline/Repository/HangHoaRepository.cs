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
        return _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .ToList();
    }

    public IEnumerable<DanhMuc> GetDanhMucs()
    {
        return _context.DanhMucs.ToList();
    }

    public HangHoa GetHangHoa(int maHangHoa)
    {
        return _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
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

        // Đảm bảo giá bán ban đầu bằng giá gốc
        hangHoa.GiaBan = hangHoa.GiaGoc;

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
        // Lấy hàng hóa hiện tại từ CSDL để kiểm tra
        var existingHangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .FirstOrDefault(h => h.MaHangHoa == hangHoa.MaHangHoa);

        if (existingHangHoa == null)
        {
            throw new Exception("Sản phẩm không tồn tại.");
        }

        // Cập nhật các thuộc tính
        existingHangHoa.TenHangHoa = hangHoa.TenHangHoa;
        existingHangHoa.GiaGoc = hangHoa.GiaGoc;
        existingHangHoa.GiaBan = hangHoa.GiaBan; // Giá bán có thể đã được cập nhật từ giảm giá
        existingHangHoa.MoTa = hangHoa.MoTa;
        existingHangHoa.SoLuongTon = hangHoa.SoLuongTon;
        existingHangHoa.Hinh = hangHoa.Hinh;

        _context.HangHoas.Update(existingHangHoa);
        _context.SaveChanges();
        return existingHangHoa;
    }

    public HangHoa ThemDanhMuc(int maHangHoa, int maDanhMuc)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
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

            _context.HangHoaDanhMucs.Add(newDanhMuc);
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("Danh mục này đã tồn tại trong hàng hóa.");
        }

        // Load lại danh sách danh mục để đảm bảo dữ liệu mới nhất
        _context.Entry(hangHoa).Collection(h => h.HangHoaDanhMucs).Load();

        return hangHoa;
    }

    public HangHoa DeleteHangHoa(int maHangHoa)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);

        if (hangHoa != null)
        {
            // Xóa các bản ghi liên quan trong HangHoaDanhMuc
            _context.HangHoaDanhMucs.RemoveRange(hangHoa.HangHoaDanhMucs);

            // Xóa các bản ghi khuyến mãi liên quan
            var khuyenMais = _context.KhuyenMais.Where(km => km.MaHangHoa == maHangHoa);
            _context.KhuyenMais.RemoveRange(khuyenMais);

            // Xóa hàng hóa
            _context.HangHoas.Remove(hangHoa);
            _context.SaveChanges();
        }
        return hangHoa;
    }

    // Thêm giảm giá
    public void ThemGiamGia(int maHangHoa, decimal phanTramGiamGia, DateTime ngayBatDau, DateTime ngayKetThuc)
    {
        var hangHoa = _context.HangHoas.Find(maHangHoa);
        if (hangHoa == null)
        {
            throw new Exception("Sản phẩm không tồn tại.");
        }

        // Kiểm tra nếu đã có khuyến mãi thì không cho thêm mới
        var existingKhuyenMai = _context.KhuyenMais
            .FirstOrDefault(km => km.MaHangHoa == maHangHoa && km.NgayKetThuc >= DateTime.Now);
        if (existingKhuyenMai != null)
        {
            throw new Exception("Sản phẩm đã có khuyến mãi đang áp dụng.");
        }

        // Tạo bản ghi khuyến mãi
        var khuyenMai = new KhuyenMai
        {
            MaHangHoa = maHangHoa,
            PhanTramGiamGia = phanTramGiamGia,
            NgayBatDau = ngayBatDau,
            NgayKetThuc = ngayKetThuc
        };
        _context.KhuyenMais.Add(khuyenMai);

        // Cập nhật giá bán dựa trên phần trăm giảm giá
        hangHoa.GiaBan = hangHoa.GiaGoc - (hangHoa.GiaGoc * (phanTramGiamGia / 100m));
        _context.HangHoas.Update(hangHoa);

        _context.SaveChanges();
    }

    // Xóa giảm giá
    public void XoaGiamGia(int maHangHoa)
    {
        var hangHoa = _context.HangHoas.Find(maHangHoa);
        if (hangHoa == null)
        {
            throw new Exception("Sản phẩm không tồn tại.");
        }

        // Xóa bản ghi khuyến mãi liên quan
        var khuyenMai = _context.KhuyenMais
            .FirstOrDefault(km => km.MaHangHoa == maHangHoa);
        if (khuyenMai != null)
        {
            _context.KhuyenMais.Remove(khuyenMai);
        }

        // Khôi phục giá bán về giá gốc
        hangHoa.GiaBan = hangHoa.GiaGoc;
        _context.HangHoas.Update(hangHoa);

        _context.SaveChanges();
    }
    public IEnumerable<HangHoa> GetHangHoasFiltered(string search, string category, string priceRange, string discount, string stock, string sortBy)
    {
        var query = _context.HangHoas
            .Include(hh => hh.HangHoaDanhMucs)
            .Include(hh => hh.KhuyenMais)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(hh => EF.Functions.Like(hh.TenHangHoa, $"%{search}%"));
        }

        // Lọc theo danh mục (chỉ áp dụng trong Index)
        if (!string.IsNullOrEmpty(category) && int.TryParse(category, out int categoryId))
        {
            query = query.Where(hh => hh.HangHoaDanhMucs.Any(hdm => hdm.MaDanhMuc == categoryId));
        }

        // Lọc theo mức giá
        if (!string.IsNullOrEmpty(priceRange))
        {
            switch (priceRange)
            {
                case "0-500000":
                    query = query.Where(hh => hh.GiaBan >= 0 && hh.GiaBan <= 500000);
                    break;
                case "500000-1000000":
                    query = query.Where(hh => hh.GiaBan > 500000 && hh.GiaBan <= 1000000);
                    break;
                case "1000000-5000000":
                    query = query.Where(hh => hh.GiaBan > 1000000 && hh.GiaBan <= 5000000);
                    break;
                case "5000000-":
                    query = query.Where(hh => hh.GiaBan > 5000000);
                    break;
            }
        }

        // Lọc theo tình trạng hàng
        if (!string.IsNullOrEmpty(stock))
        {
            switch (stock)
            {
                case "in-stock":
                    query = query.Where(hh => hh.SoLuongTon > 0);
                    break;
                case "low-stock":
                    query = query.Where(hh => hh.SoLuongTon > 0 && hh.SoLuongTon <= 10);
                    break;
                case "out-of-stock":
                    query = query.Where(hh => hh.SoLuongTon == 0);
                    break;
            }
        }

        // Sắp xếp
        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy)
            {
                case "price-asc":
                    query = query.OrderBy(hh => hh.GiaBan);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(hh => hh.GiaBan);
                    break;
                case "name-asc":
                    query = query.OrderBy(hh => hh.TenHangHoa);
                    break;
                case "name-desc":
                    query = query.OrderByDescending(hh => hh.TenHangHoa);
                    break;
            }
        }

        return query.ToList();
    }
}