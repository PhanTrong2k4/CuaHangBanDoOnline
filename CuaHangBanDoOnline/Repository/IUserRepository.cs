using CuaHangBanDoOnline.Models;

namespace CuaHangBanDoOnline.Repository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUser(int maNguoiDung);
        User AddUser(User user);
        User UpdateUser(User user);
        User DeleteUser(int maNguoiDung);
        bool IsUserInRoleAsync(int userId, string roleName);
        User GetUserByUsername(string tenDangNhap); 
        bool UserExists(string tenDangNhap); 
    }
}
