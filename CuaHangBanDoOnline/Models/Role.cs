namespace CuaHangBanDoOnline.Models
{
    public class Role
    {
        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; } = string.Empty;
    }

    public enum RoleType
    {
        Admin = 1,
        Staff = 2,
        Customer = 3
    }
}