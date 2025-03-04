using CuaHangBanDoOnline.Models;
using Microsoft.AspNetCore.Http;

public interface IHangHoaRepository
{
    IEnumerable<HangHoa> GetHangHoas();
    IEnumerable<DanhMuc> GetDanhMucs();
    HangHoa GetHangHoa(int maHangHoa);
    HangHoa AddHangHoa(HangHoa hangHoa, List<int> danhMucIds, IFormFile HinhAnh);
    HangHoa UpdateHangHoa(HangHoa hangHoa);
    HangHoa ThemDanhMuc(int maHangHoa, int maDanhMuc);
    HangHoa DeleteHangHoa(int maHangHoa);
}
