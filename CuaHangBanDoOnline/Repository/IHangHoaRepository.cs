using CuaHangBanDoOnline.Models;
public interface IHangHoaRepository
{
    IEnumerable<HangHoa> GetHangHoas();
    HangHoa GetHangHoa(int maHangHoa);
    HangHoa AddHangHoa(HangHoa hangHoa);
    HangHoa UpdateHangHoa(HangHoa hangHoa);
    HangHoa DeleteHangHoa(int maHangHoa);
}