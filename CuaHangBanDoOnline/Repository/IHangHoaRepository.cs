﻿using CuaHangBanDoOnline.Models;
using Microsoft.AspNetCore.Http;

public interface IHangHoaRepository
{
    IEnumerable<HangHoa> GetHangHoas();
    IEnumerable<HangHoa> GetHangHoasFiltered(string search, string category, string priceRange, string discount, string stock, string sortBy);
    IEnumerable<DanhMuc> GetDanhMucs();
    HangHoa GetHangHoa(int maHangHoa);
    HangHoa AddHangHoa(HangHoa hangHoa, List<int> danhMucIds, IFormFile HinhAnh);
    HangHoa UpdateHangHoa(HangHoa hangHoa);
    HangHoa ThemDanhMuc(int maHangHoa, int maDanhMuc);
    HangHoa DeleteHangHoa(int maHangHoa);
    void ThemGiamGia(int maHangHoa, decimal phanTramGiamGia, DateTime ngayBatDau, DateTime ngayKetThuc);
    void XoaGiamGia(int maHangHoa);
}