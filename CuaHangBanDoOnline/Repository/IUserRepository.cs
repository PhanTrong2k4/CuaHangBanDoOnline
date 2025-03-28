using CuaHangBanDoOnline.Models;

namespace CuaHangBanDoOnline.Repository
{
    public interface IUserRepository
    {
        User AddUser(User user);
        User GetUserByEmailOrUsername(string input);
        void ConfirmEmail(string token);
        bool EmailExists(string email);
        bool UserExists(string tenDangNhap);
        void AddPasswordResetToken(int maNguoiDung, string token, DateTime expires);
        User GetUserByResetToken(string token);
        void UpdateUser(User user);
        PasswordResetToken GetResetToken(string token);
        void UpdateResetToken(PasswordResetToken resetToken);
        UserProfile GetUserProfile(int maNguoiDung); 
        void UpdateUserProfile(UserProfile profile); 
        User GetUserByConfirmationToken(string token);
        User GetUserById(int maNguoiDung); 
        IEnumerable<User> GetAllUsers(); 
    }
}