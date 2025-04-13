using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using DoAnChamSocSucKhoe.Areas.Patient.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng hiện tại
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Lấy thông tin hồ sơ sức khỏe
            var healthProfile = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == userId);

            if (healthProfile == null)
            {
                // Tạo hồ sơ sức khỏe mới nếu chưa có
                healthProfile = new HoSoSucKhoe
                {
                    NguoiDungId = userId,
                    NgayCapNhat = DateTime.Now
                };
                _context.HoSoSucKhoes.Add(healthProfile);
                await _context.SaveChangesAsync();
            }

            // Lấy lịch sử chỉ số sức khỏe
            var healthMetrics = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == userId)
                .OrderByDescending(c => c.ThoiGianDo)
                .Take(30)
                .ToListAsync();

            // Lấy lịch hẹn sắp tới
            var upcomingAppointments = await _context.LichHens
                .Include(l => l.NguoiDuocDatLich)
                .Where(l => l.NguoiDatLichId == userId && l.ThoiGianBatDau > DateTime.Now)
                .OrderBy(l => l.ThoiGianBatDau)
                .Take(5)
                .ToListAsync();

            // Lấy nhắc nhở sức khỏe
            var healthReminders = await _context.NhacNhoSucKhoes
                .Where(n => n.NguoiDungId == userId && n.ThoiGian > DateTime.Now)
                .OrderBy(n => n.ThoiGian)
                .Take(5)
                .ToListAsync();

            // Lấy tư vấn gần đây
            var recentConsultations = await _context.TuVanSucKhoes
                .Include(t => t.NguoiTraLoi)
                .Where(t => t.NguoiHoiId == userId)
                .OrderByDescending(t => t.NgayTao)
                .Take(5)
                .ToListAsync();

            // Lấy kế hoạch dinh dưỡng
            var nutritionPlans = await _context.KeHoachDinhDuongs
                .Include(k => k.ChiTietKeHoachDinhDuongs)
                .Where(k => k.NguoiTaoId == userId || _context.ChiTietKeHoachDinhDuongs
                    .Any(c => c.KeHoachDinhDuongId == k.Id && c.KeHoachDinhDuong.NguoiTaoId != userId))
                .OrderByDescending(k => k.NgayTao)
                .Take(3)
                .ToListAsync();

            // Lấy kế hoạch tập luyện
            var exercisePlans = await _context.KeHoachTapLuyens
                .Include(k => k.ChiTietKeHoachTapLuyens)
                .Where(k => k.NguoiTaoId == userId || _context.ChiTietKeHoachTapLuyens
                    .Any(c => c.KeHoachTapLuyenId == k.Id && c.KeHoachTapLuyen.NguoiTaoId != userId))
                .OrderByDescending(k => k.NgayTao)
                .Take(3)
                .ToListAsync();

            // Tạo ViewModel
            var viewModel = new PatientDashboardViewModel
            {
                HealthProfile = healthProfile,
                HealthMetrics = healthMetrics,
                UpcomingAppointments = upcomingAppointments,
                HealthReminders = healthReminders,
                RecentConsultations = recentConsultations,
                NutritionPlans = nutritionPlans,
                ExercisePlans = exercisePlans
            };

            // Tính BMI nếu có chiều cao và cân nặng
            if (healthProfile.ChieuCao.HasValue && healthProfile.ChieuCao.Value > 0 &&
                healthProfile.CanNang.HasValue && healthProfile.CanNang.Value > 0)
            {
                // BMI = cân nặng (kg) / (chiều cao (m) * chiều cao (m))
                var heightInMeters = healthProfile.ChieuCao.Value / 100; // Chuyển từ cm sang m
                viewModel.BMI = Math.Round(healthProfile.CanNang.Value / (heightInMeters * heightInMeters), 1);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetHealthMetricsData()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            // Lấy dữ liệu nhịp tim trong 30 ngày gần nhất
            var heartRateData = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == userId && c.LoaiChiSo == "nhip_tim" && c.ThoiGianDo >= DateTime.Now.AddDays(-30))
                .OrderBy(c => c.ThoiGianDo)
                .Select(c => new { Date = c.ThoiGianDo, Value = c.GiaTri })
                .ToListAsync();

            // Lấy dữ liệu huyết áp trong 30 ngày gần nhất
            var bloodPressureData = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == userId && (c.LoaiChiSo == "huyet_ap_tam_thu" || c.LoaiChiSo == "huyet_ap_tam_truong") && c.ThoiGianDo >= DateTime.Now.AddDays(-30))
                .OrderBy(c => c.ThoiGianDo)
                .Select(c => new { Date = c.ThoiGianDo, Type = c.LoaiChiSo, Value = c.GiaTri })
                .ToListAsync();

            // Lấy dữ liệu đường huyết trong 30 ngày gần nhất
            var bloodSugarData = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == userId && c.LoaiChiSo == "duong_huyet" && c.ThoiGianDo >= DateTime.Now.AddDays(-30))
                .OrderBy(c => c.ThoiGianDo)
                .Select(c => new { Date = c.ThoiGianDo, Value = c.GiaTri })
                .ToListAsync();

            // Lấy dữ liệu cân nặng trong 30 ngày gần nhất
            var weightData = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == userId && c.LoaiChiSo == "can_nang" && c.ThoiGianDo >= DateTime.Now.AddDays(-30))
                .OrderBy(c => c.ThoiGianDo)
                .Select(c => new { Date = c.ThoiGianDo, Value = c.GiaTri })
                .ToListAsync();

            return Json(new
            {
                success = true,
                heartRateData,
                bloodPressureData,
                bloodSugarData,
                weightData
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHealthProfile(HoSoSucKhoe model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            var healthProfile = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == userId);

            if (healthProfile == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ sức khỏe" });
            }

            // Cập nhật thông tin
            healthProfile.ChieuCao = model.ChieuCao;
            healthProfile.CanNang = model.CanNang;
            healthProfile.NhipTim = model.NhipTim;
            healthProfile.DuongHuyet = model.DuongHuyet;
            healthProfile.HuyetApTamThu = model.HuyetApTamThu;
            healthProfile.HuyetApTamTruong = model.HuyetApTamTruong;
            healthProfile.NgayCapNhat = DateTime.Now;

            // Lưu lịch sử chỉ số
            if (model.ChieuCao.HasValue)
            {
                _context.ChiSoSucKhoes.Add(new ChiSoSucKhoe
                {
                    NguoiDungId = userId,
                    LoaiChiSo = "chieu_cao",
                    GiaTri = model.ChieuCao.Value.ToString(),
                    ThoiGianDo = DateTime.Now
                });
            }

            if (model.CanNang.HasValue)
            {
                _context.ChiSoSucKhoes.Add(new ChiSoSucKhoe
                {
                    NguoiDungId = userId,
                    LoaiChiSo = "can_nang",
                    GiaTri = model.CanNang.Value.ToString(),
                    ThoiGianDo = DateTime.Now
                });
            }

            if (model.NhipTim.HasValue)
            {
                _context.ChiSoSucKhoes.Add(new ChiSoSucKhoe
                {
                    NguoiDungId = userId,
                    LoaiChiSo = "nhip_tim",
                    GiaTri = model.NhipTim.Value.ToString(),
                    ThoiGianDo = DateTime.Now
                });
            }

            if (model.DuongHuyet.HasValue)
            {
                _context.ChiSoSucKhoes.Add(new ChiSoSucKhoe
                {
                    NguoiDungId = userId,
                    LoaiChiSo = "duong_huyet",
                    GiaTri = model.DuongHuyet.Value.ToString(),
                    ThoiGianDo = DateTime.Now
                });
            }

            if (model.HuyetApTamThu.HasValue)
            {
                _context.ChiSoSucKhoes.Add(new ChiSoSucKhoe
                {
                    NguoiDungId = userId,
                    LoaiChiSo = "huyet_ap_tam_thu",
                    GiaTri = model.HuyetApTamThu.Value.ToString(),
                    ThoiGianDo = DateTime.Now
                });
            }

            if (model.HuyetApTamTruong.HasValue)
            {
                _context.ChiSoSucKhoes.Add(new ChiSoSucKhoe
                {
                    NguoiDungId = userId,
                    LoaiChiSo = "huyet_ap_tam_truong",
                    GiaTri = model.HuyetApTamTruong.Value.ToString(),
                    ThoiGianDo = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            // Tính BMI
            double? bmi = null;
            if (healthProfile.ChieuCao.HasValue && healthProfile.ChieuCao.Value > 0 &&
                healthProfile.CanNang.HasValue && healthProfile.CanNang.Value > 0)
            {
                var heightInMeters = healthProfile.ChieuCao.Value / 100; // Chuyển từ cm sang m
                bmi = Math.Round(healthProfile.CanNang.Value / (heightInMeters * heightInMeters), 1);
            }

            return Json(new
            {
                success = true,
                message = "Cập nhật hồ sơ sức khỏe thành công",
                bmi
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddHealthReminder(NhacNhoSucKhoe model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            model.NguoiDungId = userId;
            model.NgayTao = DateTime.Now;
            model.TrangThai = "cho_gui";

            _context.NhacNhoSucKhoes.Add(model);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Thêm nhắc nhở sức khỏe thành công",
                reminder = new
                {
                    id = model.Id,
                    noiDung = model.NoiDung,
                    thoiGian = model.ThoiGian,
                    trangThai = model.TrangThai
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHealthReminder(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            var reminder = await _context.NhacNhoSucKhoes
                .FirstOrDefaultAsync(n => n.Id == id && n.NguoiDungId == userId);

            if (reminder == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhắc nhở sức khỏe" });
            }

            _context.NhacNhoSucKhoes.Remove(reminder);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Xóa nhắc nhở sức khỏe thành công"
            });
        }
    }
}
