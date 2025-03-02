using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IThanhToanRepository
    {
        IEnumerable<ThanhToan> GetThanhToans();
        ThanhToan GetThanhToan(int maThanhToan);
        ThanhToan AddThanhToan(ThanhToan thanhToan);
        ThanhToan UpdateThanhToan(ThanhToan thanhToan);
        ThanhToan DeleteThanhToan(int maThanhToan);
    }
}
