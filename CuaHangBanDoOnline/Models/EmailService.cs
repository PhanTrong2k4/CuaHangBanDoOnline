using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using CuaHangBanDoOnline.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CuaHangBanDoOnline.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendConfirmationEmail(string email, string token)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : "https://localhost:44375"; // Fallback
            var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?token={token}";

            await SendEmailAsync(email, "Xác nhận Email",
                $"Vui lòng nhấp vào liên kết sau để xác nhận email: <a href='{confirmationLink}'>{confirmationLink}</a>");
        }

        public async Task SendResetPasswordEmail(string email, string resetLink)
        {
            await SendEmailAsync(email, "Đặt lại mật khẩu",
                $"Vui lòng nhấp vào liên kết sau để đặt lại mật khẩu của bạn: <a href='{resetLink}'>{resetLink}</a>");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromPassword = _configuration["EmailSettings:FromPassword"];
            var fromName = _configuration["EmailSettings:FromName"] ?? "Cửa hàng Bán Đồ Online";
            var host = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(_configuration["EmailSettings:Port"], out int parsedPort) ? parsedPort : 587;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, fromPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}