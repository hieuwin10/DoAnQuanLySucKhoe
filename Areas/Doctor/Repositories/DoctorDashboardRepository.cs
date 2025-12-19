using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using DoAnChamSocSucKhoe.Areas.Doctor.Models;
using DoAnChamSocSucKhoe.Data;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Repositories
{
    public class DoctorDashboardRepository : IDoctorDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public DoctorDashboardRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<DoctorDashboardViewModel> GetDashboardDataAsync(string doctorId)
        {
            var cacheKey = $"doctor_dashboard_{doctorId}";

            if (_cache.TryGetValue(cacheKey, out DoctorDashboardViewModel? cachedData))
            {
                if (cachedData != null)
                {
                    return cachedData;
                }
            }

            var dashboardData = new DoctorDashboardViewModel
            {
                Profile = await GetDoctorProfileAsync(doctorId),
                Stats = await GetDashboardStatsAsync(doctorId),
                UpcomingAppointments = await GetUpcomingAppointmentsAsync(doctorId),
                RecentPatients = await GetRecentPatientsAsync(doctorId),
                RecentConsultations = await GetRecentConsultationsAsync(doctorId),
                Notifications = await GetNotificationsAsync(doctorId)
            };

            _cache.Set(cacheKey, dashboardData, _cacheDuration);
            return dashboardData;
        }

        public async Task<DoctorProfile> GetDoctorProfileAsync(string doctorId)
        {
            var cacheKey = $"doctor_profile_{doctorId}";

            if (_cache.TryGetValue(cacheKey, out DoctorProfile? cachedProfile))
            {
                if (cachedProfile != null)
                {
                    return cachedProfile;
                }
            }

            var profile = await _context.NguoiDungs
                .Where(d => d.Id == doctorId)
                .Select(d => new DoctorProfile
                {
                    FullName = d.HoTen,
                    Specialization = "Unknown",
                    YearsOfExperience = 0,
                    Hospital = "Unknown",
                    Department = "Unknown",
                    Email = d.Email ?? "no-email@example.com",
                    PhoneNumber = d.PhoneNumber ?? "No phone number",
                    ProfileImageUrl = d.AnhDaiDien ?? "default_profile_image.png",
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                profile = new DoctorProfile
                {
                    FullName = "Unknown Doctor",
                    Specialization = "Unknown",
                    YearsOfExperience = 0,
                    Hospital = "Unknown",
                    Department = "Unknown",
                    Email = "no-email@example.com",
                    PhoneNumber = "No phone number",
                    ProfileImageUrl = "default_profile_image.png"
                };
            }

            _cache.Set(cacheKey, profile, _cacheDuration);
            return profile;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync(string doctorId)
        {
            var cacheKey = $"doctor_stats_{doctorId}";

            if (_cache.TryGetValue(cacheKey, out DashboardStats? cachedStats))
            {
                if (cachedStats != null)
                {
                    return cachedStats;
                }
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var stats = new DashboardStats
            {
                TotalAppointments = await _context.LichHens
                    .CountAsync(a => a.ChuyenGiaId.ToString() == doctorId),

                TodayAppointments = await _context.LichHens
                    .CountAsync(a => a.ChuyenGiaId.ToString() == doctorId &&
                                     a.NgayGioHen >= today &&
                                     a.NgayGioHen < tomorrow),

                TotalPatients = await _context.NguoiDungs
                    .CountAsync(p => p.VaiTroId == 3),

                ActiveConsultations = await _context.TuVanSucKhoes
                    .CountAsync(c => c.ChuyenGiaId.ToString() == doctorId &&
                                     c.TrangThai == 1),

                PendingTasks = 0
            };

            _cache.Set(cacheKey, stats, _cacheDuration);
            return stats;
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsAsync(string doctorId, int limit = 5)
        {
            var cacheKey = $"doctor_appointments_{doctorId}_{limit}";

            if (_cache.TryGetValue(cacheKey, out List<Appointment>? cachedAppointments))
            {
                if (cachedAppointments != null)
                {
                    return cachedAppointments;
                }
            }

            var appointments = await _context.LichHens
                .Where(a => a.ChuyenGiaId == doctorId &&
                            a.NgayGioHen >= DateTime.Now)
                .OrderBy(a => a.NgayGioHen)
                .Take(limit)
                .Select(a => new Appointment
                {
                    Id = a.LichHenId,
                    PatientName = a.NguoiDung != null ? a.NguoiDung.HoTen : "Unknown",
                    AppointmentTime = a.NgayHen,
                    Status = a.TrangThai.ToString(),
                    Type = a.LoaiLichHen ?? "Offline",
                    Notes = a.GhiChu ?? "None" // No Notes property in LichHen
                })
                .ToListAsync();

            _cache.Set(cacheKey, appointments, _cacheDuration);
            return appointments;
        }

        public async Task<List<PatientIntermediate>> GetRecentPatientsAsync(string doctorId, int limit = 5)
        {
            var cacheKey = $"doctor_patients_{doctorId}_{limit}";

            if (_cache.TryGetValue(cacheKey, out List<PatientIntermediate>? cachedPatients))
            {
                if (cachedPatients != null)
                {
                    return cachedPatients;
                }
            }

            var patientIntermediates = await _context.NguoiDungs
                .Where(p => p.VaiTroId == 3) // Assuming Patient role has Id 3
                .OrderByDescending(p => p.NgayTao)
                .Take(limit)
                .Select(p => new PatientIntermediate
                {
                    IdString = p.Id,
                    FullName = p.HoTen,
                    ProfileImageUrl = p.AnhDaiDien ?? "default_profile_image.png",
                    LastVisit = p.NgayTao,
                    LastDiagnosis = "Unknown" // You may want to join with another table to get actual diagnosis
                })
                .ToListAsync();

            _cache.Set(cacheKey, patientIntermediates, _cacheDuration);
            return patientIntermediates;
        }

        public async Task<List<Consultation>> GetRecentConsultationsAsync(string doctorId, int limit = 5)
        {
            var consultations = await _context.TuVanSucKhoes
                .Include(c => c.NguoiDung)
                .Include(c => c.ChuyenGia)
                .Where(c => c.ChuyenGiaId == doctorId)
                .OrderByDescending(c => c.NgayTao)
                .Take(limit)
                .Select(c => new Consultation
                {
                    Id = c.TuVanSucKhoeId,
                    PatientName = c.NguoiDung.HoTen,
                    Subject = c.TieuDe,
                    CreatedAt = c.NgayTao,
                    Status = c.TrangThai.ToString(),
                    LastMessage = c.TraLoi ?? "Chưa có trả lời",
                    PatientImageUrl = c.NguoiDung.AnhDaiDien ?? "/images/default-avatar.png"
                })
                .ToListAsync();

            return consultations;
        }

        public async Task<List<Notification>> GetNotificationsAsync(string doctorId, int limit = 10)
        {
            var cacheKey = $"doctor_notifications_{doctorId}_{limit}";

            if (_cache.TryGetValue(cacheKey, out List<Notification>? cachedNotifications))
            {
                if (cachedNotifications != null)
                {
                    return cachedNotifications;
                }
            }

            var notifications = new List<Notification>(); // No Notifications DbSet

            _cache.Set(cacheKey, notifications, TimeSpan.FromMinutes(1)); // Shorter cache duration for notifications
            await Task.CompletedTask;
            return notifications;
        }
    }
}
