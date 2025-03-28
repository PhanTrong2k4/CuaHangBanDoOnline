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
        ThanhToan ThanhToanDonHang(int maDonHang, string phuongThucThanhToan, decimal soTien); // Thêm phương thức thanh toán
        List<ThanhToan> ThanhToanDonHangWithKeyGames(int maDonHang, List<(int MaHangHoa, decimal GiaBan, int SoLuong)> chiTietDonHangs, string phuongThucThanhToan);    
    }
}