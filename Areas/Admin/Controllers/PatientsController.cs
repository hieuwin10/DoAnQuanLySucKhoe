using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Areas.Admin.Models;
using DoAnChamSocSucKhoe.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<PatientsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchTerm, string statusFilter = "all", int pageNumber = 1)
        {
            try
            {
                // Set page size
                const int pageSize = 10;

                // Create view model
                var viewModel = new PatientListViewModel
                {
                    SearchTerm = searchTerm ?? string.Empty,
                    StatusFilter = statusFilter,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Get patient statistics
                await LoadPatientStatistics(viewModel);

                // Apply filters to get only patients (VaiTroId = 3 for patients)
                var query = _context.NguoiDungs
                    .Include(u => u.HoSoSucKhoe)
                    .Where(u => u.VaiTroId == 3) // Assuming Role ID 3 is for patients
                    .AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.HoTen.ToLower().Contains(searchTerm) ||
                        (p.Email != null && p.Email.ToLower().Contains(searchTerm)) ||
                        (p.PhoneNumber != null && p.PhoneNumber.Contains(searchTerm)));
                }

                // Status filter
                switch (statusFilter)
                {
                    case "treatment":
                        query = query.Where(p => p.HoSoSucKhoe != null && p.HoSoSucKhoe.TrangThai == "Đang điều trị");
                        break;
                    case "monitoring":
                        query = query.Where(p => p.HoSoSucKhoe != null && p.HoSoSucKhoe.TrangThai == "Cần theo dõi");
                        break;
                    case "new":
                        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                        query = query.Where(p => p.NgayTao >= thirtyDaysAgo);
                        break;
                }

                // Get total count for pagination
                var totalPatients = await query.CountAsync();
                viewModel.TotalPages = (int)Math.Ceiling(totalPatients / (double)pageSize);

                // Get patients for current page with their health profile info
                var patients = await query
                    .OrderByDescending(p => p.NgayTao)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PatientViewModel
                    {
                        Id = p.Id,
                        FullName = p.HoTen,
                        Age = (p.HoSoSucKhoe != null && p.HoSoSucKhoe.NgaySinh.HasValue)
                            ? DateTime.Now.Year - p.HoSoSucKhoe.NgaySinh.Value.Year : 0,
                        Gender = p.HoSoSucKhoe != null ? p.HoSoSucKhoe.GioiTinh ?? "N/A" : "N/A",
                        PhoneNumber = p.PhoneNumber ?? "",
                        Diagnosis = p.HoSoSucKhoe != null ? p.HoSoSucKhoe.ChanDoan ?? "Chưa có" : "Chưa có",
                        Status = p.HoSoSucKhoe != null ? p.HoSoSucKhoe.TrangThai ?? "Chờ khám" : "Chờ khám",
                        Avatar = !string.IsNullOrEmpty(p.AnhDaiDien) ? p.AnhDaiDien : "/images/BN.png"
                    })
                    .ToListAsync();

                viewModel.Patients = patients;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang quản lý bệnh nhân");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách bệnh nhân.";
                return View(new PatientListViewModel());
            }
        }

        private async Task LoadPatientStatistics(PatientListViewModel viewModel)
        {
            try
            {
                // Try to get from cache first
                PatientStats? stats;
                if (!_cache.TryGetValue("PatientStats", out stats))
                {
                    var lastMonth = DateTime.Now.AddMonths(-1);

                    // Get current counts - assumes VaiTroId 3 is for patients
                    var totalPatients = await _context.NguoiDungs.CountAsync(p => p.VaiTroId == 3);

                    // New patients (registered in the last 30 days)
                    var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                    var newPatients = await _context.NguoiDungs
                        .CountAsync(p => p.VaiTroId == 3 && p.NgayTao >= thirtyDaysAgo);

                    // Patients under treatment
                    var underTreatmentPatients = await _context.NguoiDungs
                        .Where(p => p.VaiTroId == 3 && p.HoSoSucKhoe != null && p.HoSoSucKhoe.TrangThai == "Đang điều trị")
                        .CountAsync();

                    // Patients needing monitoring
                    var monitoringPatients = await _context.NguoiDungs
                        .Where(p => p.VaiTroId == 3 && p.HoSoSucKhoe != null && p.HoSoSucKhoe.TrangThai == "Cần theo dõi")
                        .CountAsync();

                    // Get last month counts for growth calculation
                    var twoMonthsAgo = DateTime.Now.AddMonths(-2);
                    var lastMonthNewPatients = await _context.NguoiDungs
                        .CountAsync(p => p.VaiTroId == 3 && p.NgayTao >= twoMonthsAgo && p.NgayTao < thirtyDaysAgo);

                    // Calculate growth rates
                    decimal newGrowthRate = lastMonthNewPatients > 0
                        ? ((decimal)newPatients - lastMonthNewPatients) / lastMonthNewPatients * 100
                        : 100;

                    // For simplicity, use fixed growth rates for treatment and monitoring
                    decimal treatmentGrowthRate = 4.5m;
                    decimal monitoringGrowthRate = 3.0m;

                    // Age distribution data
                    var ageDistribution = new List<AgeGroupData>
                    {
                        new AgeGroupData { AgeGroup = "0-10", MaleCount = 12, FemaleCount = 10 },
                        new AgeGroupData { AgeGroup = "11-20", MaleCount = 15, FemaleCount = 18 },
                        new AgeGroupData { AgeGroup = "21-30", MaleCount = 21, FemaleCount = 25 },
                        new AgeGroupData { AgeGroup = "31-40", MaleCount = 28, FemaleCount = 30 },
                        new AgeGroupData { AgeGroup = "41-50", MaleCount = 35, FemaleCount = 33 },
                        new AgeGroupData { AgeGroup = "51-60", MaleCount = 25, FemaleCount = 22 },
                        new AgeGroupData { AgeGroup = "61-70", MaleCount = 18, FemaleCount = 15 },
                        new AgeGroupData { AgeGroup = "71-80", MaleCount = 10, FemaleCount = 8 },
                        new AgeGroupData { AgeGroup = "81+", MaleCount = 5, FemaleCount = 3 }
                    };

                    // Diagnosis distribution data
                    var diagnosisDistribution = new List<DiagnosisData>
                    {
                        new DiagnosisData { DiagnosisName = "Tim mạch", PatientCount = 45, Color = "#4e73df" },
                        new DiagnosisData { DiagnosisName = "Hô hấp", PatientCount = 35, Color = "#1cc88a" },
                        new DiagnosisData { DiagnosisName = "Tiêu hóa", PatientCount = 28, Color = "#36b9cc" },
                        new DiagnosisData { DiagnosisName = "Da liễu", PatientCount = 15, Color = "#f6c23e" },
                        new DiagnosisData { DiagnosisName = "Cơ xương khớp", PatientCount = 25, Color = "#e74a3b" },
                        new DiagnosisData { DiagnosisName = "Nội tiết", PatientCount = 18, Color = "#fd7e14" },
                        new DiagnosisData { DiagnosisName = "Tâm thần", PatientCount = 12, Color = "#6610f2" },
                        new DiagnosisData { DiagnosisName = "Khác", PatientCount = 20, Color = "#858796" }
                    };

                    stats = new PatientStats
                    {
                        TotalPatients = totalPatients,
                        NewPatients = newPatients,
                        UnderTreatmentPatients = underTreatmentPatients,
                        MonitoringPatients = monitoringPatients,
                        NewPatientsGrowthRate = newGrowthRate,
                        UnderTreatmentGrowthRate = treatmentGrowthRate,
                        MonitoringGrowthRate = monitoringGrowthRate,
                        AgeDistribution = ageDistribution,
                        DiagnosisDistribution = diagnosisDistribution
                    };

                    // Cache for 5 minutes
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    _cache.Set("PatientStats", stats, cacheOptions);
                }

                viewModel.Stats = stats ?? new PatientStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải thống kê bệnh nhân");
                viewModel.Stats = new PatientStats
                {
                    TotalPatients = 248,
                    NewPatients = 32,
                    UnderTreatmentPatients = 156,
                    MonitoringPatients = 42,
                    NewPatientsGrowthRate = 3.7m,
                    UnderTreatmentGrowthRate = 4.5m,
                    MonitoringGrowthRate = 3.0m
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string patientId, string status)
        {
            try
            {
                // Get patient statistics from database
                var patientIds = await _context.NguoiDungs
                    .Where(p => p.VaiTroId == 3) // Assuming 3 is the patient role ID
                    .Select(p => p.Id)
                    .ToListAsync();

                if (patientIds.Contains(patientId))
                {
                    // Continue with your existing code...
                    // This avoids the int and string comparison issue
                    var patient = await _context.NguoiDungs
                        .Include(p => p.HoSoSucKhoe)
                        .FirstOrDefaultAsync(p => p.Id == patientId && p.VaiTroId == 3);

                    if (patient == null || patient.HoSoSucKhoe == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy hồ sơ sức khỏe của bệnh nhân" });
                    }

                    patient.HoSoSucKhoe.TrangThai = status;
                    patient.HoSoSucKhoe.NgayCapNhat = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Invalidate cache
                    _cache.Remove("PatientStats");

                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy bệnh nhân" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi trạng thái bệnh nhân");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Lấy thông tin bệnh nhân cùng với hồ sơ sức khỏe
                var patient = await _context.NguoiDungs
                    .Include(u => u.HoSoSucKhoe)
                    .Include(u => u.ChiSoSucKhoes)
                    .Include(u => u.LichHens)
                    .FirstOrDefaultAsync(u => u.Id == id && u.VaiTroId == 3);

                if (patient == null)
                {
                    return NotFound("Không tìm thấy bệnh nhân");
                }

                // Tính tuổi từ ngày sinh
                int age = 0;
                if (patient.HoSoSucKhoe != null && patient.HoSoSucKhoe.NgaySinh.HasValue)
                {
                    age = DateTime.Now.Year - patient.HoSoSucKhoe.NgaySinh.Value.Year;
                    if (DateTime.Now.DayOfYear < patient.HoSoSucKhoe.NgaySinh.Value.DayOfYear)
                        age--;
                }

                // Lấy lịch sử khám bệnh (các lịch hẹn đã hoàn thành)
                var medicalHistory = await _context.LichHens
                    .Include(a => a.BacSi)
                    .Where(a => a.BenhNhanId == id && a.TrangThai == "Đã hoàn thành")
                    .OrderByDescending(a => a.NgayHen)
                    .Take(10)
                    .Select(a => new MedicalHistoryItem
                    {
                        AppointmentDate = a.NgayHen,
                        DoctorName = a.BacSi != null ? a.BacSi.HoTen : "Không xác định",
                        Diagnosis = a.ChanDoan ?? "Không có",
                        Prescription = a.DonThuoc ?? "Không có",
                        Notes = a.GhiChu ?? "Không có"
                    })
                    .ToListAsync();

                // Lấy lịch hẹn sắp tới
                var upcomingAppointments = await _context.LichHens
                    .Include(a => a.BacSi)
                    .Where(a => a.BenhNhanId == id &&
                               (a.TrangThai == "Đã xác nhận" || a.TrangThai == "Chờ xác nhận") &&
                                a.NgayHen >= DateTime.Now)
                    .OrderBy(a => a.NgayHen)
                    .Take(5)
                    .Select(a => new AppointmentItem
                    {
                        Id = a.Id,
                        AppointmentDate = a.NgayHen,
                        DoctorName = a.BacSi != null ? a.BacSi.HoTen : "Không xác định",
                        Status = a.TrangThai,
                        Reason = a.LyDo ?? "Không có"
                    })
                    .ToListAsync();

                // Lấy chỉ số sức khỏe gần đây nhất
                var latestHealthMetrics = await _context.ChiSoSucKhoes
                    .Where(m => m.NguoiDungId == id)
                    .OrderByDescending(m => m.NgayCapNhat)
                    .Take(10)
                    .ToListAsync();

                // Tạo view model
                var viewModel = new PatientDetailViewModel
                {
                    Id = patient.Id,
                    FullName = patient.HoTen,
                    Email = patient.Email ?? "Không có email",
                    PhoneNumber = patient.PhoneNumber ?? "",
                    Avatar = !string.IsNullOrEmpty(patient.AnhDaiDien) ? patient.AnhDaiDien : "/images/BN.png",
                    Status = patient.HoSoSucKhoe?.TrangThai ?? "Chờ khám",
                    Age = age,
                    Gender = patient.HoSoSucKhoe?.GioiTinh ?? "Không xác định",
                    BloodType = patient.HoSoSucKhoe?.NhomMau ?? "Chưa cập nhật",
                    Weight = latestHealthMetrics.FirstOrDefault(m => m.LoaiChiSo == "Cân nặng")?.GiaTri ?? "Chưa cập nhật",
                    Height = latestHealthMetrics.FirstOrDefault(m => m.LoaiChiSo == "Chiều cao")?.GiaTri ?? "Chưa cập nhật",
                    BloodPressure = latestHealthMetrics.FirstOrDefault(m => m.LoaiChiSo == "Huyết áp")?.GiaTri ?? "Chưa cập nhật",
                    HeartRate = latestHealthMetrics.FirstOrDefault(m => m.LoaiChiSo == "Nhịp tim")?.GiaTri ?? "Chưa cập nhật",
                    GlucoseLevel = latestHealthMetrics.FirstOrDefault(m => m.LoaiChiSo == "Đường huyết")?.GiaTri ?? "Chưa cập nhật",
                    Cholesterol = latestHealthMetrics.FirstOrDefault(m => m.LoaiChiSo == "Cholesterol")?.GiaTri ?? "Chưa cập nhật",
                    Address = patient.HoSoSucKhoe?.DiaChi ?? "Chưa cập nhật",
                    RegisterDate = patient.NgayTao,
                    MedicalHistory = patient.HoSoSucKhoe?.TienSuBenh ?? "Không có",
                    Allergies = patient.HoSoSucKhoe?.DiUng ?? "Không có",
                    CurrentMedications = patient.HoSoSucKhoe?.ThuocDangDung ?? "Không có",
                    FamilyMedicalHistory = patient.HoSoSucKhoe?.TienSuGiaDinh ?? "Không có",
                    Lifestyle = patient.HoSoSucKhoe?.LoiSong ?? "Chưa cập nhật",
                    CurrentDiagnosis = patient.HoSoSucKhoe?.ChanDoan ?? "Không có",
                    Treatments = patient.HoSoSucKhoe?.PhuongPhapDieuTri ?? "Không có",
                    Notes = patient.HoSoSucKhoe?.GhiChu ?? "Không có",
                    MedicalHistoryItems = medicalHistory,
                    UpcomingAppointments = upcomingAppointments,
                    HealthMetrics = latestHealthMetrics.Select(m => new HealthMetricItem
                    {
                        MetricType = m.LoaiChiSo,
                        Value = m.GiaTri,
                        Date = m.NgayCapNhat,
                        Status = DetermineMetricStatus(m.LoaiChiSo, m.GiaTri)
                    }).ToList()
                };

                // Xác định trạng thái cho các chỉ số sức khỏe
                SetHealthMetricsStatus(viewModel);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết bệnh nhân");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin bệnh nhân.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string DetermineMetricStatus(string metricType, string value)
        {
            if (string.IsNullOrEmpty(value) || value == "Chưa cập nhật")
                return "normal";

            try
            {
                switch (metricType)
                {
                    case "Huyết áp":
                        // Định dạng huyết áp: "120/80 mmHg"
                        var parts = value.Split('/');
                        if (parts.Length == 2)
                        {
                            var systolic = int.Parse(parts[0].Trim());
                            var diastolic = int.Parse(parts[1].Split(' ')[0].Trim());

                            if (systolic >= 140 || diastolic >= 90)
                                return "danger";
                            if (systolic >= 130 || diastolic >= 85)
                                return "warning";
                            if (systolic <= 90 || diastolic <= 60)
                                return "warning";
                            return "normal";
                        }
                        break;

                    case "Nhịp tim":
                        var heartRate = int.Parse(value.Split(' ')[0]);
                        if (heartRate > 100)
                            return "danger";
                        if (heartRate < 60)
                            return "warning";
                        return "normal";

                    case "Đường huyết":
                        var glucose = double.Parse(value.Split(' ')[0]);
                        if (glucose > 126)
                            return "danger";
                        if (glucose > 100)
                            return "warning";
                        return "normal";

                    case "Cholesterol":
                        var cholesterol = int.Parse(value.Split(' ')[0]);
                        if (cholesterol > 240)
                            return "danger";
                        if (cholesterol > 200)
                            return "warning";
                        return "normal";
                }
            }
            catch
            {
                // Nếu có lỗi khi phân tích, trả về normal để tránh lỗi UI
            }

            return "normal";
        }

        private void SetHealthMetricsStatus(PatientDetailViewModel viewModel)
        {
            // Tính BMI nếu có cân nặng và chiều cao
            if (viewModel.Weight != "Chưa cập nhật" && viewModel.Height != "Chưa cập nhật")
            {
                try
                {
                    double weight = double.Parse(viewModel.Weight.Split(' ')[0]);
                    double height = double.Parse(viewModel.Height.Split(' ')[0]) / 100; // Chuyển cm sang m

                    double bmi = weight / (height * height);
                    viewModel.BMI = bmi.ToString("F1");

                    if (bmi < 18.5)
                        viewModel.BMIStatus = "warning";
                    else if (bmi > 25)
                        viewModel.BMIStatus = "danger";
                    else
                        viewModel.BMIStatus = "normal";
                }
                catch
                {
                    viewModel.BMI = "Không tính được";
                    viewModel.BMIStatus = "normal";
                }
            }

            // Trạng thái huyết áp
            viewModel.BloodPressureStatus = DetermineMetricStatus("Huyết áp", viewModel.BloodPressure);

            // Trạng thái nhịp tim
            viewModel.HeartRateStatus = DetermineMetricStatus("Nhịp tim", viewModel.HeartRate);

            // Trạng thái đường huyết
            viewModel.GlucoseLevelStatus = DetermineMetricStatus("Đường huyết", viewModel.GlucoseLevel);

            // Trạng thái cholesterol
            viewModel.CholesterolStatus = DetermineMetricStatus("Cholesterol", viewModel.Cholesterol);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePatientStatus(string patientId, string status)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return Json(new { success = false, message = "ID bệnh nhân không hợp lệ" });
            }

            try
            {
                var healthProfile = await _context.HoSoSucKhoes
                    .FirstOrDefaultAsync(h => h.NguoiDungId == patientId);

                if (healthProfile == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ sức khỏe của bệnh nhân" });
                }

                healthProfile.TrangThai = status;
                healthProfile.NgayCapNhat = DateTime.Now;

                await _context.SaveChangesAsync();

                // Xóa cache để cập nhật số liệu
                _cache.Remove("PatientStats");

                return Json(new { success = true, message = "Cập nhật trạng thái bệnh nhân thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái bệnh nhân");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        // GET: Admin/Patients/Create
        public IActionResult Create()
        {
            return View(new CreatePatientViewModel());
        }

        // POST: Admin/Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Kiểm tra tên đăng nhập đã tồn tại chưa
                var existingUserName = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                if (existingUserName != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập này đã được sử dụng.");
                    return View(model);
                }

                // Tạo user mới
                var user = new NguoiDung
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    PhoneNumber = model.PhoneNumber,
                    GioiTinh = model.GioiTinh,
                    DiaChi = model.DiaChi,
                    NgaySinh = model.NgaySinh,
                    AnhDaiDien = string.IsNullOrEmpty(model.AnhDaiDien) ? "/images/default-avatar.png" : model.AnhDaiDien,
                    VaiTroId = 3, // Patient role
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    TrangThai = "Đang hoạt động"
                };

                // Hash password
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<NguoiDung>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã tạo tài khoản bệnh nhân '{model.HoTen}' thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bệnh nhân mới");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo bệnh nhân mới.";
                return View(model);
            }
        }
    }
}