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
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<HangHoaDanhMuc> HangHoaDanhMucs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HangHoaDanhMuc>()
    .HasKey(hhdm => new { hhdm.MaHangHoa, hhdm.MaDanhMuc });
            modelBuilder.Entity<HangHoaDanhMuc>()
                .HasOne(hhdm => hhdm.DanhMuc)
                .WithMany(dm => dm.HangHoaDanhMucs)
                .HasForeignKey(hhdm => hhdm.MaDanhMuc)
                .OnDelete(DeleteBehavior.Cascade);
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
            // Cấu hình User và UserProfile (Quan hệ 1-1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.User)
                .HasForeignKey<UserProfile>(up => up.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Cấu hình Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.MaVaiTro);
                entity.Property(r => r.TenVaiTro).IsRequired();
            });

            // Seed dữ liệu vai trò
            modelBuilder.Entity<Role>().HasData(
                new Role { MaVaiTro = 1, TenVaiTro = "Admin" },
                new Role { MaVaiTro = 2, TenVaiTro = "Staff" },
                new Role { MaVaiTro = 3, TenVaiTro = "Customer" }
            );

            // Cấu hình DonHang -> NguoiDung
            modelBuilder.Entity<DonHang>()
    .HasOne(dh => dh.NguoiDung)
    .WithMany(nd => nd.DonHangs)
    .HasForeignKey(dh => dh.MaNguoiDung)
    .OnDelete(DeleteBehavior.Restrict);

           

            // Cấu hình ThanhToan
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasKey(tt => tt.MaThanhToan);
                entity.HasOne(tt => tt.DonHang)
                      .WithMany()
                      .HasForeignKey(tt => tt.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // Cấu hình ThanhToan -> DonHang
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasOne(tt => tt.DonHang)
                      .WithMany()
                      .HasForeignKey(tt => tt.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.HasKey(ctdh => ctdh.MaChiTietDonHang);
                entity.HasOne(ctdh => ctdh.DonHang)
                      .WithMany(dh => dh.ChiTietDonHangs)
                      .HasForeignKey(ctdh => ctdh.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ctdh => ctdh.HangHoa)
                      .WithMany()
                      .HasForeignKey(ctdh => ctdh.MaHangHoa)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(ctdh => ctdh.GiaBan).HasColumnType("decimal(18,2)");
            });

            // Cấu hình GioHang
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasKey(gh => gh.MaGioHang);
                entity.HasOne(gh => gh.NguoiDung)
                      .WithMany()
                      .HasForeignKey(gh => gh.MaNguoiDung)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình ChiTietGioHang
            modelBuilder.Entity<ChiTietGioHang>(entity =>
            {
                entity.HasKey(ctgh => ctgh.MaChiTietGioHang);
                entity.HasOne(ctgh => ctgh.GioHang)
                      .WithMany(gh => gh.ChiTietGioHangs)
                      .HasForeignKey(ctgh => ctgh.MaGioHang)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ctgh => ctgh.HangHoa)
                      .WithMany()
                      .HasForeignKey(ctgh => ctgh.MaHangHoa)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình HangHoa
            modelBuilder.Entity<HangHoa>(entity =>
            {
                entity.HasKey(hh => hh.MaHangHoa);
            });

            // Cấu hình DanhMuc
            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.HasKey(dm => dm.MaDanhMuc);
                entity.Property(dm => dm.TenDanhMuc).IsRequired().HasMaxLength(255);
                entity.Property(dm => dm.MoTa).HasMaxLength(500);
            });

            // Cấu hình HangHoaDanhMuc (Quan hệ nhiều-nhiều giữa HangHoa và DanhMuc)
            modelBuilder.Entity<HangHoaDanhMuc>()
                .HasKey(hhdm => new { hhdm.MaHangHoa, hhdm.MaDanhMuc });
            modelBuilder.Entity<HangHoaDanhMuc>()
                .HasOne(hhdm => hhdm.HangHoa)
                .WithMany(hh => hh.HangHoaDanhMucs)
                .HasForeignKey(hhdm => hhdm.MaHangHoa)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HangHoaDanhMuc>()
                .HasOne(hhdm => hhdm.DanhMuc)
                .WithMany(dm => dm.HangHoaDanhMucs)
                .HasForeignKey(hhdm => hhdm.MaDanhMuc)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình Wishlist
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.HangHoa)
                .WithMany(h => h.Wishlists) // Assuming HangHoa has a collection of Wishlists
                .HasForeignKey(w => w.MaHangHoa)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure Wishlist -> NguoiDung relationship
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.NguoiDung)
                .WithMany(n => n.Wishlists) // Assuming NguoiDung has a collection of Wishlists
                .HasForeignKey(w => w.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade); // Optio

            // Cấu hình KhuyenMai
            modelBuilder.Entity<KhuyenMai>()
                .HasOne(km => km.HangHoa)
                .WithMany()
                .HasForeignKey(km => km.MaHangHoa);

            // Cấu hình Slide và PageContent (không có quan hệ đặc biệt)
            modelBuilder.Entity<Slide>(entity =>
            {
                entity.HasKey(s => s.Id); // Giả sử Slide có khóa chính là Id
            });

            modelBuilder.Entity<PageContent>(entity =>
            {
                entity.HasKey(pc => pc.Id); // Giả sử PageContent có khóa chính là Id
            });
            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.HasKey(e => e.MaDanhMuc); // Xác định khóa chính
            });
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasKey(e => e.MaThanhToan); // Xác định khóa chính
            });
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.MaDonHang); // Xác định khóa chính
            });
        }
    }
}