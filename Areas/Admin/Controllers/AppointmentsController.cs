using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Areas.Admin.Models;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ApplicationDbContext context, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Appointments
        public async Task<IActionResult> Index(string searchTerm = "", string statusFilter = "all", DateTime? dateFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.LichHens
                    .Include(l => l.NguoiDung)
                    .Include(l => l.ChuyenGia)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(l =>
                        (l.NguoiDung != null && l.NguoiDung.HoTen.Contains(searchTerm)) ||
                        (l.ChuyenGia != null && l.ChuyenGia.HoTen.Contains(searchTerm)) ||
                        l.LyDo.Contains(searchTerm) ||
                        l.DiaDiem.Contains(searchTerm));
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
                {
                    query = query.Where(l => l.TrangThai == statusFilter);
                }

                // Apply date filter
                if (dateFilter.HasValue)
                {
                    query = query.Where(l => l.NgayGioHen.Date == dateFilter.Value.Date);
                }

                var totalRecords = await query.CountAsync();
                var appointments = await query
                    .OrderByDescending(l => l.NgayGioHen)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new AppointmentListItemViewModel
                    {
                        Id = l.LichHenId,
                        PatientName = l.NguoiDung != null ? l.NguoiDung.HoTen : "Không xác định",
                        PatientAvatar = "~/images/default-avatar.png", // Default avatar
                        DoctorName = l.ChuyenGia != null ? l.ChuyenGia.HoTen : "Chưa phân công",
                        DateTime = l.NgayGioHen,
                        Location = l.DiaDiem,
                        Reason = l.LyDo,
                        Status = l.TrangThai,
                        StatusClass = GetStatusClass(l.TrangThai)
                    })
                    .ToListAsync();

                var viewModel = new AppointmentListViewModel
                {
                    Appointments = appointments,
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    DateFilter = dateFilter,
                    CurrentPage = pageNumber,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Stats = new AppointmentStats
                    {
                        TotalAppointments = await _context.LichHens.CountAsync(),
                        UpcomingAppointments = await _context.LichHens.CountAsync(l => l.NgayGioHen > DateTime.Now && l.TrangThai == "Chờ xác nhận"),
                        CompletedAppointments = await _context.LichHens.CountAsync(l => l.TrangThai == "Hoàn thành"),
                        CanceledAppointments = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã hủy")
                    }
                };

                // Calculate growth rates (simplified - comparing with last month)
                var lastMonth = DateTime.Now.AddMonths(-1);
                var thisMonth = DateTime.Now;

                viewModel.Stats.UpcomingGrowthRate = await CalculateGrowthRate("Chờ xác nhận", lastMonth, thisMonth);
                viewModel.Stats.CompletedGrowthRate = await CalculateGrowthRate("Hoàn thành", lastMonth, thisMonth);
                viewModel.Stats.CanceledGrowthRate = await CalculateGrowthRate("Đã hủy", lastMonth, thisMonth);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointments list");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách lịch hẹn.";
                return View(new AppointmentListViewModel
                {
                    Appointments = new List<AppointmentListItemViewModel>(),
                    Stats = new AppointmentStats(),
                    SearchTerm = "",
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                });
            }
        }

        // GET: Admin/Appointments/Pending
        public async Task<IActionResult> Pending(string searchTerm = "", DateTime? dateFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            return await GetFilteredAppointments("Chờ xác nhận", searchTerm, dateFilter, pageNumber, pageSize);
        }

        // GET: Admin/Appointments/Confirmed
        public async Task<IActionResult> Confirmed(string searchTerm = "", DateTime? dateFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            return await GetFilteredAppointments("Đã xác nhận", searchTerm, dateFilter, pageNumber, pageSize);
        }

        // GET: Admin/Appointments/Completed
        public async Task<IActionResult> Completed(string searchTerm = "", DateTime? dateFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            return await GetFilteredAppointments("Hoàn thành", searchTerm, dateFilter, pageNumber, pageSize);
        }

        // Helper method for filtered appointment views
        private async Task<IActionResult> GetFilteredAppointments(string status, string searchTerm = "", DateTime? dateFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.LichHens
                    .Include(l => l.NguoiDung)
                    .Include(l => l.ChuyenGia)
                    .Where(l => l.TrangThai == status)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(l =>
                        (l.NguoiDung != null && l.NguoiDung.HoTen.Contains(searchTerm)) ||
                        (l.ChuyenGia != null && l.ChuyenGia.HoTen.Contains(searchTerm)) ||
                        l.LyDo.Contains(searchTerm) ||
                        l.DiaDiem.Contains(searchTerm));
                }

                // Apply date filter
                if (dateFilter.HasValue)
                {
                    var startDate = dateFilter.Value.Date;
                    var endDate = startDate.AddDays(1);
                    query = query.Where(l => l.NgayGioHen >= startDate && l.NgayGioHen < endDate);
                }

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var appointments = await query
                    .OrderByDescending(l => l.NgayGioHen)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new AppointmentListItemViewModel
                    {
                        Id = l.LichHenId,
                        PatientName = l.NguoiDung != null ? l.NguoiDung.HoTen : "Không xác định",
                        PatientAvatar = "~/images/default-avatar.png",
                        DoctorName = l.ChuyenGia != null ? l.ChuyenGia.HoTen : "Chưa phân công",
                        DateTime = l.NgayGioHen,
                        Location = l.DiaDiem,
                        Reason = l.LyDo,
                        Status = l.TrangThai,
                        StatusClass = GetStatusClass(l.TrangThai)
                    })
                    .ToListAsync();

                // Calculate stats for this status
                var stats = await CalculateAppointmentStats();

                var viewModel = new AppointmentListViewModel
                {
                    Appointments = appointments,
                    Stats = stats,
                    SearchTerm = searchTerm,
                    StatusFilter = status,
                    DateFilter = dateFilter,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                ViewData["Title"] = $"Lịch hẹn - {status}";
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading {status.ToLower()} appointments");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi tải danh sách lịch hẹn {status.ToLower()}.";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Appointments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var appointment = await _context.LichHens
                    .Include(l => l.NguoiDung)
                    .Include(l => l.ChuyenGia)
                    .FirstOrDefaultAsync(l => l.LichHenId == id);

                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new AppointmentDetailViewModel
                {
                    Id = appointment.LichHenId,
                    PatientName = appointment.NguoiDung?.HoTen ?? "Không xác định",
                    PatientEmail = appointment.NguoiDung?.Email ?? "",
                    PatientPhone = appointment.NguoiDung?.PhoneNumber ?? "",
                    DoctorName = appointment.ChuyenGia?.HoTen ?? "Chưa phân công",
                    DoctorEmail = appointment.ChuyenGia?.Email ?? "",
                    DateTime = appointment.NgayGioHen,
                    Location = appointment.DiaDiem,
                    Reason = appointment.LyDo,
                    Status = appointment.TrangThai,
                    Diagnosis = appointment.ChanDoan,
                    Prescription = appointment.DonThuoc,
                    Notes = appointment.GhiChu,
                    StatusClass = GetStatusClass(appointment.TrangThai)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointment details for ID: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết lịch hẹn.";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Appointments/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new CreateAppointmentViewModel
                {
                    AvailableDoctors = await _context.NguoiDungs
                        .Where(u => u.VaiTroId == 2)
                        .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = u.Id,
                            Text = u.HoTen
                        })
                        .ToListAsync(),
                    AvailablePatients = await _context.NguoiDungs
                        .Where(u => u.VaiTroId == 3)
                        .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = u.Id,
                            Text = u.HoTen
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create appointment form");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo lịch hẹn.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var appointment = new LichHen
                    {
                        NguoiDungId = model.PatientId,
                        ChuyenGiaId = model.DoctorId,
                        NgayGioHen = model.DateTime,
                        DiaDiem = model.Location,
                        LyDo = model.Reason,
                        TrangThai = "Chờ xác nhận",
                        ChanDoan = model.Diagnosis,
                        DonThuoc = model.Prescription,
                        GhiChu = model.Notes
                    };

                    _context.LichHens.Add(appointment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo lịch hẹn thành công.";
                    return RedirectToAction("Details", new { id = appointment.LichHenId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating appointment");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo lịch hẹn. Vui lòng thử lại.");
                }
            }

            // Reload dropdowns if validation failed
            model.AvailableDoctors = await _context.NguoiDungs
                .Where(u => u.VaiTroId == 2)
                .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = u.HoTen
                })
                .ToListAsync();

            model.AvailablePatients = await _context.NguoiDungs
                .Where(u => u.VaiTroId == 3)
                .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = u.HoTen
                })
                .ToListAsync();

            return View(model);
        }

        // GET: Admin/Appointments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var appointment = await _context.LichHens.FindAsync(id);

                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new EditAppointmentViewModel
                {
                    Id = appointment.LichHenId,
                    PatientId = appointment.NguoiDungId,
                    DoctorId = appointment.ChuyenGiaId,
                    DateTime = appointment.NgayGioHen,
                    Location = appointment.DiaDiem,
                    Reason = appointment.LyDo,
                    Status = appointment.TrangThai,
                    Diagnosis = appointment.ChanDoan,
                    Prescription = appointment.DonThuoc,
                    Notes = appointment.GhiChu,
                    AvailableDoctors = await _context.NguoiDungs
                        .Where(u => u.VaiTroId == 2)
                        .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = u.Id,
                            Text = u.HoTen,
                            Selected = u.Id == appointment.ChuyenGiaId
                        })
                        .ToListAsync(),
                    AvailablePatients = await _context.NguoiDungs
                        .Where(u => u.VaiTroId == 3)
                        .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = u.Id,
                            Text = u.HoTen,
                            Selected = u.Id == appointment.NguoiDungId
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit appointment form for ID: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa lịch hẹn.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAppointmentViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "ID không khớp.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var appointment = await _context.LichHens.FindAsync(id);

                    if (appointment == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn này.";
                        return RedirectToAction("Index");
                    }

                    appointment.NguoiDungId = model.PatientId;
                    appointment.ChuyenGiaId = model.DoctorId;
                    appointment.NgayGioHen = model.DateTime;
                    appointment.DiaDiem = model.Location;
                    appointment.LyDo = model.Reason;
                    appointment.TrangThai = model.Status;
                    appointment.ChanDoan = model.Diagnosis;
                    appointment.DonThuoc = model.Prescription;
                    appointment.GhiChu = model.Notes;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật lịch hẹn thành công.";
                    return RedirectToAction("Details", new { id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating appointment ID: {Id}", id);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật lịch hẹn. Vui lòng thử lại.");
                }
            }

            // Reload dropdowns if validation failed
            model.AvailableDoctors = await _context.NguoiDungs
                .Where(u => u.VaiTroId == 2)
                .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = u.HoTen,
                    Selected = u.Id == model.DoctorId
                })
                .ToListAsync();

            model.AvailablePatients = await _context.NguoiDungs
                .Where(u => u.VaiTroId == 3)
                .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = u.HoTen,
                    Selected = u.Id == model.PatientId
                })
                .ToListAsync();

            return View(model);
        }

        // POST: Admin/Appointments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var appointment = await _context.LichHens.FindAsync(id);

                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn này.";
                    return RedirectToAction("Index");
                }

                _context.LichHens.Remove(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa lịch hẹn thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment ID: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa lịch hẹn.";
                return RedirectToAction("Index");
            }
        }

        // Helper methods
        private async Task<AppointmentStats> CalculateAppointmentStats()
        {
            try
            {
                var now = DateTime.Now;
                var thisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local);
                var lastMonth = thisMonth.AddMonths(-1);

                var stats = new AppointmentStats();
                stats.TotalAppointments = await _context.LichHens.CountAsync();
                stats.PendingAppointments = await _context.LichHens.CountAsync(l => l.TrangThai == "Chờ xác nhận");
                stats.ConfirmedAppointments = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã xác nhận");
                stats.CompletedAppointments = await _context.LichHens.CountAsync(l => l.TrangThai == "Hoàn thành");
                stats.UpcomingAppointments = await _context.LichHens.CountAsync(l => l.NgayGioHen > DateTime.Now && l.TrangThai == "Chờ xác nhận");
                stats.GrowthRate = await CalculateGrowthRate("Hoàn thành", lastMonth, thisMonth);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating appointment stats");
                return new AppointmentStats();
            }
        }

        private static string GetStatusClass(string status)
        {
            return status switch
            {
                "Chờ xác nhận" => "bg-warning",
                "Đã xác nhận" => "bg-info",
                "Hoàn thành" => "bg-success",
                "Đã hủy" => "bg-danger",
                _ => "bg-secondary"
            };
        }

        private async Task<double> CalculateGrowthRate(string status, DateTime lastMonth, DateTime thisMonth)
        {
            try
            {
                var lastMonthCount = await _context.LichHens
                    .CountAsync(l => l.TrangThai == status &&
                                   l.NgayGioHen >= lastMonth.AddMonths(-1) &&
                                   l.NgayGioHen < lastMonth);

                var thisMonthCount = await _context.LichHens
                    .CountAsync(l => l.TrangThai == status &&
                                   l.NgayGioHen >= thisMonth.AddMonths(-1) &&
                                   l.NgayGioHen < thisMonth);

                if (lastMonthCount == 0) return thisMonthCount > 0 ? 100 : 0;

                return Math.Round(((double)(thisMonthCount - lastMonthCount) / lastMonthCount) * 100, 1);
            }
            catch
            {
                return 0;
            }
        }
    }
}