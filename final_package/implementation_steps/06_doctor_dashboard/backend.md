# Hướng dẫn triển khai backend bảng điều khiển bác sĩ (Doctor Dashboard)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai phần backend của bảng điều khiển bác sĩ (Doctor Dashboard) cho hệ thống quản lý sức khỏe cá nhân. Phần backend của bảng điều khiển bác sĩ bao gồm controller, service, và các model cần thiết để xử lý và cung cấp dữ liệu cho giao diện người dùng.

## Các bước triển khai

### 1. Tạo DoctorDashboardController

Đầu tiên, cần tạo controller cho bảng điều khiển bác sĩ. Controller này sẽ xử lý các request đến bảng điều khiển và trả về view tương ứng.

Tạo file `Controllers/DoctorController.cs` với nội dung từ file `DoctorDashboardController.cs` trong thư mục code.

### 2. Tạo DoctorDashboardViewModel

Tạo model cho bảng điều khiển bác sĩ để truyền dữ liệu từ controller đến view.

Tạo file `Models/ViewModels/DoctorDashboardViewModel.cs` với nội dung từ file `DoctorDashboardViewModel.cs` trong thư mục code.

### 3. Tạo các service cần thiết

#### 3.1. Tạo PatientService

Service này sẽ xử lý việc lấy thông tin bệnh nhân.

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
    public interface IPatientService
    {
        Task<List<ApplicationUser>> GetAllPatientsAsync(string doctorId);
        Task<List<ApplicationUser>> GetNewPatientsAsync(string doctorId, int days = 30);
        Task<ApplicationUser> GetPatientByIdAsync(string patientId);
        Task<List<HoSoSucKhoe>> GetPatientHealthProfilesAsync(string patientId, int count = 10);
    }

    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetAllPatientsAsync(string doctorId)
        {
            // Lấy danh sách bệnh nhân từ lịch hẹn và tư vấn
            var patientIdsFromAppointments = await _context.LichHens
                .Where(l => l.BacSiId == doctorId)
                .Select(l => l.BenhNhanId)
                .Distinct()
                .ToListAsync();

            var patientIdsFromConsultations = await _context.TuVans
                .Where(t => t.BacSiId == doctorId)
                .Select(t => t.BenhNhanId)
                .Distinct()
                .ToListAsync();

            // Kết hợp danh sách bệnh nhân
            var patientIds = patientIdsFromAppointments
                .Union(patientIdsFromConsultations)
                .Distinct()
                .ToList();

            // Lấy thông tin chi tiết của bệnh nhân
            var patients = new List<ApplicationUser>();
            foreach (var patientId in patientIds)
            {
                var patient = await _userManager.FindByIdAsync(patientId);
                if (patient != null && await _userManager.IsInRoleAsync(patient, "Patient"))
                {
                    patients.Add(patient);
                }
            }

            return patients;
        }

        public async Task<List<ApplicationUser>> GetNewPatientsAsync(string doctorId, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);
            
            // Lấy danh sách bệnh nhân mới từ lịch hẹn
            var patientIdsFromAppointments = await _context.LichHens
                .Where(l => l.BacSiId == doctorId && l.NgayTao >= startDate)
                .Select(l => l.BenhNhanId)
                .Distinct()
                .ToListAsync();

            // Lấy danh sách bệnh nhân mới từ tư vấn
            var patientIdsFromConsultations = await _context.TuVans
                .Where(t => t.BacSiId == doctorId && t.NgayTao >= startDate)
                .Select(t => t.BenhNhanId)
                .Distinct()
                .ToListAsync();

            // Kết hợp danh sách bệnh nhân mới
            var patientIds = patientIdsFromAppointments
                .Union(patientIdsFromConsultations)
                .Distinct()
                .ToList();

            // Lấy thông tin chi tiết của bệnh nhân mới
            var patients = new List<ApplicationUser>();
            foreach (var patientId in patientIds)
            {
                var patient = await _userManager.FindByIdAsync(patientId);
                if (patient != null && await _userManager.IsInRoleAsync(patient, "Patient"))
                {
                    patients.Add(patient);
                }
            }

            return patients;
        }

        public async Task<ApplicationUser> GetPatientByIdAsync(string patientId)
        {
            var patient = await _userManager.FindByIdAsync(patientId);
            if (patient != null && await _userManager.IsInRoleAsync(patient, "Patient"))
            {
                return patient;
            }
            return null;
        }

        public async Task<List<HoSoSucKhoe>> GetPatientHealthProfilesAsync(string patientId, int count = 10)
        {
            return await _context.HoSoSucKhoes
                .Where(h => h.NguoiDungId == patientId)
                .OrderByDescending(h => h.NgayTao)
                .Take(count)
                .ToListAsync();
        }
    }
}
```

#### 3.2. Tạo DoctorAppointmentService

Service này sẽ xử lý việc lấy và cập nhật thông tin lịch hẹn của bác sĩ.

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
    public interface IDoctorAppointmentService
    {
        Task<List<LichHen>> GetTodayAppointmentsAsync(string doctorId);
        Task<List<LichHen>> GetUpcomingAppointmentsAsync(string doctorId, int count = 10);
        Task<List<LichHen>> GetPastAppointmentsAsync(string doctorId, int count = 10);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string doctorId, string status);
        Task<Dictionary<DayOfWeek, int>> GetAppointmentsByDayOfWeekAsync(string doctorId, int weeks = 4);
    }

    public class DoctorAppointmentService : IDoctorAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public DoctorAppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LichHen>> GetTodayAppointmentsAsync(string doctorId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            return await _context.LichHens
                .Where(l => l.BacSiId == doctorId && l.NgayHen >= today && l.NgayHen < tomorrow)
                .Include(l => l.BenhNhan)
                .OrderBy(l => l.NgayHen)
                .ToListAsync();
        }

        public async Task<List<LichHen>> GetUpcomingAppointmentsAsync(string doctorId, int count = 10)
        {
            var today = DateTime.Today;
            
            return await _context.LichHens
                .Where(l => l.BacSiId == doctorId && l.NgayHen >= today)
                .Include(l => l.BenhNhan)
                .OrderBy(l => l.NgayHen)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<LichHen>> GetPastAppointmentsAsync(string doctorId, int count = 10)
        {
            var today = DateTime.Today;
            
            return await _context.LichHens
                .Where(l => l.BacSiId == doctorId && l.NgayHen < today)
                .Include(l => l.BenhNhan)
                .OrderByDescending(l => l.NgayHen)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string doctorId, string status)
        {
            var appointment = await _context.LichHens
                .FirstOrDefaultAsync(l => l.Id == appointmentId && l.BacSiId == doctorId);

            if (appointment == null)
                return false;

            appointment.TrangThai = status;
            _context.LichHens.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<DayOfWeek, int>> GetAppointmentsByDayOfWeekAsync(string doctorId, int weeks = 4)
        {
            var startDate = DateTime.Today.AddDays(-7 * weeks);
            var endDate = DateTime.Today;
            
            var appointments = await _context.LichHens
                .Where(l => l.BacSiId == doctorId && l.NgayHen >= startDate && l.NgayHen <= endDate)
                .ToListAsync();

            var result = new Dictionary<DayOfWeek, int>
            {
                { DayOfWeek.Monday, 0 },
                { DayOfWeek.Tuesday, 0 },
                { DayOfWeek.Wednesday, 0 },
                { DayOfWeek.Thursday, 0 },
                { DayOfWeek.Friday, 0 },
                { DayOfWeek.Saturday, 0 },
                { DayOfWeek.Sunday, 0 }
            };

            foreach (var appointment in appointments)
            {
                var dayOfWeek = appointment.NgayHen.DayOfWeek;
                result[dayOfWeek]++;
            }

            return result;
        }
    }
}
```

#### 3.3. Tạo DoctorConsultationService

Service này sẽ xử lý việc lấy và cập nhật thông tin tư vấn của bác sĩ.

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
    public interface IDoctorConsultationService
    {
        Task<List<TuVan>> GetPendingConsultationsAsync(string doctorId);
        Task<List<TuVan>> GetRecentConsultationsAsync(string doctorId, int count = 10);
        Task<bool> AnswerConsultationAsync(int consultationId, string doctorId, string answer);
        Task<Dictionary<string, int>> GetConsultationsByMonthAsync(string doctorId, int months = 12);
    }

    public class DoctorConsultationService : IDoctorConsultationService
    {
        private readonly ApplicationDbContext _context;

        public DoctorConsultationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TuVan>> GetPendingConsultationsAsync(string doctorId)
        {
            return await _context.TuVans
                .Where(t => t.BacSiId == doctorId && string.IsNullOrEmpty(t.TraLoi))
                .Include(t => t.BenhNhan)
                .OrderBy(t => t.NgayTao)
                .ToListAsync();
        }

        public async Task<List<TuVan>> GetRecentConsultationsAsync(string doctorId, int count = 10)
        {
            return await _context.TuVans
                .Where(t => t.BacSiId == doctorId && !string.IsNullOrEmpty(t.TraLoi))
                .Include(t => t.BenhNhan)
                .OrderByDescending(t => t.NgayTraLoi)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> AnswerConsultationAsync(int consultationId, string doctorId, string answer)
        {
            var consultation = await _context.TuVans
                .FirstOrDefaultAsync(t => t.Id == consultationId && t.BacSiId == doctorId);

            if (consultation == null)
                return false;

            consultation.TraLoi = answer;
            consultation.NgayTraLoi = DateTime.Now;
            _context.TuVans.Update(consultation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, int>> GetConsultationsByMonthAsync(string doctorId, int months = 12)
        {
            var startDate = DateTime.Today.AddMonths(-months + 1).AddDays(1 - DateTime.Today.Day);
            var endDate = DateTime.Today;
            
            var consultations = await _context.TuVans
                .Where(t => t.BacSiId == doctorId && t.NgayTao >= startDate && t.NgayTao <= endDate)
                .ToListAsync();

            var result = new Dictionary<string, int>();
            
            for (int i = 0; i < months; i++)
            {
                var date = startDate.AddMonths(i);
                var monthKey = date.ToString("yyyy-MM");
                result[monthKey] = 0;
            }

            foreach (var consultation in consultations)
            {
                var monthKey = consultation.NgayTao.ToString("yyyy-MM");
                if (result.ContainsKey(monthKey))
                {
                    result[monthKey]++;
                }
            }

            return result;
        }
    }
}
```

### 4. Đăng ký các service trong Program.cs

Cập nhật file `Program.cs` để đăng ký các service:

```csharp
// Đăng ký các service
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorAppointmentService, DoctorAppointmentService>();
builder.Services.AddScoped<IDoctorConsultationService, DoctorConsultationService>();
```

### 5. Cập nhật DoctorDashboardController để sử dụng các service

Cập nhật file `Controllers/DoctorController.cs` để sử dụng các service đã tạo:

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
    [Authorize(Roles = "Doctor")]
    [Route("Doctor")]
    public class DoctorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPatientService _patientService;
        private readonly IDoctorAppointmentService _appointmentService;
        private readonly IDoctorConsultationService _consultationService;

        public DoctorController(
            UserManager<ApplicationUser> userManager,
            IPatientService patientService,
            IDoctorAppointmentService appointmentService,
            IDoctorConsultationService consultationService)
        {
            _userManager = userManager;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _consultationService = consultationService;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var doctorId = user.Id;

            var viewModel = new DoctorDashboardViewModel
            {
                Doctor = user,
                Patients = await _patientService.GetAllPatientsAsync(doctorId),
                NewPatients = await _patientService.GetNewPatientsAsync(doctorId),
                TodayAppointments = await _appointmentService.GetTodayAppointmentsAsync(doctorId),
                UpcomingAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(doctorId),
                PendingConsultations = await _consultationService.GetPendingConsultationsAsync(doctorId),
                AppointmentsByDayOfWeek = await _appointmentService.GetAppointmentsByDayOfWeekAsync(doctorId),
                ConsultationsByMonth = await _consultationService.GetConsultationsByMonthAsync(doctorId)
            };

            return View(viewModel);
        }

        [HttpPost("UpdateAppointmentStatus")]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, user.Id, status);
            return Json(new { success = result });
        }

        [HttpPost("AnswerConsultation")]
        public async Task<IActionResult> AnswerConsultation(int consultationId, string answer)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _consultationService.AnswerConsultationAsync(consultationId, user.Id, answer);
            return Json(new { success = result });
        }

        [HttpGet("GetPatientDetails/{patientId}")]
        public async Task<IActionResult> GetPatientDetails(string patientId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
                return NotFound();

            var healthProfiles = await _patientService.GetPatientHealthProfilesAsync(patientId);

            return Json(new { patient, healthProfiles });
        }
    }
}
```

### 6. Tạo API Controller cho dữ liệu biểu đồ

Tạo file `Controllers/DoctorApiController.cs` để cung cấp API cho dữ liệu biểu đồ:

```csharp
using DoAnChamSocSucKhoe.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Controllers
{
    [Authorize(Roles = "Doctor")]
    [Route("api/doctor")]
    [ApiController]
    public class DoctorApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorAppointmentService _appointmentService;
        private readonly IDoctorConsultationService _consultationService;

        public DoctorApiController(
            UserManager<ApplicationUser> userManager,
            IDoctorAppointmentService appointmentService,
            IDoctorConsultationService consultationService)
        {
            _userManager = userManager;
            _appointmentService = appointmentService;
            _consultationService = consultationService;
        }

        [HttpGet("appointments-by-day")]
        public async Task<IActionResult> GetAppointmentsByDayOfWeek([FromQuery] int weeks = 4)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _appointmentService.GetAppointmentsByDayOfWeekAsync(user.Id, weeks);
            return Ok(data);
        }

        [HttpGet("consultations-by-month")]
        public async Task<IActionResult> GetConsultationsByMonth([FromQuery] int months = 12)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var data = await _consultationService.GetConsultationsByMonthAsync(user.Id, months);
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
- Sử dụng các attribute [Authorize] để đảm bảo chỉ người dùng có vai trò "Doctor" mới có thể truy cập
- Tối ưu hóa truy vấn cơ sở dữ liệu để tăng hiệu suất
- Sử dụng API Controller để cung cấp dữ liệu cho các biểu đồ thông qua AJAX
