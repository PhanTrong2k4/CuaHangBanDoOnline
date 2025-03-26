using CuaHangBanDoOnline.Models;

namespace CuaHangBanDoOnline.Repository
{
    public interface IChitietdonhangRepository
    {
        IEnumerable<ChiTietDonHang> GetChiTietDonHangsByDonHangId(int maDonHang);
        ChiTietDonHang AddChiTietDonHang(ChiTietDonHang chiTietDonHang);
        IEnumerable<ChiTietDonHang> GetAllChiTietDonHangs(); // Thêm phương thức mới
    }
}