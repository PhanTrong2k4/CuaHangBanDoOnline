using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Repository
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly CuaHangDbContext _context;

        public ManagerRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.NguoiDungs
            .Include(u => u.VaiTro)
            .Include(u => u.UserProfile) // Thêm include UserProfiles
            .ToList();
        }

        public User GetUserById(int maNguoiDung)
        {
            return _context.NguoiDungs
                .Include(u => u.VaiTro)
                .FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);
        }

        public void UpdateUser(User user)
        {
            var existingUser = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == user.MaNguoiDung);
            if (existingUser != null && existingUser.MaVaiTro != (int)RoleType.Admin)
            {
                existingUser.TenDangNhap = user.TenDangNhap;
                existingUser.Email = user.Email;
                existingUser.MatKhau = user.MatKhau;
                existingUser.MaVaiTro = user.MaVaiTro;
                _context.NguoiDungs.Update(existingUser);
                _context.SaveChanges();
            }
        }

        public void DeleteUser(int maNguoiDung)
        {
            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);
            if (user != null && user.MaVaiTro != (int)RoleType.Admin)
            {
                _context.NguoiDungs.Remove(user);
                _context.SaveChanges();
            }
        }

        public void ChangeUserRole(int maNguoiDung, int newRoleId)
        {
            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);
            if (user != null && user.MaVaiTro != (int)RoleType.Admin)
            {
                if (newRoleId == (int)RoleType.Customer || newRoleId == (int)RoleType.Staff)
                {
                    user.MaVaiTro = newRoleId;
                    _context.NguoiDungs.Update(user);
                    _context.SaveChanges();
                }
            }
        }

        public IEnumerable<User> SearchUsersByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return GetAllUsers();
            }

            return _context.NguoiDungs
                .Include(u => u.VaiTro)
                .Where(u => u.Email.ToLower().Contains(email.ToLower())) // Sử dụng ToLower()
                .ToList();
        }
    }
}