# Hướng dẫn triển khai backend bảng điều khiển bệnh nhân (Patient Dashboard)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai phần backend của bảng điều khiển bệnh nhân (Patient Dashboard) cho hệ thống quản lý sức khỏe cá nhân. Phần backend của bảng điều khiển bệnh nhân bao gồm controller, service, và các model cần thiết để xử lý và cung cấp dữ liệu cho giao diện người dùng.

## Các bước triển khai

### 1. Tạo PatientDashboardController

Đầu tiên, cần tạo controller cho bảng điều khiển bệnh nhân. Controller này sẽ xử lý các request đến bảng điều khiển và trả về view tương ứng.

Tạo file `Controllers/PatientController.cs` với nội dung từ file `PatientDashboardController.cs` trong thư mục code.

### 2. Tạo PatientDashboardViewModel

Tạo model cho bảng điều khiển bệnh nhân để truyền dữ liệu từ controller đến view.

Tạo file `Models/ViewModels/PatientDashboardViewModel.cs` với nội dung từ file `PatientDashboardViewModel.cs` trong thư mục code.

### 3. Tạo các service cần thiết

#### 3.1. Tạo HealthMetricsService

Service này sẽ xử lý việc lấy và cập nhật các chỉ số sức khỏe của bệnh nhân.

```csharp
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Services
{
    public interface IHealthMetricsService
    {
        Task<List<ChiTietKeHoachDinhDuong>> GetNutritionPlanAsync(string userId);
        Task<List<ChiTietKeHoachTapLuyen>> GetExercisePlanAsync(string userId);
        Task<List<HoSoSucKhoe>> GetHealthProfilesAsync(string userId, int count = 10);
        Task<HoSoSucKhoe> GetLatestHealthProfileAsync(string userId);
        Task<Dictionary<DateTime, double>> GetWeightHistoryAsync(string userId, int days = 30);
        Task<Dictionary<DateTime, int>> GetHeartRateHistoryAsync(string userId, int days = 30);
        Task<Dictionary<DateTime, string>> GetBloodPressureHistoryAsync(string userId, int days = 30);
        Task<Dictionary<DateTime, double>> GetBloodGlucoseHistoryAsync(string userId, int days = 30);
    }

    public class HealthMetricsService : IHealthMetricsService
    {
        private readonly ApplicationDbContext _context;

        public HealthMetricsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChiTietKeHoachDinhDuong>> GetNutritionPlanAsync(string userId)
        {
            var keHoach = await _context.KeHoachDinhDuongs
                .Where(k => k.NguoiDungId == userId && k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now)
                .OrderByDescending(k => k.NgayBatDau)
                .FirstOrDefaultAsync();

            if (keHoach == null)
                return new List<ChiTietKeHoachDinhDuong>();

            return await _context.ChiTietKeHoachDinhDuongs
                .Where(c => c.KeHoachDinhDuongId == keHoach.Id)
                .Include(c => c.ThucPham)
                .ToListAsync();
        }

        public async Task<List<ChiTietKeHoachTapLuyen>> GetExercisePlanAsync(string userId)
        {
            var keHoach = await _context.KeHoachTapLuyens
                .Where(k => k.NguoiDungId == userId && k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now)
                .OrderByDescending(k => k.NgayBatDau)
                .FirstOrDefaultAsync();

            if (keHoach == null)
                return new List<ChiTietKeHoachTapLuyen>();

            return await _context.ChiTietKeHoachTapLuyens
                .Where(c => c.KeHoachTapLuyenId == keHoach.Id)
                .Include(c => c.BaiTap)
                .ToListAsync();
        }

        public async Task<List<HoSoSucKhoe>> GetHealthProfilesAsync(string userId, int count = 10)
        {
            return await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == userId)
                .OrderByDescending(h => h.NgayTao)
                .Take(count)
                .ToListAsync();
        }

        public async Task<HoSoSucKhoe> GetLatestHealthProfileAsync(string userId)
        {
            return await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == userId)
                .OrderByDescending(h => h.NgayTao)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<DateTime, double>> GetWeightHistoryAsync(string userId, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);
            var profiles = await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == userId && h.NgayTao >= startDate)
                .OrderBy(h => h.NgayTao)
                .ToListAsync();

            return profiles.ToDictionary(p => p.NgayTao.Date, p => p.CanNang);
        }

        public async Task<Dictionary<DateTime, int>> GetHeartRateHistoryAsync(string userId, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);
            var profiles = await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == userId && h.NgayTao >= startDate)
                .OrderBy(h => h.NgayTao)
                .ToListAsync();

            return profiles.ToDictionary(p => p.NgayTao.Date, p => p.NhipTim);
        }

        public async Task<Dictionary<DateTime, string>> GetBloodPressureHistoryAsync(string userId, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);
            var profiles = await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == userId && h.NgayTao >= startDate)
                .OrderBy(h => h.NgayTao)
                .ToListAsync();

            return profiles.ToDictionary(p => p.NgayTao.Date, p => $"{p.HuyetApTamThu}/{p.HuyetApTamTruong}");
        }

        public async Task<Dictionary<DateTime, double>> GetBloodGlucoseHistoryAsync(string userId, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);
            var profiles = await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == userId && h.NgayTao >= startDate)
                .OrderBy(h => h.NgayTao)
                .ToListAsync();

            return profiles.ToDictionary(p => p.NgayTao.Date, p => p.DuongHuyet);
        }
    }
}
```

#### 3.2. Tạo AppointmentService

Service này sẽ xử lý việc lấy thông tin lịch hẹn của bệnh nhân.

```csharp
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Services
{
    public interface IAppointmentService
    {
        Task<List<LichHen>> GetUpcomingAppointmentsAsync(string userId, int count = 5);
        Task<List<LichHen>> GetPastAppointmentsAsync(string userId, int count = 5);
        Task<bool> CreateAppointmentAsync(LichHen appointment);
        Task<bool> CancelAppointmentAsync(int appointmentId, string userId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LichHen>> GetUpcomingAppointmentsAsync(string userId, int count = 5)
        {
            return await _context.LichHens
                .Where(l => l.BenhNhanId == userId && l.NgayHen >= DateTime.Now)
                .Include(l => l.BacSi)
                .OrderBy(l => l.NgayHen)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<LichHen>> GetPastAppointmentsAsync(string userId, int count = 5)
        {
            return await _context.LichHens
                .Where(l => l.BenhNhanId == userId && l.NgayHen < DateTime.Now)
                .Include(l => l.BacSi)
                .OrderByDescending(l => l.NgayHen)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> CreateAppointmentAsync(LichHen appointment)
        {
            try
            {
                _context.LichHens.Add(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, string userId)
        {
            var appointment = await _context.LichHens
                .FirstOrDefaultAsync(l => l.Id == appointmentId && l.BenhNhanId == userId);

            if (appointment == null)
                return false;

            appointment.TrangThai = "Đã hủy";
            _context.LichHens.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

#### 3.3. Tạo ConsultationService

Service này sẽ xử lý việc lấy thông tin tư vấn của bệnh nhân.

```csharp
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Services
{
    public interface IConsultationService
    {
        Task<List<TuVan>> GetRecentConsultationsAsync(string userId, int count = 5);
        Task<bool> CreateConsultationAsync(TuVan consultation);
    }

    public class ConsultationService : IConsultationService
    {
        private readonly ApplicationDbContext _context;

        public ConsultationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TuVan>> GetRecentConsultationsAsync(string userId, int count = 5)
        {
            return await _context.TuVans
                .Where(t => t.BenhNhanId == userId)
                .Include(t => t.BacSi)
                .OrderByDescending(t => t.NgayTao)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> CreateConsultationAsync(TuVan consultation)
        {
            try
            {
                _context.TuVans.Add(consultation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

#### 3.4. Tạo ReminderService

Service này sẽ xử lý việc lấy thông tin nhắc nhở của bệnh nhân.

```csharp
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Services
{
    public interface IReminderService
    {
        Task<List<NhacNho>> GetActiveRemindersAsync(string userId);
        Task<bool> CreateReminderAsync(NhacNho reminder);
        Task<bool> UpdateReminderStatusAsync(int reminderId, string userId, bool isCompleted);
    }

    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;

        public ReminderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NhacNho>> GetActiveRemindersAsync(string userId)
        {
            return await _context.NhacNhos
                .Where(n => n.NguoiDungId == userId && n.NgayNhac >= DateTime.Today)
                .OrderBy(n => n.NgayNhac)
                .ToListAsync();
        }

        public async Task<bool> CreateReminderAsync(NhacNho reminder)
        {
            try
            {
                _context.NhacNhos.Add(reminder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateReminderStatusAsync(int reminderId, string userId, bool isCompleted)
        {
            var reminder = await _context.NhacNhos
                .FirstOrDefaultAsync(n => n.Id == reminderId && n.NguoiDungId == userId);

            if (reminder == null)
                return false;

            reminder.DaHoanThanh = isCompleted;
            _context.NhacNhos.Update(reminder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

### 4. Đăng ký các service trong Program.cs

Cập nhật file `Program.cs` để đăng ký các service:

```csharp
// Đăng ký các service
builder.Services.AddScoped<IHealthMetricsService, HealthMetricsService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
```

### 5. Cập nhật PatientDashboardController để sử dụng các service

Cập nhật file `Controllers/PatientController.cs` để sử dụng các service đã tạo:

```csharp
using DoAnChamSocSucKhoe.Models;
using DoAnChamSocSucKhoe.Models.ViewModels;
using DoAnChamSocSucKhoe.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize(Roles = "Patient")]
    [Route("Patient")]
    public class PatientController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHealthMetricsService _healthMetricsService;
        private readonly IAppointmentService _appointmentService;
        private readonly IConsultationService _consultationService;
        private readonly IReminderService _reminderService;

        public PatientController(
            UserManager<ApplicationUser> userManager,
            IHealthMetricsService healthMetricsService,
            IAppointmentService appointmentService,
            IConsultationService consultationService,
            IReminderService reminderService)
        {
            _userManager = userManager;
            _healthMetricsService = healthMetricsService;
            _appointmentService = appointmentService;
            _consultationService = consultationService;
            _reminderService = reminderService;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userId = user.Id;

            var viewModel = new PatientDashboardViewModel
            {
                User = user,
                LatestHealthProfile = await _healthMetricsService.GetLatestHealthProfileAsync(userId),
                UpcomingAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(userId),
                ActiveReminders = await _reminderService.GetActiveRemindersAsync(userId),
                RecentConsultations = await _consultationService.GetRecentConsultationsAsync(userId),
                NutritionPlan = await _healthMetricsService.GetNutritionPlanAsync(userId),
                ExercisePlan = await _healthMetricsService.GetExercisePlanAsync(userId),
                WeightHistory = await _healthMetricsService.GetWeightHistoryAsync(userId),
                HeartRateHistory = await _healthMetricsService.GetHeartRateHistoryAsync(userId),
                BloodPressureHistory = await _healthMetricsService.GetBloodPressureHistoryAsync(userId),
                BloodGlucoseHistory = await _healthMetricsService.GetBloodGlucoseHistoryAsync(userId)
            };

            return View(viewModel);
        }

        [HttpGet("GetWeightHistory")]
        public async Task<IActionResult> GetWeightHistory(int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetWeightHistoryAsync(user.Id, days);
            return Json(data);
        }

        [HttpGet("GetHeartRateHistory")]
        public async Task<IActionResult> GetHeartRateHistory(int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetHeartRateHistoryAsync(user.Id, days);
            return Json(data);
        }

        [HttpGet("GetBloodPressureHistory")]
        public async Task<IActionResult> GetBloodPressureHistory(int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetBloodPressureHistoryAsync(user.Id, days);
            return Json(data);
        }

        [HttpGet("GetBloodGlucoseHistory")]
        public async Task<IActionResult> GetBloodGlucoseHistory(int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetBloodGlucoseHistoryAsync(user.Id, days);
            return Json(data);
        }

        [HttpPost("UpdateReminderStatus")]
        public async Task<IActionResult> UpdateReminderStatus(int reminderId, bool isCompleted)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _reminderService.UpdateReminderStatusAsync(reminderId, user.Id, isCompleted);
            return Json(new { success = result });
        }
    }
}
```

### 6. Tạo API Controller cho dữ liệu biểu đồ

Tạo file `Controllers/PatientApiController.cs` để cung cấp API cho dữ liệu biểu đồ:

```csharp
using DoAnChamSocSucKhoe.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize(Roles = "Patient")]
    [Route("api/patient")]
    [ApiController]
    public class PatientApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHealthMetricsService _healthMetricsService;

        public PatientApiController(
            UserManager<ApplicationUser> userManager,
            IHealthMetricsService healthMetricsService)
        {
            _userManager = userManager;
            _healthMetricsService = healthMetricsService;
        }

        [HttpGet("weight-history")]
        public async Task<IActionResult> GetWeightHistory([FromQuery] int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetWeightHistoryAsync(user.Id, days);
            return Ok(data);
        }

        [HttpGet("heart-rate-history")]
        public async Task<IActionResult> GetHeartRateHistory([FromQuery] int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetHeartRateHistoryAsync(user.Id, days);
            return Ok(data);
        }

        [HttpGet("blood-pressure-history")]
        public async Task<IActionResult> GetBloodPressureHistory([FromQuery] int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetBloodPressureHistoryAsync(user.Id, days);
            return Ok(data);
        }

        [HttpGet("blood-glucose-history")]
        public async Task<IActionResult> GetBloodGlucoseHistory([FromQuery] int days = 30)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _healthMetricsService.GetBloodGlucoseHistoryAsync(user.Id, days);
            return Ok(data);
        }
    }
}
```

## Tích hợp với frontend

Các controller và service này sẽ tương tác với frontend thông qua các view. Xem chi tiết trong [Hướng dẫn triển khai frontend](frontend.md).

## Lưu ý

- Đảm bảo các service được đăng ký đúng cách trong Program.cs
- Sử dụng async/await cho tất cả các thao tác với cơ sở dữ liệu để tránh blocking
- Xử lý ngoại lệ và ghi log để dễ dàng debug khi có lỗi
- Sử dụng các attribute [Authorize] để đảm bảo chỉ người dùng có vai trò "Patient" mới có thể truy cập
- Tối ưu hóa truy vấn cơ sở dữ liệu để tăng hiệu suất
- Sử dụng API Controller để cung cấp dữ liệu cho các biểu đồ thông qua AJAX
