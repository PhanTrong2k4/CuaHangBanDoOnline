using CuaHangBanDoOnline.Models;

namespace CuaHangBanDoOnline.Repository
{
    public interface IManagerRepository
    {
        IEnumerable<User> GetAllUsers(); // Lấy danh sách tất cả người dùng
        User GetUserById(int maNguoiDung); // Lấy thông tin người dùng theo ID
        void UpdateUser(User user); // Cập nhật thông tin người dùng
        void DeleteUser(int maNguoiDung); // Xóa người dùng
        void ChangeUserRole(int maNguoiDung, int newRoleId); // Thay đổi vai trò người dùng
        IEnumerable<User> SearchUsersByEmail(string email);
    }
}