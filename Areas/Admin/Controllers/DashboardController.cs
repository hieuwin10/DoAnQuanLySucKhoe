using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Areas.Admin.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public DashboardController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // Try to get cached data first
            DashboardViewModel? dashboardData;
            if (!_cache.TryGetValue("DashboardData", out dashboardData))
            {
                // Calculate growth rates
                var lastMonth = DateTime.Now.AddMonths(-1);
                var currentMonth = DateTime.Now;

                // Get total counts
                var totalUsers = await _context.Users.CountAsync();
                var totalDoctors = await _context.NguoiDungs.CountAsync(d => d.VaiTroId == 2); // Assuming Doctor role has Id 2
                var totalPatients = await _context.NguoiDungs.CountAsync(p => p.VaiTroId == 3); // Assuming Patient role has Id 3
                var totalAppointments = await _context.LichHens.CountAsync();

                // Get previous month counts for growth calculation
                var lastMonthUsers = await _context.NguoiDungs
                    .Where(u => _context.Users.Any(user => user.Id == u.Id && u.NgayTao < lastMonth))
                    .CountAsync();
                var lastMonthDoctors = await _context.NguoiDungs
                    .Where(d => d.VaiTroId == 2 && _context.Users.Any(user => user.Id == d.Id && d.NgayTao < lastMonth)) // Assuming Doctor role has Id 2
                    .CountAsync();
                var lastMonthPatients = await _context.NguoiDungs
                    .Where(p => p.VaiTroId == 3 && _context.Users.Any(user => user.Id == p.Id && p.NgayTao < lastMonth)) // Assuming Patient role has Id 3
                    .CountAsync();
                var lastMonthAppointments = await _context.LichHens
                    .Where(a => a.NgayGioHen < lastMonth)
                    .CountAsync();

                // Calculate growth rates
                var userGrowthRate = lastMonthUsers > 0 ?
                    (decimal)(totalUsers - lastMonthUsers) / lastMonthUsers * 100 : 0;
                var doctorGrowthRate = lastMonthDoctors > 0 ?
                    (decimal)(totalDoctors - lastMonthDoctors) / lastMonthDoctors * 100 : 0;
                var patientGrowthRate = lastMonthPatients > 0 ?
                    (decimal)(totalPatients - lastMonthPatients) / lastMonthPatients * 100 : 0;
                var appointmentGrowthRate = lastMonthAppointments > 0 ?
                    (decimal)(totalAppointments - lastMonthAppointments) / lastMonthAppointments * 100 : 0;

                // Get recent appointments
                var recentAppointments = await _context.LichHens
                    .OrderByDescending(a => a.NgayGioHen)
                    .Take(5)
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.LichHenId,
                        PatientName = a.NguoiDung != null ? a.NguoiDung.HoTen : "Unknown",
                        DoctorName = a.ChuyenGia != null ? a.ChuyenGia.HoTen : "Unknown",
                        Specialty = "Unknown", // No Specialty property in NguoiDung
                        AppointmentDate = a.NgayGioHen,
                        AppointmentTime = TimeSpan.FromTicks(a.NgayGioHen.Ticks), // Convert DateTime to TimeSpan
                        Status = a.TrangThai,
                        AppointmentStatus = a.TrangThai // Assuming AppointmentStatus corresponds to TrangThai
                    })
                    .ToListAsync();

                // Get daily stats for the last 7 days
                var dailyStats = await _context.LichHens
                    .Where(a => a.NgayGioHen >= DateTime.Now.AddDays(-7))
                    .GroupBy(a => a.NgayGioHen.Date)
                    .Select(g => new DailyStats
                    {
                        Date = g.Key,
                        NewAppointments = g.Count(),
                        CompletedAppointments = g.Count(a => a.TrangThai == "Completed")
                    })
                    .ToListAsync();

                // Get monthly stats for the last 6 months
                var monthlyStats = await _context.LichHens
                    .Where(a => a.NgayGioHen >= DateTime.Now.AddMonths(-6))
                    .GroupBy(a => new { a.NgayGioHen.Year, a.NgayGioHen.Month })
                    .Select(g => new MonthlyStats
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month}",
                        TotalAppointments = g.Count(),
                        Revenue = 0 // No Fee property in LichHen
                    })
                    .ToListAsync();

                // Get appointment status stats
                var statusStats = await _context.LichHens
                    .GroupBy(a => a.TrangThai)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Lấy số lượng tư vấn mới (chờ xử lý)
                var newConsultationCount = await _context.TuVanSucKhoes.CountAsync(t => t.TrangThai == 0);
                // Lấy số lượng lịch hẹn chờ xác nhận
                var pendingAppointmentCount = await _context.LichHens.CountAsync(a => a.TrangThai == "Chờ xác nhận" || a.TrangThai == "Pending");

                var appointmentStatusStats = new AppointmentStatusStats
                {
                    Total = totalAppointments,
                    Confirmed = statusStats.FirstOrDefault(s => s != null && s.Status == "Confirmed")?.Count ?? 0,
                    Pending = statusStats.FirstOrDefault(s => s != null && (s.Status == "Pending" || s.Status == "Chờ xác nhận"))?.Count ?? 0,
                    Cancelled = statusStats.FirstOrDefault(s => s != null && s.Status == "Cancelled")?.Count ?? 0,
                    Completed = statusStats.FirstOrDefault(s => s != null && s.Status == "Completed")?.Count ?? 0
                };

                // Get specialty stats
                var specialtyStats = new List<SpecialtyStats>(); // No Specialty in NguoiDung and no Doctor navigation in LichHen

                dashboardData = new DashboardViewModel
                {
                    TotalUsers = totalUsers,
                    TotalDoctors = totalDoctors,
                    TotalPatients = totalPatients,
                    TotalAppointments = totalAppointments,
                    UserGrowthRate = userGrowthRate,
                    DoctorGrowthRate = doctorGrowthRate,
                    PatientGrowthRate = patientGrowthRate,
                    AppointmentGrowthRate = appointmentGrowthRate,
                    RecentAppointments = recentAppointments,
                    DailyStats = dailyStats,
                    MonthlyStats = monthlyStats,
                    AppointmentStatusStats = appointmentStatusStats,
                    SpecialtyStats = specialtyStats,
                    NewConsultationCount = newConsultationCount,
                    PendingAppointmentCount = pendingAppointmentCount
                };

                // Truyền số badge sang ViewBag để layout sidebar dùng được
                ViewBag.NewConsultationCount = newConsultationCount;
                ViewBag.PendingAppointmentCount = pendingAppointmentCount;

                // Cache the data for 5 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _cache.Set("DashboardData", dashboardData, cacheEntryOptions);
            }
            else
            {
                // Lấy từ cache nếu có
                ViewBag.NewConsultationCount = dashboardData?.NewConsultationCount ?? 0;
                ViewBag.PendingAppointmentCount = dashboardData?.PendingAppointmentCount ?? 0;
            }

            return View(dashboardData ?? new DashboardViewModel());
        }
    }
}