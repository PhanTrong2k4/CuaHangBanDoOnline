namespace CuaHangBanDoOnline.Models
{
    // Lớp cơ sở trừu tượng Role (Vai trò)
    public abstract class Role
    {
        public int MaVaiTro { get; set; } // Khóa chính
        public string TenVaiTro { get; set; } = string.Empty; // Tên vai trò (ví dụ: "Chủ cửa hàng", "Quản lý")
    }

    // Vai trò Chủ cửa hàng
    public class VaiTroChuCuaHang : Role
    {
        public string TenCuaHang { get; set; } = string.Empty; // Tên cửa hàng (dành riêng cho chủ cửa hàng)
        public string DiaChiCuaHang { get; set; } = string.Empty; // Địa chỉ cửa hàng
    }

    // Vai trò Quản lý
    public class VaiTroQuanLy : Role
    {
        public string PhongBan { get; set; } = string.Empty; // Phòng ban (dành riêng cho quản lý)
    }

    // Vai trò Nhân viên
    public class VaiTroNhanVien : Role
    {
        public DateTime NgayBatDau { get; set; } // Ngày bắt đầu làm việc (dành riêng cho nhân viên)
        public string ViTriCongViec { get; set; } = string.Empty; // Vị trí công việc
    }

    // Vai trò Khách hàng
    public class VaiTroKhachHang : Role
    {
        public string CapDoHoiVien { get; set; } = string.Empty; // Cấp độ hội viên (ví dụ: Vàng, Bạc)
        public int DiemThuong { get; set; } // Điểm thưởng cho khách hàng
    }
}
