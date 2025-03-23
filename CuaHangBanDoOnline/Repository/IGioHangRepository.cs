using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IGioHangRepository
    {
        GioHang GetGioHangByUserId(int maNguoiDung);
        GioHang CreateGioHang(int maNguoiDung);
        void UpdateGioHang(GioHang gioHang);
        void DeleteGioHang(int maGioHang);
    }
}
