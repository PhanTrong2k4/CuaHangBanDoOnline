using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IChiTietGioHangRepository
    {
        IEnumerable<ChiTietGioHang> GetChiTietGioHangs();
        IEnumerable<ChiTietGioHang> GetChiTietGioHangsByGioHangId(int maGioHang);
        ChiTietGioHang GetChiTietGioHangByHangHoaId(int maGioHang, int maHangHoa);
        ChiTietGioHang GetChiTietGioHang(int maChiTietGioHang);
        ChiTietGioHang AddChiTietGioHang(ChiTietGioHang chiTietGioHang);
        ChiTietGioHang UpdateChiTietGioHang(ChiTietGioHang chiTietGioHang);
        void DeleteChiTietGioHang(int maChiTietGioHang);
    }
}
