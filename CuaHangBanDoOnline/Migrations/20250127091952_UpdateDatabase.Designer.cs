﻿// <auto-generated />
using System;
using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CuaHangBanDoOnline.Migrations
{
    [DbContext(typeof(CuaHangDbContext))]
    [Migration("20250127091952_UpdateDatabase")]
    partial class UpdateDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CuaHangBanDoOnline.Models.ChiTietDonHang", b =>
                {
                    b.Property<int>("MaChiTietDonHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaChiTietDonHang"));

                    b.Property<decimal>("GiaBan")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MaDonHang")
                        .HasColumnType("int");

                    b.Property<int>("MaHangHoa")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("MaChiTietDonHang");

                    b.HasIndex("MaDonHang");

                    b.HasIndex("MaHangHoa");

                    b.ToTable("ChiTietDonHangs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.ChiTietGioHang", b =>
                {
                    b.Property<int>("MaChiTietGioHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaChiTietGioHang"));

                    b.Property<decimal>("GiaBan")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MaGioHang")
                        .HasColumnType("int");

                    b.Property<int>("MaHangHoa")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("MaChiTietGioHang");

                    b.HasIndex("MaGioHang");

                    b.HasIndex("MaHangHoa");

                    b.ToTable("ChiTietGioHangs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.DanhMuc", b =>
                {
                    b.Property<int>("MaDanhMuc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaDanhMuc"));

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenDanhMuc")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MaDanhMuc");

                    b.ToTable("DanhMucs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.DonHang", b =>
                {
                    b.Property<int>("MaDonHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaDonHang"));

                    b.Property<int>("MaNguoiDung")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayDatHang")
                        .HasColumnType("datetime2");

                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MaDonHang");

                    b.HasIndex("MaNguoiDung");

                    b.ToTable("DonHangs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.GioHang", b =>
                {
                    b.Property<int>("MaGioHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaGioHang"));

                    b.Property<int>("MaNguoiDung")
                        .HasColumnType("int");

                    b.HasKey("MaGioHang");

                    b.HasIndex("MaNguoiDung");

                    b.ToTable("GioHangs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.HangHoa", b =>
                {
                    b.Property<int>("MaHangHoa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaHangHoa"));

                    b.Property<decimal>("GiaBan")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MaDanhMuc")
                        .HasColumnType("int");

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SoLuongTon")
                        .HasColumnType("int");

                    b.Property<string>("TenHangHoa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MaHangHoa");

                    b.HasIndex("MaDanhMuc");

                    b.ToTable("HangHoas");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.HoaDon", b =>
                {
                    b.Property<int>("MaHoaDon")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaHoaDon"));

                    b.Property<int>("MaDonHang")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TongTien")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("MaHoaDon");

                    b.HasIndex("MaDonHang");

                    b.ToTable("HoaDons");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.Role", b =>
                {
                    b.Property<int>("MaVaiTro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaVaiTro"));

                    b.Property<string>("TenVaiTro")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MaVaiTro");

                    b.ToTable("Roles");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.ThanhToan", b =>
                {
                    b.Property<int>("MaThanhToan")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaThanhToan"));

                    b.Property<int>("MaDonHang")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayThanhToan")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhuongThucThanhToan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SoTien")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("MaThanhToan");

                    b.HasIndex("MaDonHang");

                    b.ToTable("ThanhToans");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.User", b =>
                {
                    b.Property<int>("MaNguoiDung")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaNguoiDung"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaVaiTro")
                        .HasColumnType("int");

                    b.Property<string>("MatKhau")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenDangNhap")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MaNguoiDung");

                    b.HasIndex("MaVaiTro");

                    b.ToTable("NguoiDungs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroChuCuaHang", b =>
                {
                    b.HasBaseType("CuaHangBanDoOnline.Models.Role");

                    b.Property<string>("DiaChiCuaHang")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenCuaHang")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("VaiTroChuCuaHang", (string)null);
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroKhachHang", b =>
                {
                    b.HasBaseType("CuaHangBanDoOnline.Models.Role");

                    b.Property<string>("CapDoHoiVien")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DiemThuong")
                        .HasColumnType("int");

                    b.ToTable("VaiTroKhachHang", (string)null);
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroNhanVien", b =>
                {
                    b.HasBaseType("CuaHangBanDoOnline.Models.Role");

                    b.Property<DateTime>("NgayBatDau")
                        .HasColumnType("datetime2");

                    b.Property<string>("ViTriCongViec")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("VaiTroNhanVien", (string)null);
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroQuanLy", b =>
                {
                    b.HasBaseType("CuaHangBanDoOnline.Models.Role");

                    b.Property<string>("PhongBan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("VaiTroQuanLy", (string)null);
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.ChiTietDonHang", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.DonHang", "DonHang")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("MaDonHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CuaHangBanDoOnline.Models.HangHoa", "HangHoa")
                        .WithMany()
                        .HasForeignKey("MaHangHoa")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DonHang");

                    b.Navigation("HangHoa");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.ChiTietGioHang", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.GioHang", "GioHang")
                        .WithMany("ChiTietGioHangs")
                        .HasForeignKey("MaGioHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CuaHangBanDoOnline.Models.HangHoa", "HangHoa")
                        .WithMany()
                        .HasForeignKey("MaHangHoa")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GioHang");

                    b.Navigation("HangHoa");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.DonHang", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.User", "NguoiDung")
                        .WithMany()
                        .HasForeignKey("MaNguoiDung")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NguoiDung");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.GioHang", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.User", "NguoiDung")
                        .WithMany()
                        .HasForeignKey("MaNguoiDung")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NguoiDung");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.HangHoa", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.DanhMuc", "DanhMuc")
                        .WithMany("HangHoas")
                        .HasForeignKey("MaDanhMuc")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DanhMuc");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.HoaDon", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.DonHang", "DonHang")
                        .WithMany()
                        .HasForeignKey("MaDonHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DonHang");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.ThanhToan", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.DonHang", "DonHang")
                        .WithMany()
                        .HasForeignKey("MaDonHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DonHang");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.User", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.Role", "VaiTro")
                        .WithMany()
                        .HasForeignKey("MaVaiTro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VaiTro");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroChuCuaHang", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.Role", null)
                        .WithOne()
                        .HasForeignKey("CuaHangBanDoOnline.Models.VaiTroChuCuaHang", "MaVaiTro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroKhachHang", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.Role", null)
                        .WithOne()
                        .HasForeignKey("CuaHangBanDoOnline.Models.VaiTroKhachHang", "MaVaiTro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroNhanVien", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.Role", null)
                        .WithOne()
                        .HasForeignKey("CuaHangBanDoOnline.Models.VaiTroNhanVien", "MaVaiTro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.VaiTroQuanLy", b =>
                {
                    b.HasOne("CuaHangBanDoOnline.Models.Role", null)
                        .WithOne()
                        .HasForeignKey("CuaHangBanDoOnline.Models.VaiTroQuanLy", "MaVaiTro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.DanhMuc", b =>
                {
                    b.Navigation("HangHoas");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.DonHang", b =>
                {
                    b.Navigation("ChiTietDonHangs");
                });

            modelBuilder.Entity("CuaHangBanDoOnline.Models.GioHang", b =>
                {
                    b.Navigation("ChiTietGioHangs");
                });
#pragma warning restore 612, 618
        }
    }
}
