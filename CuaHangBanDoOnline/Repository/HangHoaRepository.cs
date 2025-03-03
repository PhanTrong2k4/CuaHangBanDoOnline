using CuaHangBanDoOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class HangHoaRepository : IHangHoaRepository
{
    private readonly CuaHangDbContext _context;

    public HangHoaRepository(CuaHangDbContext context)
    {
        _context = context;
    }

    public IEnumerable<HangHoa> GetHangHoas()
    {
        return _context.HangHoas.ToList();
    }

    public HangHoa GetHangHoa(int maHangHoa)
    {
        return _context.HangHoas.Find(maHangHoa);
    }

    public HangHoa AddHangHoa(HangHoa hangHoa)
    {
        _context.HangHoas.Add(hangHoa);
        _context.SaveChanges();
        return hangHoa;
    }

    public HangHoa UpdateHangHoa(HangHoa hangHoa)
    {
        _context.HangHoas.Update(hangHoa);
        _context.SaveChanges();
        return hangHoa;
    }

    public HangHoa DeleteHangHoa(int maHangHoa)
    {
        var hangHoa = _context.HangHoas.Find(maHangHoa);
        if (hangHoa != null)
        {
            hangHoa.TenHangHoa = "Đã xóa";
            _context.SaveChanges();
        }
        return hangHoa;
    }
}
