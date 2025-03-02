using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IDonHangRepository
    {
        IEnumerable<DonHang> GetDonHangs();
        DonHang GetDonHang(int maDonHang);
        DonHang AddDonHang(DonHang donHang);
        DonHang UpdateDonHang(DonHang donHang);
        DonHang DeleteDonHang(int maDonHang);
    }
}
