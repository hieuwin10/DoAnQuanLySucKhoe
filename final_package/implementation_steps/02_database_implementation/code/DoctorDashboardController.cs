using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using DoAnChamSocSucKhoe.Areas.Doctor.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
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

            // Lấy tổng số bệnh nhân đã tương tác với bác sĩ
            var totalPatients = await _context.LichHens
                .Where(l => l.NguoiDuocDatLichId == userId)
                .Select(l => l.NguoiDatLichId)
                .Distinct()
                .CountAsync();

            // Lấy tổng số lịch hẹn
            var totalAppointments = await _context.LichHens
                .Where(l => l.NguoiDuocDatLichId == userId)
                .CountAsync();

            // Lấy tổng số câu hỏi tư vấn
            var totalConsultations = await _context.TuVanSucKhoes
                .Where(t => t.NguoiTraLoiId == userId)
                .CountAsync();

            // Lấy tổng số đánh giá
            var totalRatings = await _context.DanhGiaChuyenGias
                .Where(d => d.ChuyenGiaId == userId)
                .CountAsync();

            // Lấy điểm đánh giá trung bình
            double averageRating = 0;
            if (totalRatings > 0)
            {
                averageRating = await _context.DanhGiaChuyenGias
                    .Where(d => d.ChuyenGiaId == userId)
                    .AverageAsync(d => d.SoSao);
            }

            // Lấy lịch hẹn sắp tới
            var upcomingAppointments = await _context.LichHens
                .Include(l => l.NguoiDatLich)
                .Where(l => l.NguoiDuocDatLichId == userId && l.ThoiGianBatDau > DateTime.Now)
                .OrderBy(l => l.ThoiGianBatDau)
                .Take(5)
                .ToListAsync();

            // Lấy câu hỏi tư vấn chưa trả lời
            var pendingConsultations = await _context.TuVanSucKhoes
                .Include(t => t.NguoiHoi)
                .Where(t => t.NguoiTraLoiId == userId && t.CauTraLoi == null)
                .OrderByDescending(t => t.NgayTao)
                .Take(5)
                .ToListAsync();

            // Lấy bệnh nhân gần đây
            var recentPatients = await _context.LichHens
                .Include(l => l.NguoiDatLich)
                .Where(l => l.NguoiDuocDatLichId == userId)
                .OrderByDescending(l => l.ThoiGianBatDau)
                .Select(l => l.NguoiDatLich)
                .Distinct()
                .Take(5)
                .ToListAsync();

            // Tạo ViewModel
            var viewModel = new DoctorDashboardViewModel
            {
                TotalPatients = totalPatients,
                TotalAppointments = totalAppointments,
                TotalConsultations = totalConsultations,
                TotalRatings = totalRatings,
                AverageRating = averageRating,
                UpcomingAppointments = upcomingAppointments,
                PendingConsultations = pendingConsultations,
                RecentPatients = recentPatients
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentsByDate(DateTime date)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            // Lấy lịch hẹn theo ngày
            var appointments = await _context.LichHens
                .Include(l => l.NguoiDatLich)
                .Where(l => l.NguoiDuocDatLichId == userId && 
                       l.ThoiGianBatDau.Date == date.Date)
                .OrderBy(l => l.ThoiGianBatDau)
                .Select(l => new
                {
                    id = l.Id,
                    title = l.NguoiDatLich.HoTen,
                    start = l.ThoiGianBatDau,
                    end = l.ThoiGianKetThuc,
                    status = l.TrangThai,
                    note = l.GhiChu
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                appointments
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, string status)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            var appointment = await _context.LichHens
                .FirstOrDefaultAsync(l => l.Id == id && l.NguoiDuocDatLichId == userId);

            if (appointment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
            }

            appointment.TrangThai = status;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Cập nhật trạng thái lịch hẹn thành công"
            });
        }

        [HttpPost]
        public async Task<IActionResult> AnswerConsultation(int id, string answer)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            var consultation = await _context.TuVanSucKhoes
                .FirstOrDefaultAsync(t => t.Id == id && t.NguoiTraLoiId == userId);

            if (consultation == null)
            {
                return Json(new { success = false, message = "Không tìm thấy câu hỏi tư vấn" });
            }

            consultation.CauTraLoi = answer;
            consultation.NgayTraLoi = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Trả lời câu hỏi tư vấn thành công"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientDetails(string id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Người dùng không xác định" });
            }

            // Lấy thông tin bệnh nhân
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (patient == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bệnh nhân" });
            }

            // Lấy hồ sơ sức khỏe của bệnh nhân
            var healthProfile = await _context.HoSoSucKhoes
                .FirstOrDefaultAsync(h => h.NguoiDungId == id);

            // Lấy lịch sử chỉ số sức khỏe
            var healthMetrics = await _context.ChiSoSucKhoes
                .Where(c => c.NguoiDungId == id)
                .OrderByDescending(c => c.ThoiGianDo)
                .Take(10)
                .ToListAsync();

            // Lấy lịch sử lịch hẹn
            var appointmentHistory = await _context.LichHens
                .Where(l => l.NguoiDatLichId == id && l.NguoiDuocDatLichId == userId)
                .OrderByDescending(l => l.ThoiGianBatDau)
                .Take(5)
                .ToListAsync();

            // Lấy lịch sử tư vấn
            var consultationHistory = await _context.TuVanSucKhoes
                .Where(t => t.NguoiHoiId == id && t.NguoiTraLoiId == userId)
                .OrderByDescending(t => t.NgayTao)
                .Take(5)
                .ToListAsync();

            return Json(new
            {
                success = true,
                patient = new
                {
                    id = patient.Id,
                    name = patient.HoTen,
                    email = patient.Email,
                    phone = patient.PhoneNumber,
                    gender = patient.GioiTinh,
                    birthDate = patient.NgaySinh,
                    address = patient.DiaChi
                },
                healthProfile = healthProfile != null ? new
                {
                    height = healthProfile.ChieuCao,
                    weight = healthProfile.CanNang,
                    heartRate = healthProfile.NhipTim,
                    bloodSugar = healthProfile.DuongHuyet,
                    systolicBP = healthProfile.HuyetApTamThu,
                    diastolicBP = healthProfile.HuyetApTamTruong,
                    lastUpdated = healthProfile.NgayCapNhat
                } : null,
                healthMetrics = healthMetrics.Select(m => new
                {
                    type = m.LoaiChiSo,
                    value = m.GiaTri,
                    date = m.ThoiGianDo
                }),
                appointmentHistory = appointmentHistory.Select(a => new
                {
                    id = a.Id,
                    startTime = a.ThoiGianBatDau,
                    endTime = a.ThoiGianKetThuc,
                    status = a.TrangThai,
                    note = a.GhiChu
                }),
                consultationHistory = consultationHistory.Select(c => new
                {
                    id = c.Id,
                    question = c.CauHoi,
                    answer = c.CauTraLoi,
                    askDate = c.NgayTao,
                    answerDate = c.NgayTraLoi
                })
            });
        }
    }
}
