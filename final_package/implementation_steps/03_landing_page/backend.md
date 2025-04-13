# Hướng dẫn triển khai backend trang chủ (Landing Page)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai phần backend của trang chủ (Landing Page) cho hệ thống quản lý sức khỏe cá nhân. Phần backend của trang chủ khá đơn giản vì chủ yếu là hiển thị nội dung tĩnh, nhưng vẫn cần một số xử lý để đảm bảo trang hoạt động chính xác.

## Các bước triển khai

### 1. Tạo HomeController

Đầu tiên, cần tạo controller cho trang chủ. Controller này sẽ xử lý các request đến trang chủ và các trang liên quan như Contact, Privacy, v.v.

```csharp
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Models;
using System.Diagnostics;

namespace DoAnChamSocSucKhoe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Xử lý gửi email liên hệ
                // Có thể sử dụng dịch vụ email đã cấu hình trong hệ thống
                
                // Thông báo thành công
                TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ với chúng tôi. Chúng tôi sẽ phản hồi trong thời gian sớm nhất.";
                return RedirectToAction(nameof(Contact));
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
```

### 2. Tạo ContactViewModel

Tạo model cho form liên hệ:

```csharp
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [Display(Name = "Tiêu đề")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [Display(Name = "Nội dung")]
        public string Message { get; set; }
    }
}
```

### 3. Cấu hình Route

Cấu hình route cho trang chủ trong file `Program.cs`:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### 4. Tạo dịch vụ gửi email (nếu cần)

Nếu cần xử lý form liên hệ, tạo dịch vụ gửi email:

```csharp
// Services/EmailService.cs
using System.Net;
using System.Net.Mail;

namespace DoAnChamSocSucKhoe.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            using (var client = new SmtpClient(smtpSettings["Server"], int.Parse(smtpSettings["Port"])))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);
                client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
```

### 5. Đăng ký dịch vụ gửi email trong Program.cs

```csharp
// Đăng ký dịch vụ gửi email
builder.Services.AddTransient<IEmailService, EmailService>();
```

### 6. Cấu hình SMTP trong appsettings.json

```json
"SmtpSettings": {
  "Server": "smtp.gmail.com",
  "Port": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "Health Manager",
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "EnableSsl": true
}
```

### 7. Cập nhật HomeController để sử dụng dịch vụ gửi email

```csharp
private readonly IEmailService _emailService;

public HomeController(ILogger<HomeController> logger, IEmailService emailService)
{
    _logger = logger;
    _emailService = emailService;
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Contact(ContactViewModel model)
{
    if (ModelState.IsValid)
    {
        // Xây dựng nội dung email
        var message = $"<h3>Thông tin liên hệ mới</h3>" +
                      $"<p><strong>Họ tên:</strong> {model.Name}</p>" +
                      $"<p><strong>Email:</strong> {model.Email}</p>" +
                      $"<p><strong>Số điện thoại:</strong> {model.Phone}</p>" +
                      $"<p><strong>Tiêu đề:</strong> {model.Subject}</p>" +
                      $"<p><strong>Nội dung:</strong> {model.Message}</p>";

        // Gửi email
        await _emailService.SendEmailAsync("admin@healthmanager.com", "Liên hệ mới từ website", message);

        // Thông báo thành công
        TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ với chúng tôi. Chúng tôi sẽ phản hồi trong thời gian sớm nhất.";
        return RedirectToAction(nameof(Contact));
    }
    return View(model);
}
```

## Tích hợp với frontend

Các controller và service này sẽ tương tác với frontend thông qua các view. Xem chi tiết trong [Hướng dẫn triển khai frontend](frontend.md).

## Lưu ý

- Đảm bảo cấu hình SMTP chính xác để chức năng gửi email hoạt động
- Xử lý ngoại lệ khi gửi email để tránh lỗi ứng dụng
- Sử dụng TempData để hiển thị thông báo sau khi chuyển hướng
- Đảm bảo bảo mật thông tin người dùng khi xử lý form liên hệ
- Cân nhắc sử dụng reCAPTCHA để tránh spam từ form liên hệ
