using Microsoft.EntityFrameworkCore;

namespace CuaHangBanDoOnline.Models
{
    public class CuaHangDbContext : DbContext
    {
        public CuaHangDbContext(DbContextOptions<CuaHangDbContext> options) : base(options)
        {
        }

        public DbSet<User> NguoiDungs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<HangHoaDanhMuc> HangHoaDanhMucs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình NguoiDung -> Role
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.MaNguoiDung); // Xác định khóa chính
                entity.HasOne(nd => nd.VaiTro)
                      .WithMany()
                      .HasForeignKey(nd => nd.MaVaiTro)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình DonHang -> NguoiDung
            modelBuilder.Entity<DonHang>()
    .HasOne(dh => dh.NguoiDung)
    .WithMany(nd => nd.DonHangs)
    .HasForeignKey(dh => dh.MaNguoiDung)
    .OnDelete(DeleteBehavior.Restrict);


            // Cấu hình HoaDon -> DonHang
            modelBuilder.Entity<HoaDon>()
     .HasOne(h => h.NguoiDung)
     .WithMany(u => u.HoaDons)
     .HasForeignKey(h => h.MaNguoiDung)
     .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade


            // Cấu hình ThanhToan -> DonHang
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasOne(tt => tt.DonHang)
                      .WithMany()
                      .HasForeignKey(tt => tt.MaDonHang)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình ChiTietDonHang -> DonHang và HangHoa
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

                entity.Property(ctdh => ctdh.GiaBan)
                      .HasColumnType("decimal(18,2)");
            });


            // Cấu hình GioHang -> NguoiDung
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasKey(e => e.MaGioHang); // Xác định khóa chính

                entity.HasOne(gh => gh.NguoiDung) // Quan hệ 1:N với NguoiDung
                      .WithMany()
                      .HasForeignKey(gh => gh.MaNguoiDung)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Cấu hình ChiTietGioHang -> GioHang và HangHoa
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
            modelBuilder.Entity<HangHoaDanhMuc>()
        .HasKey(dmhh => new { dmhh.MaDanhMuc, dmhh.MaHangHoa }); 

            modelBuilder.Entity<HangHoaDanhMuc>()
                .HasOne(dmhh => dmhh.DanhMuc)
                .WithMany(dm => dm.HangHoaDanhMucs)
                .HasForeignKey(dmhh => dmhh.MaDanhMuc)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HangHoaDanhMuc>()
                .HasOne(dmhh => dmhh.HangHoa)
                .WithMany(hh => hh.HangHoaDanhMucs)
                .HasForeignKey(dmhh => dmhh.MaHangHoa)
                .OnDelete(DeleteBehavior.Cascade);

           
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
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.MaVaiTro); // Xác định khóa chính cho Role
                entity.Property(e => e.TenVaiTro).IsRequired(); // Thuộc tính bắt buộc
            });

            // Cấu hình các lớp kế thừa
            modelBuilder.Entity<VaiTroChuCuaHang>().ToTable("VaiTroChuCuaHang");
            modelBuilder.Entity<VaiTroQuanLy>().ToTable("VaiTroQuanLy");
            modelBuilder.Entity<VaiTroNhanVien>().ToTable("VaiTroNhanVien");
            modelBuilder.Entity<VaiTroKhachHang>().ToTable("VaiTroKhachHang");
        }
    }
    
}
