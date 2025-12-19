using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DoAnChamSocSucKhoe.Models; // Added correct model namespace
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using System;
using System.Security.Claims;
using DoAnChamSocSucKhoe.Areas.Patient.Models;

namespace DoAnChamSocSucKhoe.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Policy = "CanBookAppointment")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
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

            // Use LichHens DbSet and LichHen properties
            var appointments = await _context.LichHens
                .Include(a => a.ChuyenGia) // Use ChuyenGia navigation property
                .Where(a => a.NguoiDungId == targetUserId)
                .OrderByDescending(a => a.NgayGioHen) // Use NgayGioHen property
                .ToListAsync();

            // TODO: Need to map LichHen to a ViewModel suitable for the View, 
            // as the View likely expects properties from the old Appointment model.
            // For now, returning the LichHen list directly might cause view errors.
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

            // Use LichHens DbSet and LichHen properties
            var events = await _context.LichHens
                .Include(a => a.ChuyenGia) // Include ChuyenGia to get the name
                .Where(a => a.NguoiDungId == targetUserId)
                .Select(a => new
                {
                    // Assuming LyDo can be used as title, or construct a title
                    title = $"{(a.ChuyenGia != null ? a.ChuyenGia.HoTen : "N/A")} - {a.LyDo}",
                    start = a.NgayGioHen, // Use NgayGioHen directly
                    // Assuming a fixed duration of 30 minutes for now
                    end = a.NgayGioHen.AddMinutes(30),
                    // Map TrangThai to CSS class using ternary operator
                    className = $"fc-event-{(a.TrangThai == null ? "unknown" : a.TrangThai.ToLower())}"
                })
                .ToListAsync();

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors(string specialty)
        {
            // Get users in "Doctor" role
            var doctorsInRole = await _userManager.GetUsersInRoleAsync("Doctor");
            
            // Get profiles
            var profiles = await _context.ChuyenGias.ToListAsync();

            var doctorList = doctorsInRole.Select(u => {
                var profile = profiles.FirstOrDefault(p => p.NguoiDungId == u.Id);
                return new {
                    User = u,
                    Profile = profile,
                    Specialty = profile?.ChuyenKhoa ?? "Khác" // Default
                };
            });

            if (!string.IsNullOrEmpty(specialty))
            {
                doctorList = doctorList.Where(d => d.Specialty == specialty);
            }

            var result = doctorList.Select(d => new {
                id = d.User.Id,
                name = d.Profile?.HoTen ?? d.User.HoTen,
                specialty = d.Specialty,
                rating = 4.5
            }).ToList();

            return Json(result);
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

            // Define working hours (e.g., 8:00 - 17:00, 30 min slots)
            var startHour = 8;
            var endHour = 17;
            var slotDuration = 30;

            var slots = new List<object>();
            
            // Get existing appointments for this doctor on this date
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
                    
                    // Check availability
                    bool isBooked = existingAppointments.Any(t => Math.Abs((t - timeSpan).TotalMinutes) < slotDuration);
                    
                    // Also check if it's in the past (if today)
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

                    // Check if the current user is a caregiver acting on behalf of a patient
                    var linkedPatient = await _context.NguoiChamSocBenhNhans
                        .FirstOrDefaultAsync(x => x.NguoiChamSocId == userId);

                    if (linkedPatient != null && linkedPatient.BenhNhanId != null)
                    {
                        targetUserId = linkedPatient.BenhNhanId;
                    }

                    // Parse Date and Time
                    // Try multiple formats just in case
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
                                // Store extra info in GhiChu since LichHen might not have specific columns
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
            var appointment = _context.LichHens.Find(id); // Use LichHens DbSet
            if (appointment != null)
            {
                appointment.TrangThai = status; // Map status to TrangThai
                                                // LichHen model doesn't have UpdatedAt. Remove or add if necessary.
                                                // appointment.UpdatedAt = DateTime.Now; 

                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Appointment not found." });
        }

        [HttpPost]
        public IActionResult Cancel(int id, string reason)
        {
            var appointment = _context.LichHens.Find(id); // Use LichHens DbSet
            if (appointment != null)
            {
                appointment.TrangThai = "Đã hủy"; // Assuming "Đã hủy" is the cancelled state
                // Map reason to LyDo or add a separate Notes property if needed
                appointment.LyDo = $"Cancelled: {reason}"; // Append reason to LyDo or handle differently
                                                           // LichHen model doesn't have UpdatedAt. Remove or add if necessary.
                                                           // appointment.UpdatedAt = DateTime.Now; 

                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Appointment not found." });
        }
    }
}