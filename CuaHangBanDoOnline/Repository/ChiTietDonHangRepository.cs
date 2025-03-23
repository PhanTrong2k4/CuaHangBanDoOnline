using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.EntityFrameworkCore;

public class ChiTietDonHangRepository : IChitietdonhangRepository
{
    private readonly CuaHangDbContext _context;

    public ChiTietDonHangRepository(CuaHangDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ChiTietDonHang> GetChiTietDonHangsByDonHangId(int maDonHang)
    {
        return _context.ChiTietDonHangs
            .Include(ctdh => ctdh.HangHoa)
            .Where(ctdh => ctdh.MaDonHang == maDonHang)
            .ToList();
    }

    public ChiTietDonHang AddChiTietDonHang(ChiTietDonHang chiTietDonHang)
    {
        _context.ChiTietDonHangs.Add(chiTietDonHang);
        _context.SaveChanges();
        return chiTietDonHang;
    }
}
