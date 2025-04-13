# Hướng dẫn triển khai backend xác thực (Authentication)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai phần backend của hệ thống xác thực (Authentication) cho hệ thống quản lý sức khỏe cá nhân. Phần backend của hệ thống xác thực bao gồm cấu hình ASP.NET Core Identity, tùy chỉnh model người dùng, và triển khai các dịch vụ liên quan.

## Các bước triển khai

### 1. Cấu hình ASP.NET Core Identity

Cập nhật file `Program.cs` để cấu hình ASP.NET Core Identity:

```csharp
// Thêm dịch vụ Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    // Cấu hình mật khẩu
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Cấu hình khóa tài khoản
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    // Cấu hình xác nhận email
    options.SignIn.RequireConfirmedEmail = true;
    
    // Cấu hình người dùng
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>() // Thêm hỗ trợ vai trò
.AddEntityFrameworkStores<ApplicationDbContext>() // Sử dụng Entity Framework Core
.AddDefaultTokenProviders(); // Thêm hỗ trợ token (cho đặt lại mật khẩu, xác nhận email, v.v.)

// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
```

### 2. Tùy chỉnh ApplicationUser

Tạo file `Models/ApplicationUser.cs` để mở rộng IdentityUser:

```csharp
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [PersonalData]
        [Column("ho_ten")]
        public string HoTen { get; set; }
        
        [PersonalData]
        [Column("gioi_tinh")]
        public string GioiTinh { get; set; }
        
        [PersonalData]
        [Column("ngay_sinh")]
        public DateTime? NgaySinh { get; set; }
        
        [PersonalData]
        [Column("dia_chi")]
        public string DiaChi { get; set; }
        
        [Column("anh_dai_dien")]
        public string ProfilePicture { get; set; }
        
        [Required]
        [Column("vai_tro_id")]
        public int VaiTroId { get; set; }
        
        [Column("ngay_tao")]
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        // Navigation property
        [ForeignKey("VaiTroId")]
        public virtual VaiTro VaiTro { get; set; }
    }
}
```

### 3. Tạo dịch vụ gửi email

Tạo file `Services/EmailSender.cs` để triển khai dịch vụ gửi email:

```csharp
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            using (var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.EnableSsl = _emailSettings.EnableSsl;
                await client.SendMailAsync(mailMessage);
            }
        }
    }

    public class EmailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
```

### 4. Cấu hình dịch vụ gửi email trong Program.cs

```csharp
// Cấu hình dịch vụ gửi email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
```

### 5. Cấu hình email trong appsettings.json

```json
"EmailSettings": {
  "Server": "smtp.gmail.com",
  "Port": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "Health Manager",
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "EnableSsl": true
}
```

### 6. Tùy chỉnh RegisterModel

Cập nhật file `Areas/Identity/Pages/Account/Register.cshtml.cs` để thêm các trường tùy chỉnh:

```csharp
// Thêm vào class InputModel
[Required(ErrorMessage = "Vui lòng nhập họ tên")]
[Display(Name = "Họ tên")]
public string HoTen { get; set; }

[Required(ErrorMessage = "Vui lòng chọn vai trò")]
[Display(Name = "Vai trò")]
public string VaiTro { get; set; }
```

Cập nhật phương thức OnPostAsync để lưu thông tin tùy chỉnh:

```csharp
// Trong phương thức OnPostAsync, sau khi tạo user
var user = CreateUser();
user.HoTen = Input.HoTen;

// Lấy vai trò từ cơ sở dữ liệu
var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == Input.VaiTro);
if (vaiTro != null)
{
    user.VaiTroId = vaiTro.Id;
}

// Tiếp tục với việc tạo user
await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
var result = await _userManager.CreateAsync(user, Input.Password);

if (result.Succeeded)
{
    // Thêm user vào vai trò tương ứng
    await _userManager.AddToRoleAsync(user, Input.VaiTro);
    
    // Tiếp tục với việc gửi email xác nhận, v.v.
}
```

### 7. Tùy chỉnh LoginModel

Cập nhật file `Areas/Identity/Pages/Account/Login.cshtml.cs` để thêm chức năng chuyển hướng theo vai trò:

```csharp
// Trong phương thức OnPostAsync, sau khi đăng nhập thành công
if (result.Succeeded)
{
    _logger.LogInformation("User logged in.");
    
    // Lấy vai trò của người dùng
    var user = await _userManager.FindByEmailAsync(Input.Email);
    var roles = await _userManager.GetRolesAsync(user);
    
    // Chuyển hướng theo vai trò
    if (roles.Contains("Admin"))
    {
        return LocalRedirect("~/Admin/Dashboard");
    }
    else if (roles.Contains("Doctor"))
    {
        return LocalRedirect("~/Doctor/Dashboard");
    }
    else if (roles.Contains("Patient"))
    {
        return LocalRedirect("~/Patient/Dashboard");
    }
    
    return LocalRedirect(returnUrl);
}
```

### 8. Tùy chỉnh ManageModel

Cập nhật file `Areas/Identity/Pages/Account/Manage/Index.cshtml.cs` để thêm các trường tùy chỉnh:

```csharp
// Thêm vào class InputModel
[Display(Name = "Họ tên")]
public string HoTen { get; set; }

[Display(Name = "Giới tính")]
public string GioiTinh { get; set; }

[Display(Name = "Ngày sinh")]
[DataType(DataType.Date)]
public DateTime? NgaySinh { get; set; }

[Display(Name = "Địa chỉ")]
public string DiaChi { get; set; }

[Display(Name = "Ảnh đại diện")]
public string ProfilePicture { get; set; }

[Display(Name = "Ảnh đại diện mới")]
public IFormFile ProfilePictureFile { get; set; }
```

Cập nhật phương thức OnGetAsync để lấy thông tin tùy chỉnh:

```csharp
// Trong phương thức OnGetAsync, sau khi lấy user
Input = new InputModel
{
    PhoneNumber = phoneNumber,
    HoTen = user.HoTen,
    GioiTinh = user.GioiTinh,
    NgaySinh = user.NgaySinh,
    DiaChi = user.DiaChi,
    ProfilePicture = user.ProfilePicture
};
```

Cập nhật phương thức OnPostAsync để lưu thông tin tùy chỉnh:

```csharp
// Trong phương thức OnPostAsync, sau khi lấy user
if (Input.HoTen != user.HoTen)
{
    user.HoTen = Input.HoTen;
    await _userManager.UpdateAsync(user);
}

if (Input.GioiTinh != user.GioiTinh)
{
    user.GioiTinh = Input.GioiTinh;
    await _userManager.UpdateAsync(user);
}

if (Input.NgaySinh != user.NgaySinh)
{
    user.NgaySinh = Input.NgaySinh;
    await _userManager.UpdateAsync(user);
}

if (Input.DiaChi != user.DiaChi)
{
    user.DiaChi = Input.DiaChi;
    await _userManager.UpdateAsync(user);
}

// Xử lý upload ảnh đại diện
if (Input.ProfilePictureFile != null)
{
    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ProfilePictureFile.FileName);
    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles", fileName);
    
    // Tạo thư mục nếu chưa tồn tại
    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
    
    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await Input.ProfilePictureFile.CopyToAsync(fileStream);
    }
    
    // Cập nhật đường dẫn ảnh đại diện
    user.ProfilePicture = "/images/profiles/" + fileName;
    await _userManager.UpdateAsync(user);
}
```

### 9. Tạo Seed Data cho vai trò

Tạo file `Data/SeedData.cs` để khởi tạo dữ liệu vai trò:

```csharp
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Kiểm tra xem đã có dữ liệu trong bảng vai_tro chưa
                if (context.VaiTros.Any())
                {
                    return; // Đã có dữ liệu, không cần seed
                }

                // Tạo các vai trò
                var vaiTros = new VaiTro[]
                {
                    new VaiTro { TenVaiTro = "Admin" },
                    new VaiTro { TenVaiTro = "Doctor" },
                    new VaiTro { TenVaiTro = "Patient" }
                };

                foreach (var vaiTro in vaiTros)
                {
                    context.VaiTros.Add(vaiTro);
                }
                await context.SaveChangesAsync();

                // Tạo các vai trò trong Identity
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("Doctor"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Doctor"));
                }
                if (!await roleManager.RoleExistsAsync("Patient"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Patient"));
                }

                // Tạo tài khoản admin mặc định
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var adminUser = await userManager.FindByEmailAsync("admin@healthmanager.com");
                
                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = "admin@healthmanager.com",
                        Email = "admin@healthmanager.com",
                        EmailConfirmed = true,
                        HoTen = "Admin",
                        VaiTroId = vaiTros.First(v => v.TenVaiTro == "Admin").Id
                    };
                    
                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
        }
    }
}
```

### 10. Cập nhật Program.cs để khởi tạo dữ liệu vai trò

```csharp
// Thêm vào cuối file Program.cs, trước app.Run()
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
```

### 11. Tạo Middleware kiểm tra vai trò

Tạo file `Middleware/RoleMiddleware.cs` để kiểm tra vai trò và chuyển hướng người dùng:

```csharp
using Microsoft.AspNetCore.Identity;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Middleware
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (context.User.Identity.IsAuthenticated)
            {
                // Lấy thông tin người dùng
                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    // Lấy vai trò của người dùng
                    var roles = await userManager.GetRolesAsync(user);
                    
                    // Kiểm tra đường dẫn hiện tại
                    var path = context.Request.Path.Value.ToLower();
                    
                    // Nếu đang ở trang chủ, chuyển hướng theo vai trò
                    if (path == "/" || path == "/home" || path == "/home/index")
                    {
                        if (roles.Contains("Admin"))
                        {
                            context.Response.Redirect("/Admin/Dashboard");
                            return;
                        }
                        else if (roles.Contains("Doctor"))
                        {
                            context.Response.Redirect("/Doctor/Dashboard");
                            return;
                        }
                        else if (roles.Contains("Patient"))
                        {
                            context.Response.Redirect("/Patient/Dashboard");
                            return;
                        }
                    }
                    
                    // Kiểm tra quyền truy cập vào các area
                    if (path.StartsWith("/admin") && !roles.Contains("Admin"))
                    {
                        context.Response.Redirect("/Identity/Account/AccessDenied");
                        return;
                    }
                    else if (path.StartsWith("/doctor") && !roles.Contains("Doctor"))
                    {
                        context.Response.Redirect("/Identity/Account/AccessDenied");
                        return;
                    }
                    else if (path.StartsWith("/patient") && !roles.Contains("Patient"))
                    {
                        context.Response.Redirect("/Identity/Account/AccessDenied");
                        return;
                    }
                }
            }
            
            await _next(context);
        }
    }

    // Extension method để thêm middleware vào pipeline
    public static class RoleMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleMiddleware>();
        }
    }
}
```

### 12. Cập nhật Program.cs để sử dụng Middleware

```csharp
// Thêm vào sau app.UseAuthentication() và trước app.UseAuthorization()
app.UseRoleMiddleware();
```

## Tích hợp với frontend

Các controller và service này sẽ tương tác với frontend thông qua các view. Xem chi tiết trong [Hướng dẫn triển khai frontend](frontend.md).

## Lưu ý

- Đảm bảo cấu hình email chính xác để chức năng quên mật khẩu và xác nhận email hoạt động
- Sử dụng các phương thức bảo mật như HTTPS, Anti-forgery token, và validation để bảo vệ hệ thống
- Cân nhắc sử dụng xác thực hai yếu tố để tăng cường bảo mật
- Xử lý ngoại lệ và ghi log để dễ dàng debug khi có lỗi
- Tùy chỉnh các thông báo lỗi và xác nhận để phù hợp với ngôn ngữ tiếng Việt
