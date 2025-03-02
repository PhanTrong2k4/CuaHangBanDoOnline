using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IHoadonRepository
    {
        IEnumerable<HoaDon> GetHoadons();
        HoaDon GetHoadon(int maHoadon);
        HoaDon AddHoadon(HoaDon hoadon);
        HoaDon UpdateHoadon(HoaDon hoadon);
        HoaDon DeleteHoadon(int maHoadon);
    }
}
