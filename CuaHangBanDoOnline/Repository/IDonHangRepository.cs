using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IDonHangRepository
    {
        DonHang CreateDonHang(DonHang donHang);
        IEnumerable<DonHang> GetDonHangsByUserId(int maNguoiDung);
        DonHang GetDonHang(int maDonHang);
        void UpdateDonHang(DonHang donHang);
        void DeleteDonHang(int maDonHang);
    }
}
