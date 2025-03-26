using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CuaHangBanDoOnline.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CuaHangDbContext _context;

        public UserRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public User AddUser(User user)
        {
            _context.NguoiDungs.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User GetUserByEmailOrUsername(string input)
        {
            return _context.NguoiDungs
                .Include(u => u.VaiTro)
                .FirstOrDefault(u => u.TenDangNhap == input || u.Email == input);
        }

        public void ConfirmEmail(string token)
        {
            var user = GetUserByConfirmationToken(token);
            if (user != null)
            {
                user.IsEmailConfirmed = true;
                user.EmailConfirmationToken = null;
                _context.SaveChanges();
            }
        }

        public bool EmailExists(string email)
        {
            return _context.NguoiDungs.Any(u => u.Email == email);
        }

        public bool UserExists(string tenDangNhap)
        {
            return _context.NguoiDungs.Any(u => u.TenDangNhap == tenDangNhap);
        }

        public void AddPasswordResetToken(int maNguoiDung, string token, DateTime expires)
        {
            var resetToken = new PasswordResetToken
            {
                MaNguoiDung = maNguoiDung,
                Token = token,
                Expires = expires,
                IsUsed = false
            };
            _context.PasswordResetTokens.Add(resetToken);
            _context.SaveChanges();
        }

        public User GetUserByResetToken(string token)
        {
            var resetToken = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == token && !t.IsUsed && t.Expires > DateTime.UtcNow);
            return resetToken != null ? _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == resetToken.MaNguoiDung) : null;
        }

        public void UpdateUser(User user)
        {
            _context.NguoiDungs.Update(user);
            _context.SaveChanges();
        }

        public PasswordResetToken GetResetToken(string token)
        {
            return _context.PasswordResetTokens.FirstOrDefault(t => t.Token == token);
        }

        public void UpdateResetToken(PasswordResetToken resetToken)
        {
            _context.PasswordResetTokens.Update(resetToken);
            _context.SaveChanges();
        }

        public UserProfile GetUserProfile(int maNguoiDung)
        {
            var connection = _context.Database.GetDbConnection();
            bool tableExists = false;

            // Mở kết nối nếu chưa mở
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            try
            {
                tableExists = connection
                    .GetSchema("Tables")
                    .Rows
                    .Cast<DataRow>()
                    .Any(row => row["TABLE_NAME"].ToString() == "UserProfiles");
            }
            finally
            {
                // Đóng kết nối sau khi dùng xong
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            if (tableExists)
            {
                return _context.UserProfiles.FirstOrDefault(p => p.MaNguoiDung == maNguoiDung)
                       ?? new UserProfile { MaNguoiDung = maNguoiDung };
            }
            else
            {
                return new UserProfile { MaNguoiDung = maNguoiDung };
            }
        }

        public void UpdateUserProfile(UserProfile profile)
        {
            var existingProfile = _context.UserProfiles.FirstOrDefault(p => p.Id == profile.Id);
            if (existingProfile == null)
            {
                profile.AvatarPath ??= "/images/default-avatar.png";
                _context.UserProfiles.Add(profile);
            }
            else
            {
                existingProfile.FullName = profile.FullName;
                existingProfile.SoDienThoai = profile.SoDienThoai;
                existingProfile.AvatarPath = profile.AvatarPath ?? existingProfile.AvatarPath ?? "/images/default-avatar.png";
                _context.Entry(existingProfile).State = EntityState.Modified;
            }
            _context.SaveChanges();
        }

        public User GetUserByConfirmationToken(string token)
        {
            return _context.NguoiDungs.FirstOrDefault(u => u.EmailConfirmationToken == token);
        }
        public User GetUserById(int maNguoiDung)
        {
            return _context.NguoiDungs
                .Include(u => u.VaiTro)
                .Include(u => u.UserProfile) // Include UserProfile
                .FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.NguoiDungs
                .Include(u => u.VaiTro)
                .Include(u => u.UserProfile) // Include UserProfile nếu cần
                .ToList();
        }
    }
}