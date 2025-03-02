using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IGioHangRepository
    {
        IEnumerable<GioHang> GetGioHangs();
        GioHang GetGioHang(int maGioHang);
        GioHang AddGioHang(GioHang gioHang);
        GioHang UpdateGioHang(GioHang gioHang);
        GioHang DeleteGioHang(int maGioHang);
    }
}
