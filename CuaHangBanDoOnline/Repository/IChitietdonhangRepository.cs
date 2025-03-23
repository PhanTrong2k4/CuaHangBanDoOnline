using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IChitietdonhangRepository
    {
        IEnumerable<ChiTietDonHang> GetChiTietDonHangsByDonHangId(int maDonHang);
        ChiTietDonHang AddChiTietDonHang(ChiTietDonHang chiTietDonHang);
    }
}
