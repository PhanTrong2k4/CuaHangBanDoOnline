using CuaHangBanDoOnline.Models;
using CuaHangBanDoOnline.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext với DI container
builder.Services.AddDbContext<CuaHangDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IHangHoaRepository,HangHoaRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IThanhToanRepository, ThanhToanRepository>();
builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
builder.Services.AddScoped<IChiTietGioHangRepository, ChiTietGioHangRepository>();
builder.Services.AddScoped<IHangHoaRepository, HangHoaRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IChitietdonhangRepository, ChiTietDonHangRepository>();
builder.Services.AddScoped<IWishlistRepository,  WishlistRepository >();
builder.Services.AddScoped<ISlideRepository, SlideRepository>();



// Thêm dịch vụ cho MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Định tuyến mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
