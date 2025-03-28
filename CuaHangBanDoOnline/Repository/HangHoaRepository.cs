using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using CuaHangBanDoOnline.Repository;

public class HangHoaRepository : IHangHoaRepository
{
    private readonly CuaHangDbContext _context;

    public HangHoaRepository(CuaHangDbContext context)
    {
        _context = context;
    }

    public IEnumerable<HangHoa> GetHangHoas()
    {
        var hangHoas = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .Include(h => h.KhuyenMais)
            .Where(h => h.TenHangHoa != "Đã xóa") // Lọc bỏ các sản phẩm đã soft delete
            .ToList();

        // Recalculate GiaBan based on active promotions
        var currentDate = DateTime.Now;
        foreach (var hangHoa in hangHoas)
        {
            var activeKhuyenMai = hangHoa.KhuyenMais?
                .FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);
            if (activeKhuyenMai != null)
            {
                hangHoa.GiaBan = hangHoa.GiaGoc - (hangHoa.GiaGoc * (activeKhuyenMai.PhanTramGiamGia / 100m));
            }
        }

        return hangHoas;
    }

    public IEnumerable<DanhMuc> GetDanhMucs()
    {
        return _context.DanhMucs
            .Where(d => d.TenDanhMuc != "Đã xóa") // Chỉ lấy các danh mục chưa bị xóa
            .ToList();
    }

    public HangHoa GetHangHoa(int maHangHoa)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .Include(h => h.KhuyenMais)
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);

        if (hangHoa != null)
        {
            // Recalculate GiaBan based on active promotions
            var currentDate = DateTime.Now;
            var activeKhuyenMai = hangHoa.KhuyenMais?
                .FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);
            if (activeKhuyenMai != null)
            {
                hangHoa.GiaBan = hangHoa.GiaGoc - (hangHoa.GiaGoc * (activeKhuyenMai.PhanTramGiamGia / 100m));
            }
        }

        return hangHoa;
    }

    public HangHoa AddHangHoa(HangHoa hangHoa, List<int> danhMucIds, IFormFile HinhAnh)
    {
        // Validate: Check for duplicate product name
        if (_context.HangHoas.Any(h => h.TenHangHoa == hangHoa.TenHangHoa))
        {
            throw new Exception("Sản phẩm với tên này đã tồn tại.");
        }

        // Handle image upload
        if (HinhAnh != null && HinhAnh.Length > 0)
        {
            // Validate file type (e.g., allow only images)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(HinhAnh.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception("Chỉ chấp nhận các định dạng ảnh: .jpg, .jpeg, .png, .gif.");
            }

            // Use a unique file name to avoid overwriting
            var fileName = Guid.NewGuid().ToString() + extension;
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

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

        // Set initial GiaBan to GiaGoc
        hangHoa.GiaBan = hangHoa.GiaGoc;

        // Add the product to the database
        _context.HangHoas.Add(hangHoa);
        _context.SaveChanges();

        // Associate with categories
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
        var existingHangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .Include(h => h.KhuyenMais)
            .FirstOrDefault(h => h.MaHangHoa == hangHoa.MaHangHoa);

        if (existingHangHoa == null)
        {
            throw new Exception("Sản phẩm không tồn tại.");
        }

        // Validate: Check for duplicate product name (excluding the current product)
        if (_context.HangHoas.Any(h => h.TenHangHoa == hangHoa.TenHangHoa && h.MaHangHoa != hangHoa.MaHangHoa))
        {
            throw new Exception("Sản phẩm với tên này đã tồn tại.");
        }

        // Update properties
        existingHangHoa.TenHangHoa = hangHoa.TenHangHoa;
        existingHangHoa.GiaGoc = hangHoa.GiaGoc;
        existingHangHoa.GiaBan = hangHoa.GiaBan; // GiaBan may have been updated by a discount
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

        // Check if the category is already associated
        if (hangHoa.HangHoaDanhMucs.Any(dm => dm.MaDanhMuc == maDanhMuc))
        {
            throw new Exception("Danh mục này đã tồn tại trong hàng hóa.");
        }

        var newDanhMuc = new HangHoaDanhMuc
        {
            MaHangHoa = maHangHoa,
            MaDanhMuc = maDanhMuc
        };

        _context.HangHoaDanhMucs.Add(newDanhMuc);
        _context.SaveChanges();

        // Reload the categories to ensure the latest data
        _context.Entry(hangHoa).Collection(h => h.HangHoaDanhMucs).Load();

        return hangHoa;
    }

    public HangHoa XoaDanhMuc(int maHangHoa, int maDanhMuc)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);

        if (hangHoa == null)
        {
            throw new Exception($"Không tìm thấy hàng hóa với ID: {maHangHoa}");
        }

        var danhMucAssociation = hangHoa.HangHoaDanhMucs
            .FirstOrDefault(dm => dm.MaDanhMuc == maDanhMuc);
        if (danhMucAssociation == null)
        {
            throw new Exception("Danh mục này không tồn tại trong hàng hóa.");
        }

        _context.HangHoaDanhMucs.Remove(danhMucAssociation);
        _context.SaveChanges();

        // Reload the categories to ensure the latest data
        _context.Entry(hangHoa).Collection(h => h.HangHoaDanhMucs).Load();

        return hangHoa;
    }

    public HangHoa DeleteHangHoa(int maHangHoa)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.HangHoaDanhMucs)
            .Include(h => h.KhuyenMais)
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);

        if (hangHoa != null)
        {
            // Soft delete: Change the product name to "Đã xóa"
            hangHoa.TenHangHoa = "Đã xóa";
            _context.HangHoas.Update(hangHoa);
            _context.SaveChanges();
        }
        return hangHoa;
    }

    public void ThemGiamGia(int maHangHoa, decimal phanTramGiamGia, DateTime ngayBatDau, DateTime ngayKetThuc)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.KhuyenMais)
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);
        if (hangHoa == null)
        {
            throw new Exception("Sản phẩm không tồn tại.");
        }

        // Check for overlapping promotions
        var existingKhuyenMai = hangHoa.KhuyenMais
            .FirstOrDefault(km => (ngayBatDau <= km.NgayKetThuc && ngayKetThuc >= km.NgayBatDau));
        if (existingKhuyenMai != null)
        {
            throw new Exception("Sản phẩm đã có khuyến mãi trong khoảng thời gian này.");
        }

        // Create the promotion record
        var khuyenMai = new KhuyenMai
        {
            MaHangHoa = maHangHoa,
            PhanTramGiamGia = phanTramGiamGia,
            NgayBatDau = ngayBatDau,
            NgayKetThuc = ngayKetThuc
        };
        _context.KhuyenMais.Add(khuyenMai);

        // Update GiaBan only if the promotion is currently active
        var currentDate = DateTime.Now;
        if (ngayBatDau <= currentDate && ngayKetThuc >= currentDate)
        {
            hangHoa.GiaBan = hangHoa.GiaGoc - (hangHoa.GiaGoc * (phanTramGiamGia / 100m));
            _context.HangHoas.Update(hangHoa);
        }

        _context.SaveChanges();
    }

    public void XoaGiamGia(int maHangHoa)
    {
        var hangHoa = _context.HangHoas
            .Include(h => h.KhuyenMais)
            .FirstOrDefault(h => h.MaHangHoa == maHangHoa);
        if (hangHoa == null)
        {
            throw new Exception("Sản phẩm không tồn tại.");
        }

        // Remove only the active promotion (if any)
        var currentDate = DateTime.Now;
        var activeKhuyenMai = hangHoa.KhuyenMais
            .FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);
        if (activeKhuyenMai != null)
        {
            _context.KhuyenMais.Remove(activeKhuyenMai);

            // Reset GiaBan to GiaGoc since the active promotion is removed
            hangHoa.GiaBan = hangHoa.GiaGoc;
            _context.HangHoas.Update(hangHoa);
        }

        _context.SaveChanges();
    }

    public IEnumerable<HangHoa> GetHangHoasFiltered(string search, string category, string priceRange, string discount, string stock, string sortBy)
    {
        var query = _context.HangHoas
            .Include(hh => hh.HangHoaDanhMucs)
            .ThenInclude(hdm => hdm.DanhMuc)
            .Include(hh => hh.KhuyenMais)
            .Where(hh => hh.TenHangHoa != "Đã xóa") // Lọc bỏ các sản phẩm đã soft delete
            .AsQueryable();

        // Apply GiaBan calculation based on active promotions
        var currentDate = DateTime.Now;
        foreach (var hh in query)
        {
            var activeKhuyenMai = hh.KhuyenMais?
                .FirstOrDefault(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate);
            if (activeKhuyenMai != null)
            {
                hh.GiaBan = hh.GiaGoc - (hh.GiaGoc * (activeKhuyenMai.PhanTramGiamGia / 100m));
            }
        }

        // Filter by search term
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(hh => EF.Functions.Like(hh.TenHangHoa, $"%{search}%"));
        }

        // Filter by category
        if (!string.IsNullOrEmpty(category) && int.TryParse(category, out int categoryId))
        {
            query = query.Where(hh => hh.HangHoaDanhMucs.Any(hdm => hdm.MaDanhMuc == categoryId));
        }

        // Filter by price range
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

        // Filter by discount
        if (!string.IsNullOrEmpty(discount))
        {
            switch (discount)
            {
                case "has-discount":
                    query = query.Where(hh => hh.KhuyenMais.Any(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate));
                    break;
                case "no-discount":
                    query = query.Where(hh => !hh.KhuyenMais.Any(km => km.NgayBatDau <= currentDate && km.NgayKetThuc >= currentDate));
                    break;
            }
        }

        // Filter by stock status
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

        // Apply sorting
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

    public DanhMuc GetDanhMuc(int maDanhMuc)
    {
        return _context.DanhMucs.FirstOrDefault(dm => dm.MaDanhMuc == maDanhMuc);
    }
}