using Microsoft.AspNetCore.Builder;
using DoAnChamSocSucKhoe.Models;
using DoAnChamSocSucKhoe.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using DoAnChamSocSucKhoe.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI;
using DoAnChamSocSucKhoe.Areas.Doctor.Repositories;
using Microsoft.Extensions.Logging;
using DoAnChamSocSucKhoe.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs - chỉ sử dụng HTTP port 5000
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
// Use SQL Server for all environments
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đặt múi giờ mặc định cho ứng dụng là UTC+7 (Việt Nam)
// Thêm dòng này để đảm bảo xử lý DateTime đúng cách cho toàn bộ ứng dụng
TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
builder.Services.AddSingleton(vietnamTimeZone);

// Đặt múi giờ mặc định cho JSON serialization
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Đảm bảo DateTime được serialize theo múi giờ local
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    })
    .AddRazorRuntimeCompilation();

// Configure Identity
builder.Services.AddIdentity<NguoiDung, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Thay đổi từ true sang false
    options.SignIn.RequireConfirmedEmail = false; // Không yêu cầu xác nhận email
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Cấu hình Authorization Policies (Chính sách quyền hạn)
builder.Services.AddAuthorization(options =>
{
    // Policy: Xem hồ sơ cá nhân
    options.AddPolicy("CanViewProfile", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Patient") || 
            context.User.HasClaim(c => c.Type == "CanViewProfile" && c.Value == "true")));

    // Policy: Đặt lịch hẹn
    options.AddPolicy("CanBookAppointment", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Patient") || 
            context.User.HasClaim(c => c.Type == "CanBookAppointment" && c.Value == "true")));

    // Policy: Gửi tư vấn
    options.AddPolicy("CanSendConsultation", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Patient") || 
            context.User.HasClaim(c => c.Type == "CanSendConsultation" && c.Value == "true")));

    // Policy: Xem lịch sử khám bệnh
    options.AddPolicy("CanViewMedicalHistory", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Patient") || 
            context.User.IsInRole("Doctor") || 
            context.User.HasClaim(c => c.Type == "CanViewMedicalHistory" && c.Value == "true")));

    // Policy: Quản lý tài khoản
    options.AddPolicy("CanManageAccounts", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Admin") || 
            context.User.HasClaim(c => c.Type == "CanManageAccounts" && c.Value == "true")));
});

// Add UserManager
builder.Services.AddTransient<UserManager<NguoiDung>>();

// Add Razor Pages
builder.Services.AddRazorPages();

// Configure Antiforgery for AJAX requests
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Doctor Dashboard Repository
builder.Services.AddScoped<IDoctorDashboardRepository, DoctorDashboardRepository>();

// Add SignalR
builder.Services.AddSignalR();

// Add HttpClientFactory
builder.Services.AddHttpClient("Gemini", client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
});

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("RequirePatientRole", policy => policy.RequireRole("Patient"));
    options.AddPolicy("RequireCaregiverRole", policy => policy.RequireRole("Caregiver"));
});

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // Bỏ HSTS vì chỉ sử dụng HTTP
}

app.UseStaticFiles();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<RoleMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Thêm route cụ thể cho HealthProfile trong area Patient
app.MapControllerRoute(
    name: "patientHealthProfile",
    pattern: "Patient/HealthProfile/{action=Index}/{id?}",
    defaults: new { area = "Patient", controller = "HealthProfile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}/{id?}");
app.MapRazorPages();

// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Apply migrations for SQL Server
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

await app.RunAsync();