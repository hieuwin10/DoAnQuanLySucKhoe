using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DoAnChamSocSucKhoe.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using System;
using System.Security.Claims;
using DoAnChamSocSucKhoe.Areas.Patient.Models;

namespace DoAnChamSocSucKhoe.Areas.Caregiver.Controllers
{
    [Area("Caregiver")]
    [Authorize(Roles = "Caregiver")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            string targetUserId = userId;

            // Caregiver truy cập thông tin của bệnh nhân được chăm sóc
            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var appointments = await _context.LichHens
                .Include(a => a.ChuyenGia)
                .Where(a => a.NguoiDungId == targetUserId)
                .OrderByDescending(a => a.NgayGioHen)
                .ToListAsync();

            return View(appointments);
        }

        public async Task<IActionResult> GetCalendarEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            string targetUserId = userId;

            var linkedPatient = await _context.NguoiChamSocBenhNhans
                .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

            if (linkedPatient != null && linkedPatient.BenhNhanId != null)
            {
                targetUserId = linkedPatient.BenhNhanId;
            }

            var events = await _context.LichHens
                .Include(a => a.ChuyenGia)
                .Where(a => a.NguoiDungId == targetUserId)
                .Select(a => new
                {
                    title = $"{(a.ChuyenGia != null ? a.ChuyenGia.HoTen : "N/A")} - {a.LyDo}",
                    start = a.NgayGioHen,
                    end = a.NgayGioHen.AddMinutes(30),
                    className = $"fc-event-{(a.TrangThai == null ? "unknown" : a.TrangThai.ToLower())}"
                })
                .ToListAsync();

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors(string specialty)
        {
            var query = _context.ChuyenGias
                .Include(c => c.NguoiDung)
                .Where(c => c.TrangThai == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(specialty))
            {
                query = query.Where(c => c.ChuyenKhoa == specialty);
            }

            var doctors = await query.Select(c => new
            {
                id = c.NguoiDungId,
                name = c.HoTen ?? c.NguoiDung.HoTen,
                specialty = c.ChuyenKhoa,
                rating = 4.5
            }).ToListAsync();

            return Json(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeSlots(string date, string doctorId)
        {
            DateTime selectedDate;
            string[] formats = { "d/M/yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };
            
            if (!DateTime.TryParseExact(date, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out selectedDate))
            {
                 if (!DateTime.TryParse(date, out selectedDate))
                    return Json(new List<object>());
            }

            var startHour = 8;
            var endHour = 17;
            var slotDuration = 30;

            var slots = new List<object>();
            
            var existingAppointments = await _context.LichHens
                .Where(a => a.ChuyenGiaId == doctorId && a.NgayHen.Date == selectedDate.Date && a.TrangThai != "Đã hủy")
                .Select(a => a.NgayGioHen.TimeOfDay)
                .ToListAsync();

            for (var hour = startHour; hour < endHour; hour++)
            {
                for (var minute = 0; minute < 60; minute += slotDuration)
                {
                    var timeSpan = new TimeSpan(hour, minute, 0);
                    var timeString = timeSpan.ToString(@"hh\:mm");
                    
                    bool isBooked = existingAppointments.Any(t => Math.Abs((t - timeSpan).TotalMinutes) < slotDuration);
                    bool isPast = selectedDate.Date == DateTime.Today && timeSpan < DateTime.Now.TimeOfDay;

                    slots.Add(new 
                    { 
                        time = timeString,
                        isAvailable = !isBooked && !isPast
                    });
                }
            }

            return Json(slots);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null) return Unauthorized();
                    string targetUserId = userId;

                    var linkedPatient = await _context.NguoiChamSocBenhNhans
                        .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

                    if (linkedPatient != null && linkedPatient.BenhNhanId != null)
                    {
                        targetUserId = linkedPatient.BenhNhanId;
                    }

                    string[] formats = { "d/M/yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };
                    if (DateTime.TryParseExact(model.NgayHen, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        if (TimeSpan.TryParse(model.GioHen, out TimeSpan time))
                        {
                            var dateTime = date.Add(time);
                            
                            var appointment = new LichHen
                            {
                                NguoiDungId = targetUserId,
                                ChuyenGiaId = model.ChuyenGiaId,
                                NgayGioHen = dateTime,
                                NgayHen = date,
                                DiaDiem = model.DiaDiem,
                                LyDo = model.LyDo,
                                TrangThai = "Chờ xác nhận",
                                GhiChu = $"Loại: {model.LoaiLichHen}, Chuyên khoa: {model.ChuyenKhoa}, BHYT: {(model.SuDungBHYT ? "Có" : "Không")}, Nhắc nhở: {(model.NhanNhacNho ? "Có" : "Không")}"
                            };

                            _context.LichHens.Add(appointment);
                            await _context.SaveChangesAsync();

                            return Json(new { success = true });
                        }
                        else 
                        {
                             return Json(new { success = false, errors = new[] { "Định dạng giờ không hợp lệ (HH:mm)" } });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, errors = new[] { $"Định dạng ngày không hợp lệ: {model.NgayHen}" } });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errors = new[] { "Lỗi server: " + ex.Message } });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var appointment = _context.LichHens.Find(id);
            if (appointment != null)
            {
                appointment.TrangThai = status;
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Appointment not found." });
        }

        [HttpPost]
        public IActionResult Cancel(int id, string reason)
        {
            var appointment = _context.LichHens.Find(id);
            if (appointment != null)
            {
                appointment.TrangThai = "Đã hủy";
                appointment.LyDo = $"Cancelled: {reason}";
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Appointment not found." });
        }
    }
}
