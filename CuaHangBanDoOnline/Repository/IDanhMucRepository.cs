using CuaHangBanDoOnline.Models;
namespace CuaHangBanDoOnline.Repository
{
    public interface IDanhMucRepository
    {
        IEnumerable<DanhMuc> GetDanhMucs();
        DanhMuc GetDanhMuc(int maDanhMuc);
        DanhMuc AddDanhMuc(DanhMuc danhMuc);
        DanhMuc UpdateDanhMuc(DanhMuc danhMuc);
        DanhMuc DeleteDanhMuc(int maDanhMuc);
    }
}
