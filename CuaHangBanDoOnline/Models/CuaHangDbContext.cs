using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Models
{
    public class CuaHangDbContext : DbContext
    {
        public CuaHangDbContext(DbContextOptions<CuaHangDbContext> options) : base(options)
        {
        }

        // DbSet cho các bảng
        public DbSet<User> NguoiDungs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
        .HasOne(u => u.UserProfile)
        .WithOne()
        .HasForeignKey<UserProfile>(up => up.MaNguoiDung);
            modelBuilder.Entity<UserProfile>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.MaNguoiDung);
            // Cấu hình bảng User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.MaNguoiDung); // Khóa chính
                entity.HasOne(u => u.VaiTro)       // Quan hệ 1-n với Role
                      .WithMany()
                      .HasForeignKey(u => u.MaVaiTro)
                      .OnDelete(DeleteBehavior.Restrict); // Không xóa Role khi xóa User

                // Đặt index và unique constraint cho Email và TenDangNhap
                entity.HasIndex(u => u.Email).IsUnique();      // Index duy nhất cho Email
                entity.HasIndex(u => u.TenDangNhap).IsUnique(); // Index duy nhất cho TenDangNhap
            });

            // Cấu hình bảng Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.MaVaiTro); // Khóa chính
                entity.Property(r => r.TenVaiTro).IsRequired(); // Tên vai trò không được null
            });

            // Seed dữ liệu vai trò
            modelBuilder.Entity<Role>().HasData(
                new Role { MaVaiTro = 1, TenVaiTro = "Admin" },
                new Role { MaVaiTro = 2, TenVaiTro = "Staff" },
                new Role { MaVaiTro = 3, TenVaiTro = "Customer" }
            );

            // Cấu hình quan hệ DonHang và NguoiDung
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(dh => dh.MaDonHang); // Khóa chính
                entity.HasOne(dh => dh.NguoiDung)  // Quan hệ 1-n với User
                      .WithMany()
                      .HasForeignKey(dh => dh.MaNguoiDung)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa đơn hàng khi xóa người dùng
            });

            // Cấu hình HoaDon -> DonHang
            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(hd => hd.MaHoaDon); // Khóa chính
                entity.HasOne(hd => hd.DonHang)   // Quan hệ 1-1 với DonHang
                      .WithMany()
                      .HasForeignKey(hd => hd.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa hóa đơn khi xóa đơn hàng
            });

            // Cấu hình ThanhToan -> DonHang
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasKey(tt => tt.MaThanhToan); // Khóa chính
                entity.HasOne(tt => tt.DonHang)      // Quan hệ 1-1 với DonHang
                      .WithMany()
                      .HasForeignKey(tt => tt.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa thanh toán khi xóa đơn hàng
            });

            // Cấu hình ChiTietDonHang -> DonHang và HangHoa
            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.HasKey(ctdh => ctdh.MaChiTietDonHang); // Khóa chính
                entity.HasOne(ctdh => ctdh.DonHang)           // Quan hệ n-1 với DonHang
                      .WithMany(dh => dh.ChiTietDonHangs)
                      .HasForeignKey(ctdh => ctdh.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade);      // Xóa chi tiết khi xóa đơn hàng
                entity.HasOne(ctdh => ctdh.HangHoa)           // Quan hệ n-1 với HangHoa
                      .WithMany()
                      .HasForeignKey(ctdh => ctdh.MaHangHoa)
                      .OnDelete(DeleteBehavior.Cascade);      // Xóa chi tiết khi xóa hàng hóa
                entity.Property(ctdh => ctdh.GiaBan)
                      .HasColumnType("decimal(18,2)");       // Định dạng tiền tệ
            });

            // Cấu hình GioHang -> NguoiDung
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasKey(gh => gh.MaGioHang); // Khóa chính
                entity.HasOne(gh => gh.NguoiDung)  // Quan hệ 1-n với User
                      .WithMany()
                      .HasForeignKey(gh => gh.MaNguoiDung)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa giỏ hàng khi xóa người dùng
            });

            // Cấu hình ChiTietGioHang -> GioHang và HangHoa
            modelBuilder.Entity<ChiTietGioHang>(entity =>
            {
                entity.HasKey(ctgh => ctgh.MaChiTietGioHang); // Khóa chính
                entity.HasOne(ctgh => ctgh.GioHang)           // Quan hệ n-1 với GioHang
                      .WithMany(gh => gh.ChiTietGioHangs)
                      .HasForeignKey(ctgh => ctgh.MaGioHang)
                      .OnDelete(DeleteBehavior.Cascade);      // Xóa chi tiết khi xóa giỏ hàng
                entity.HasOne(ctgh => ctgh.HangHoa)           // Quan hệ n-1 với HangHoa
                      .WithMany()
                      .HasForeignKey(ctgh => ctgh.MaHangHoa)
                      .OnDelete(DeleteBehavior.Cascade);      // Xóa chi tiết khi xóa hàng hóa
            });

            // Cấu hình HangHoa -> DanhMuc
            modelBuilder.Entity<HangHoa>(entity =>
            {
                entity.HasKey(hh => hh.MaHangHoa); // Khóa chính
                entity.HasOne(hh => hh.DanhMuc)    // Quan hệ n-1 với DanhMuc
                      .WithMany(dm => dm.HangHoas)
                      .HasForeignKey(hh => hh.MaDanhMuc)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa hàng hóa khi xóa danh mục
            });

            // Cấu hình DanhMuc
            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.HasKey(dm => dm.MaDanhMuc); // Khóa chính
                entity.Property(dm => dm.TenDanhMuc).IsRequired().HasMaxLength(255); // Tên danh mục bắt buộc
                entity.Property(dm => dm.MoTa).HasMaxLength(500); // Mô tả tối đa 500 ký tự
            });
        }
    }
}