using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IChitietdonhangRepository
    {
        IEnumerable<ChiTietDonHang> GetChitietdonhangs();
        ChiTietDonHang GetChitietdonhang(int maChitietdonhang);
        ChiTietDonHang AddChitietdonhang(ChiTietDonHang chitietdonhang);
        ChiTietDonHang UpdateChitietdonhang(ChiTietDonHang chitietdonhang);
    }
}
