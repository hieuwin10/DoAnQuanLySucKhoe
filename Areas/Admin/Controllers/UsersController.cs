using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Areas.Admin.Models;
using DoAnChamSocSucKhoe.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using DoAnChamSocSucKhoe.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Claims;

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UsersController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(
            ApplicationDbContext context,
            UserManager<NguoiDung> userManager,
            IMemoryCache cache,
            ILogger<UsersController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _cache = cache;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchTerm, string statusFilter = "all", int pageNumber = 1)
        {
            try
            {
                // Set page size
                const int pageSize = 10;

                // Create view model
                var viewModel = new UserListViewModel
                {
                    SearchTerm = searchTerm ?? string.Empty,
                    StatusFilter = statusFilter,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Get user statistics
                await LoadUserStatistics(viewModel);

                // Apply filters
                var query = _context.NguoiDungs.AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(u =>
                        u.HoTen.ToLower().Contains(searchTerm) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchTerm)));
                }

                // Status filter
                switch (statusFilter)
                {
                    case "active":
                        query = query.Where(u => u.TrangThai == "Hoạt động");
                        break;
                    case "pending":
                        query = query.Where(u => u.TrangThai == "Chờ duyệt");
                        break;
                    case "inactive":
                        query = query.Where(u => u.TrangThai == "Không hoạt động");
                        break;
                }

                // Get total count for pagination
                var totalUsers = await query.CountAsync();
                viewModel.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

                // Get users for current page
                var users = await query
                    .Include(u => u.VaiTro) // Thêm dòng này để tải thông tin VaiTro
                    .OrderByDescending(u => u.NgayTao)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        FullName = u.HoTen,
                        Email = u.Email ?? string.Empty,
                        Role = u.VaiTro != null ? u.VaiTro.TenVaiTro : "Chưa gán",
                        Status = u.TrangThai,
                        LastActivity = u.NgayCapNhat,
                        Avatar = u.AnhDaiDien ?? "/images/avatar-default.png"
                    })
                    .ToListAsync();

                viewModel.Users = users;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang quản lý người dùng");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách người dùng.";
                return View(new UserListViewModel());
            }
        }

        private async Task LoadUserStatistics(UserListViewModel viewModel)
        {
            try
            {
                // Try to get from cache first
                UserStats? stats;
                if (!_cache.TryGetValue("UserStats", out stats))
                {
                    var lastMonth = DateTime.Now.AddMonths(-1);

                    // Get current counts
                    var totalUsers = await _context.NguoiDungs.CountAsync();
                    var totalDoctors = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 2); // Assuming Doctor role Id is 2
                    var totalPatients = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 3); // Assuming Patient role Id is 3
                    var inactiveUsers = await _context.NguoiDungs.CountAsync(u => u.TrangThai == "Không hoạt động");

                    // Get last month counts for growth calculation
                    var lastMonthUsers = await _context.NguoiDungs
                        .Where(u => u.NgayTao < lastMonth)
                        .CountAsync();
                    var lastMonthDoctors = await _context.NguoiDungs
                        .Where(u => u.VaiTroId == 2 && u.NgayTao < lastMonth)
                        .CountAsync();
                    var lastMonthPatients = await _context.NguoiDungs
                        .Where(u => u.VaiTroId == 3 && u.NgayTao < lastMonth)
                        .CountAsync();
                    var lastMonthInactive = await _context.NguoiDungs
                        .Where(u => u.TrangThai == "Không hoạt động" && u.NgayCapNhat < lastMonth)
                        .CountAsync();

                    // Calculate growth rates
                    var totalGrowthRate = lastMonthUsers > 0 ?
                        (decimal)(totalUsers - lastMonthUsers) / lastMonthUsers * 100 : 0;
                    var doctorGrowthRate = lastMonthDoctors > 0 ?
                        (decimal)(totalDoctors - lastMonthDoctors) / lastMonthDoctors * 100 : 0;
                    var patientGrowthRate = lastMonthPatients > 0 ?
                        (decimal)(totalPatients - lastMonthPatients) / lastMonthPatients * 100 : 0;
                    var inactiveGrowthRate = lastMonthInactive > 0 ?
                        (decimal)(inactiveUsers - lastMonthInactive) / lastMonthInactive * 100 : 0;

                    stats = new UserStats
                    {
                        TotalUsers = totalUsers,
                        TotalDoctors = totalDoctors,
                        TotalPatients = totalPatients,
                        InactiveUsers = inactiveUsers,
                        TotalGrowthRate = totalGrowthRate,
                        DoctorGrowthRate = doctorGrowthRate,
                        PatientGrowthRate = patientGrowthRate,
                        InactiveGrowthRate = inactiveGrowthRate
                    };

                    // Cache for 5 minutes
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    _cache.Set("UserStats", stats, cacheOptions);
                }

                viewModel.Stats = stats ?? new UserStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải thống kê người dùng");
                viewModel.Stats = new UserStats();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string userId, string status)
        {
            try
            {
                var user = await _context.NguoiDungs.FindAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                user.TrangThai = status;
                user.NgayCapNhat = DateTime.Now;

                await _context.SaveChangesAsync();

                // Invalidate cache
                _cache.Remove("UserStats");

                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi trạng thái người dùng");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                var user = await _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound();
                }

                var viewModel = new UserDetailViewModel
                {
                    Id = user.Id,
                    FullName = user.HoTen,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? "",
                    Role = user.VaiTro?.TenVaiTro ?? "Không xác định",
                    Status = user.TrangThai,
                    RegistrationDate = user.NgayTao,
                    LastActivity = user.NgayCapNhat,
                    Address = user.DiaChi ?? string.Empty,
                    Avatar = !string.IsNullOrEmpty(user.AnhDaiDien)
                        ? user.AnhDaiDien
                        : "/images/avatar-default.png",
                    HasUserPermissions = user.TrangThai == "Hoạt động"
                };

                // Nếu là người chăm sóc, lấy thông tin bệnh nhân được gán
                if (viewModel.Role == "Người chăm sóc" || viewModel.Role == "Caregiver")
                {
                    var assignment = await _context.NguoiChamSocBenhNhans
                        .Include(a => a.BenhNhan)
                        .FirstOrDefaultAsync(a => a.NguoiChamSocId == user.Id);
                    
                    if (assignment != null && assignment.BenhNhan != null)
                    {
                        viewModel.AssignedPatientName = assignment.BenhNhan.HoTen;
                        viewModel.AssignedPatientId = assignment.BenhNhan.Id;
                        viewModel.AssignedPatientAvatar = !string.IsNullOrEmpty(assignment.BenhNhan.AnhDaiDien) 
                            ? assignment.BenhNhan.AnhDaiDien 
                            : "/images/BN.png";

                        // Lấy ID hồ sơ sức khỏe của bệnh nhân
                        var healthProfile = await _context.HoSoSucKhoes
                            .FirstOrDefaultAsync(h => h.NguoiDungId == assignment.BenhNhan.Id);
                        
                        if (healthProfile != null)
                        {
                            viewModel.AssignedPatientHealthProfileId = healthProfile.HoSoSucKhoeId;
                        }
                    }
                }

                // Lấy danh sách quyền của người dùng
                viewModel.Permissions = await GetUserPermissions(user);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết người dùng");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin chi tiết người dùng.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<UserPermission>> GetUserPermissions(NguoiDung user)
        {
            var permissions = new List<UserPermission>();
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            IList<Claim> claims = new List<Claim>();
            if (identityUser != null)
            {
                claims = await _userManager.GetClaimsAsync(identityUser);
            }
            
            // Kiểm tra xem người dùng có được quản lý quyền thủ công không
            bool isPermissionsManaged = claims.Any(c => c.Type == "PermissionsManaged");

            // Quyền xem hồ sơ cá nhân (tất cả người dùng đều có)
            permissions.Add(new UserPermission
            {
                Name = "Xem hồ sơ cá nhân",
                Description = "Người dùng có thể xem thông tin cá nhân của mình",
                IsGranted = true
            });

            // Quyền đặt lịch hẹn
            bool canBookAppointment;
            if (isPermissionsManaged)
            {
                canBookAppointment = claims.Any(c => c.Type == "CanBookAppointment");
            }
            else
            {
                // Logic mặc định dựa trên vai trò
                canBookAppointment = (user.VaiTro?.TenVaiTro == "Bệnh nhân" || user.VaiTro?.TenVaiTro == "Caregiver") && user.TrangThai == "Hoạt động";
            }
            
            permissions.Add(new UserPermission
            {
                Name = "Đặt lịch hẹn",
                Description = "Người dùng có thể đặt lịch hẹn với bác sĩ",
                IsGranted = canBookAppointment
            });

            // Quyền gửi tư vấn
            bool canSendConsultation;
            if (isPermissionsManaged)
            {
                canSendConsultation = claims.Any(c => c.Type == "CanSendConsultation");
            }
            else
            {
                canSendConsultation = (user.VaiTro?.TenVaiTro == "Bệnh nhân" || user.VaiTro?.TenVaiTro == "Caregiver") && user.TrangThai == "Hoạt động";
            }

            permissions.Add(new UserPermission
            {
                Name = "Gửi tư vấn",
                Description = "Người dùng có thể gửi câu hỏi tư vấn cho bác sĩ",
                IsGranted = canSendConsultation
            });

            // Quyền xem lịch sử khám bệnh
            bool canViewMedicalHistory;
            if (isPermissionsManaged)
            {
                canViewMedicalHistory = claims.Any(c => c.Type == "CanViewMedicalHistory");
            }
            else
            {
                canViewMedicalHistory = (user.VaiTro?.TenVaiTro == "Bệnh nhân" ||
                                        user.VaiTro?.TenVaiTro == "Bác sĩ" ||
                                        user.VaiTro?.TenVaiTro == "Caregiver") &&
                                       user.TrangThai == "Hoạt động";
            }

            permissions.Add(new UserPermission
            {
                Name = "Xem lịch sử khám bệnh",
                Description = "Người dùng có thể xem lịch sử khám bệnh",
                IsGranted = canViewMedicalHistory
            });

            // Quyền quản lý tài khoản
            bool canManageAccounts;
            if (isPermissionsManaged)
            {
                canManageAccounts = claims.Any(c => c.Type == "CanManageAccounts");
            }
            else
            {
                canManageAccounts = user.VaiTro?.TenVaiTro == "Admin";
            }

            permissions.Add(new UserPermission
            {
                Name = "Quản lý tài khoản",
                Description = "Người dùng có quyền quản lý tài khoản khác",
                IsGranted = canManageAccounts
            });

            return permissions;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                // Kiểm tra xem người dùng có tồn tại không
                var user = await _context.NguoiDungs.FindAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Kiểm tra xem người dùng có đang không hoạt động không
                if (user.TrangThai != "Không hoạt động")
                {
                    return Json(new { success = false, message = "Chỉ có thể xóa tài khoản người dùng không hoạt động" });
                }

                // Xóa các dữ liệu liên quan trước khi xóa người dùng
                // Lịch hẹn
                var appointments = _context.LichHens.Where(a => a.NguoiDungId == userId);
                _context.LichHens.RemoveRange(appointments);

                // Lịch sử sức khỏe
                var healthHistories = _context.LichSuSucKhoes.Where(h => h.NguoiDungId == userId);
                _context.LichSuSucKhoes.RemoveRange(healthHistories);

                // Hồ sơ sức khỏe
                var healthProfiles = _context.HoSoSucKhoes.Where(h => h.NguoiDungId == userId);
                _context.HoSoSucKhoes.RemoveRange(healthProfiles);

                // Phản hồi sức khỏe
                var feedbacks = _context.PhanHoiSucKhoes.Where(f => f.NguoiDungId == userId);
                _context.PhanHoiSucKhoes.RemoveRange(feedbacks);

                // Tư vấn sức khỏe (nếu là bệnh nhân)
                var healthAdvices = _context.TuVanSucKhoes.Where(t => t.NguoiDungId == userId);
                _context.TuVanSucKhoes.RemoveRange(healthAdvices);

                // Kế hoạch dinh dưỡng
                var nutritionPlans = _context.KeHoachDinhDuongs.Where(k => k.NguoiDungId == userId);
                foreach (var plan in nutritionPlans)
                {
                    // Xóa chi tiết kế hoạch dinh dưỡng
                    var nutritionDetails = _context.ChiTietKeHoachDinhDuongs.Where(c => c.KeHoachDinhDuongId == plan.KeHoachDinhDuongId);
                    _context.ChiTietKeHoachDinhDuongs.RemoveRange(nutritionDetails);
                }
                _context.KeHoachDinhDuongs.RemoveRange(nutritionPlans);

                // Kế hoạch tập luyện
                var exercisePlans = _context.KeHoachTapLuyens.Where(k => k.NguoiDungId == userId);
                foreach (var plan in exercisePlans)
                {
                    // Xóa chi tiết kế hoạch tập luyện
                    var exerciseDetails = _context.ChiTietKeHoachTapLuyens.Where(c => c.KeHoachTapLuyenId == plan.KeHoachTapLuyenId);
                    _context.ChiTietKeHoachTapLuyens.RemoveRange(exerciseDetails);
                }
                _context.KeHoachTapLuyens.RemoveRange(exercisePlans);                // Nhắc nhở sức khỏe
                var reminders = _context.NhacNhoSucKhoes.Where(n => n.UserId == userId);
                _context.NhacNhoSucKhoes.RemoveRange(reminders);

                // Thông báo bác sĩ
                var notifications = _context.ThongBaoBacSis.Where(t => t.NguoiDungId.ToString() == userId);
                _context.ThongBaoBacSis.RemoveRange(notifications);

                // Đánh giá chuyên gia (nếu là bệnh nhân)
                var expertRatings = _context.DanhGiaChuyenGias.Where(d => d.NguoiDungId != null && d.NguoiDungId.Equals(userId));
                _context.DanhGiaChuyenGias.RemoveRange(expertRatings);

                // Xóa người dùng từ bảng NguoiDung
                _context.NguoiDungs.Remove(user);

                // Xóa người dùng từ bảng Identity
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser != null)
                {
                    await _userManager.DeleteAsync(identityUser);
                }

                // Lưu các thay đổi
                await _context.SaveChangesAsync();

                // Xóa cache
                _cache.Remove("UserStats");

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Đã xóa tài khoản người dùng thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa tài khoản người dùng" });
            }
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                var user = await _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound();
                }

                // Lấy danh sách vai trò
                ViewBag.Roles = await _context.VaiTros
                    .Select(r => new SelectListItem
                    {
                        Value = r.VaiTroId.ToString(),
                        Text = r.TenVaiTro,
                        Selected = r.VaiTroId == user.VaiTroId
                    }).ToListAsync();

                // Lấy danh sách trạng thái
                ViewBag.Statuses = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Hoạt động", Text = "Hoạt động", Selected = user.TrangThai == "Hoạt động" },
                    new SelectListItem { Value = "Chờ duyệt", Text = "Chờ duyệt", Selected = user.TrangThai == "Chờ duyệt" },
                    new SelectListItem { Value = "Không hoạt động", Text = "Không hoạt động", Selected = user.TrangThai == "Không hoạt động" }
                };

                // Lấy danh sách bệnh nhân từ Identity Role để đảm bảo chính xác
                var patientsInRole = await _userManager.GetUsersInRoleAsync("Patient");
                var benhNhansInRole = await _userManager.GetUsersInRoleAsync("Bệnh nhân");
                
                // Gộp danh sách và loại bỏ trùng lặp
                var allPatients = patientsInRole.Concat(benhNhansInRole)
                    .GroupBy(u => u.Id)
                    .Select(g => g.First())
                    .ToList();

                ViewBag.Patients = allPatients.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.HoTen + " (" + u.Email + ")"
                }).ToList();

                // Kiểm tra xem user hiện tại có phải là Caregiver và đã được gán bệnh nhân chưa
                string? currentPatientId = null;
                var caregiverRelation = await _context.NguoiChamSocBenhNhans
                    .FirstOrDefaultAsync(x => x.NguoiChamSocId == id);
                if (caregiverRelation != null)
                {
                    currentPatientId = caregiverRelation.BenhNhanId;
                }

                var userRole = user.VaiTro?.TenVaiTro;
                
                // Lấy claims để xác định quyền hạn
                var identityUser = await _userManager.FindByIdAsync(id);
                IList<Claim> claims = new List<Claim>();
                if (identityUser != null)
                {
                    claims = await _userManager.GetClaimsAsync(identityUser);
                }
                bool isPermissionsManaged = claims.Any(c => c.Type == "PermissionsManaged");

                var viewModel = new EditUserViewModel
                {
                    Id = user.Id,
                    FullName = user.HoTen,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? "",
                    RoleId = user.VaiTroId ?? 0,
                    Status = user.TrangThai,
                    Address = user.DiaChi ?? string.Empty,
                    Avatar = !string.IsNullOrEmpty(user.AnhDaiDien)
                        ? user.AnhDaiDien
                        : "/images/avatar-default.png",
                    IsAdmin = userRole == "Admin",
                    IsDoctor = userRole == "Bác sĩ",
                    IsPatient = userRole == "Bệnh nhân",

                    // Thiết lập quyền dựa trên claims hoặc vai trò
                    CanViewProfile = isPermissionsManaged 
                        ? claims.Any(c => c.Type == "CanViewProfile") 
                        : (userRole == "Bệnh nhân" || userRole == "Caregiver") && user.TrangThai == "Hoạt động",
                    CanBookAppointment = isPermissionsManaged 
                        ? claims.Any(c => c.Type == "CanBookAppointment") 
                        : (userRole == "Bệnh nhân" || userRole == "Caregiver") && user.TrangThai == "Hoạt động",
                    CanSendConsultation = isPermissionsManaged 
                        ? claims.Any(c => c.Type == "CanSendConsultation") 
                        : (userRole == "Bệnh nhân" || userRole == "Caregiver") && user.TrangThai == "Hoạt động",
                    CanViewMedicalHistory = isPermissionsManaged 
                        ? claims.Any(c => c.Type == "CanViewMedicalHistory") 
                        : (userRole == "Bệnh nhân" || userRole == "Bác sĩ" || userRole == "Caregiver") && user.TrangThai == "Hoạt động",
                    CanManageAccounts = isPermissionsManaged 
                        ? claims.Any(c => c.Type == "CanManageAccounts") 
                        : userRole == "Admin",
                    
                    SelectedPatientId = currentPatientId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi mở form chỉnh sửa người dùng");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin người dùng.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // Lấy lại dữ liệu cho dropdown trong trường hợp ModelState không hợp lệ
            ViewBag.Roles = await _context.VaiTros
                .Select(r => new SelectListItem
                {
                    Value = r.VaiTroId.ToString(),
                    Text = r.TenVaiTro,
                    Selected = r.VaiTroId == model.RoleId
                }).ToListAsync();

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Hoạt động", Text = "Hoạt động", Selected = model.Status == "Hoạt động" },
                new SelectListItem { Value = "Chờ duyệt", Text = "Chờ duyệt", Selected = model.Status == "Chờ duyệt" },
                new SelectListItem { Value = "Không hoạt động", Text = "Không hoạt động", Selected = model.Status == "Không hoạt động" }
            };

            // Lấy lại danh sách bệnh nhân (sử dụng Identity Role)
            var patientsInRole = await _userManager.GetUsersInRoleAsync("Patient");
            var benhNhansInRole = await _userManager.GetUsersInRoleAsync("Bệnh nhân");
            
            var allPatients = patientsInRole.Concat(benhNhansInRole)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            ViewBag.Patients = allPatients.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.HoTen + " (" + u.Email + ")",
                Selected = u.Id == model.SelectedPatientId
            }).ToList();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.NguoiDungs.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Xử lý tải lên ảnh đại diện nếu có
                    if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(user.AnhDaiDien) && !user.AnhDaiDien.Contains("avatar-default.png"))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.AnhDaiDien.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/avatars");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = $"{Guid.NewGuid().ToString()}_{model.AvatarFile.FileName}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.AvatarFile.CopyToAsync(fileStream);
                        }

                        user.AnhDaiDien = $"/uploads/avatars/{uniqueFileName}";
                    }
                    // Xử lý xóa ảnh đại diện
                    else if (Request.Form["DeleteAvatar"] == "true")
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(user.AnhDaiDien) && !user.AnhDaiDien.Contains("avatar-default.png"))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.AnhDaiDien.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        
                        // Đặt về null hoặc chuỗi rỗng để hệ thống tự động dùng ảnh mặc định theo vai trò
                        user.AnhDaiDien = null;
                    }

                    // Cập nhật thông tin người dùng
                    user.HoTen = model.FullName;

                    // Chỉ cập nhật số điện thoại nếu có giá trị
                    if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                    {
                        user.PhoneNumber = model.PhoneNumber;
                    }

                    // Chỉ cập nhật địa chỉ nếu có giá trị
                    if (!string.IsNullOrWhiteSpace(model.Address))
                    {
                        user.DiaChi = model.Address;
                    }

                    // Cập nhật thông tin cơ bản từ form
                    user.TrangThai = model.Status;
                    user.VaiTroId = model.RoleId;
                    user.NgayCapNhat = DateTime.Now;

                    // Nếu email thay đổi, cập nhật email cho tài khoản Identity
                    if (user.Email != model.Email)
                    {
                        var identityUser = await _userManager.FindByIdAsync(id);
                        if (identityUser != null)
                        {
                            // Kiểm tra xem email mới đã tồn tại chưa
                            var existingUser = await _userManager.FindByEmailAsync(model.Email);
                            if (existingUser != null && existingUser.Id != id)
                            {
                                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                                return View(model);
                            }

                            identityUser.Email = model.Email;
                            identityUser.UserName = model.Email;
                            identityUser.NormalizedEmail = model.Email.ToUpper();
                            identityUser.NormalizedUserName = model.Email.ToUpper();

                            // Chỉ cập nhật số điện thoại nếu có giá trị
                            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                            {
                                identityUser.PhoneNumber = model.PhoneNumber;
                            }

                            var updateResult = await _userManager.UpdateAsync(identityUser);
                            if (!updateResult.Succeeded)
                            {
                                foreach (var error in updateResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                return View(model);
                            }
                        }

                        user.Email = model.Email;
                    }

                    // Đổi mật khẩu nếu có yêu cầu
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var identityUser = await _userManager.FindByIdAsync(id);
                        if (identityUser != null)
                        {
                            // Xóa mật khẩu cũ và đặt mật khẩu mới
                            var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                            var resetPassResult = await _userManager.ResetPasswordAsync(identityUser, token, model.NewPassword);

                            if (!resetPassResult.Succeeded)
                            {
                                foreach (var error in resetPassResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                return View(model);
                            }
                        }
                    }

                    // Đồng bộ vai trò Identity với vai trò đã xác định (user.VaiTroId)
                    var finalRole = await _context.VaiTros.FindAsync(user.VaiTroId);
                    var finalRoleName = finalRole?.TenVaiTro;

                    if (!string.IsNullOrEmpty(finalRoleName))
                    {
                        var identityUser = await _userManager.FindByIdAsync(id);
                        if (identityUser != null)
                        {
                            var currentRoles = await _userManager.GetRolesAsync(identityUser);
                            // Nếu vai trò hiện tại trong Identity khác với vai trò mong muốn
                            if (!currentRoles.Contains(finalRoleName))
                            {
                                await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
                                await _userManager.AddToRoleAsync(identityUser, finalRoleName);
                            }

                            // 2. Cập nhật Quyền hạn (Claims)
                            // Lấy danh sách claims hiện tại
                            var currentClaims = await _userManager.GetClaimsAsync(identityUser);
                            
                            // Danh sách các loại quyền chúng ta quản lý
                            var permissionTypes = new[] { "CanViewProfile", "CanBookAppointment", "CanSendConsultation", "CanViewMedicalHistory", "CanManageAccounts", "PermissionsManaged" };
                            
                            // Xóa các claim cũ thuộc về quyền hạn
                            var claimsToRemove = currentClaims.Where(c => permissionTypes.Contains(c.Type)).ToList();
                            if (claimsToRemove.Any())
                            {
                                await _userManager.RemoveClaimsAsync(identityUser, claimsToRemove);
                            }

                            // Thêm các claim mới dựa trên checkbox
                            var newClaims = new List<Claim>
                            {
                                new Claim("PermissionsManaged", "true") // Đánh dấu là tài khoản này đã được quản lý quyền thủ công
                            };

                            if (model.CanViewProfile) newClaims.Add(new Claim("CanViewProfile", "true"));
                            if (model.CanBookAppointment) newClaims.Add(new Claim("CanBookAppointment", "true"));
                            if (model.CanSendConsultation) newClaims.Add(new Claim("CanSendConsultation", "true"));
                            if (model.CanViewMedicalHistory) newClaims.Add(new Claim("CanViewMedicalHistory", "true"));
                            if (model.CanManageAccounts) newClaims.Add(new Claim("CanManageAccounts", "true"));

                            await _userManager.AddClaimsAsync(identityUser, newClaims);
                        }
                    }

                    // Xử lý gán bệnh nhân cho người chăm sóc
                    // Kiểm tra xem vai trò được chọn có phải là Caregiver không (giả sử ID hoặc Tên)
                    // Chúng ta đã lấy finalRole ở trên
                    if (finalRoleName == "Caregiver" || finalRoleName == "Người chăm sóc") 
                    {
                        if (!string.IsNullOrEmpty(model.SelectedPatientId))
                        {
                            var existingRelation = await _context.NguoiChamSocBenhNhans
                                .FirstOrDefaultAsync(x => x.NguoiChamSocId == id);
                            
                            if (existingRelation != null)
                            {
                                existingRelation.BenhNhanId = model.SelectedPatientId;
                                _context.NguoiChamSocBenhNhans.Update(existingRelation);
                            }
                            else
                            {
                                var newRelation = new NguoiChamSocBenhNhan
                                {
                                    NguoiChamSocId = id,
                                    BenhNhanId = model.SelectedPatientId,
                                    NgayTao = DateTime.Now
                                };
                                _context.NguoiChamSocBenhNhans.Add(newRelation);
                            }
                        }
                    }
                    else
                    {
                        // Nếu không phải Caregiver nữa, xóa quan hệ cũ nếu có
                        var existingRelation = await _context.NguoiChamSocBenhNhans
                            .FirstOrDefaultAsync(x => x.NguoiChamSocId == id);
                        if (existingRelation != null)
                        {
                            _context.NguoiChamSocBenhNhans.Remove(existingRelation);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Cập nhật quyền hạn (lưu vào bảng quyền nếu cần)
                    // Đây là nơi bạn có thể lưu quyền hạn vào database nếu cần

                    // Xóa cache
                    _cache.Remove("UserStats");

                    TempData["SuccessMessage"] = "Cập nhật thông tin người dùng thành công";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await _context.NguoiDungs.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Lỗi cập nhật đồng thời khi chỉnh sửa người dùng");
                        ModelState.AddModelError(string.Empty, "Người dùng đã bị thay đổi bởi người khác. Vui lòng tải lại trang và thử lại.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật thông tin người dùng");
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi lưu thông tin người dùng. Vui lòng thử lại.");
                }
            }

            return View(model);
        }
    }
}