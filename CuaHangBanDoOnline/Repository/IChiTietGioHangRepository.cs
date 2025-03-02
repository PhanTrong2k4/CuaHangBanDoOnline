using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IChiTietGioHangRepository
    {
        IEnumerable<ChiTietGioHang> GetChiTietGioHangs();
        ChiTietDonHang GetChiTietGioHang(int maChiTietGioHang);
        ChiTietDonHang AddChiTietGioHang(ChiTietGioHang chiTietGioHang);
        ChiTietDonHang UpdateChiTietGioHang(ChiTietGioHang chiTietGioHang);
        ChiTietDonHang DeleteChiTietGioHang(int maChiTietGioHang);
    }
}
