using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorsController> _logger;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private static string GetDoctorStatus(NguoiDung user)
        {
            if (user.EmailConfirmed && !user.LockoutEnabled)
                return "Hoạt động";
            else if (!user.EmailConfirmed)
                return "Chờ duyệt";
            else
                return "Không hoạt động";
        }

        public DoctorsController(ApplicationDbContext context, ILogger<DoctorsController> logger, UserManager<NguoiDung> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Doctors
        public async Task<IActionResult> Index(string searchTerm = "", string statusFilter = "all", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .Include(u => u.HoSoSucKhoe)
                    .Where(u => u.VaiTroId == 2); // Doctor role

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => (u.HoTen != null && u.HoTen.Contains(searchTerm)) ||
                                            (u.Email != null && u.Email.Contains(searchTerm)) ||
                                            (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
                }

                // Apply status filter
                if (statusFilter != "all")
                {
                    query = statusFilter switch
                    {
                        "active" => query.Where(u => u.EmailConfirmed && !u.LockoutEnabled),
                        "inactive" => query.Where(u => !u.EmailConfirmed || u.LockoutEnabled),
                        _ => query
                    };
                }

                var totalRecords = await query.CountAsync();
                var doctors = await query
                    .OrderByDescending(u => u.NgayTao)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new DoctorViewModel
                    {
                        Id = u.Id,
                        FullName = u.HoTen ?? "N/A",
                        Email = u.Email ?? "N/A",
                        PhoneNumber = u.PhoneNumber ?? "N/A",
                        Specialty = "Chưa cập nhật", // NguoiDung không có ChuyenMon
                        Status = GetDoctorStatus(u),
                        StatusClass = DoctorDetailViewModel.GetStatusClass(GetDoctorStatus(u)),
                        Avatar = string.IsNullOrEmpty(u.AnhDaiDien) ? "/images/default-avatar.png" : u.AnhDaiDien
                    })
                    .ToListAsync();

                var viewModel = new DoctorListViewModel
                {
                    Doctors = doctors,
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Stats = new DoctorStats
                    {
                        TotalDoctors = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 2),
                        ActiveDoctors = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 2 && u.EmailConfirmed && !u.LockoutEnabled),
                        InactiveDoctors = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 2 && (!u.EmailConfirmed || u.LockoutEnabled)),
                        NewDoctorsThisMonth = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 2 && u.NgayTao.Month == DateTime.Now.Month && u.NgayTao.Year == DateTime.Now.Year)
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctors list");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách bác sĩ.";
                return View(new DoctorListViewModel());
            }
        }

        // GET: Admin/Doctors/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID bác sĩ không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                var doctor = await _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .Include(u => u.HoSoSucKhoe)
                    .Include(u => u.LichHens)
                    .FirstOrDefaultAsync(u => u.Id == id && u.VaiTroId == 2);

                if (doctor == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bác sĩ này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new DoctorDetailViewModel
                {
                    Id = doctor.Id,
                    FullName = doctor.HoTen ?? "N/A",
                    Email = doctor.Email ?? "N/A",
                    PhoneNumber = doctor.PhoneNumber,
                    Specialization = "Chưa cập nhật", // NguoiDung doesn't have ChuyenMon
                    ExperienceYears = 0, // NguoiDung không có NamKinhNghiem
                    Bio = "Chưa cập nhật", // NguoiDung không có Bio
                    Education = "Chưa cập nhật", // NguoiDung không có HocVan
                    Certifications = "Chưa cập nhật", // NguoiDung không có ChungChi
                    Status = GetDoctorStatus(doctor),
                    Avatar = string.IsNullOrEmpty(doctor.AnhDaiDien) ? "/images/default-avatar.png" : doctor.AnhDaiDien,
                    CreatedDate = doctor.NgayTao,
                    LastActivity = null, // NguoiDung không có LastLoginDate
                    TotalAppointments = doctor.LichHens?.Count ?? 0,
                    CompletedAppointments = doctor.LichHens?.Count(l => l.TrangThai == "Hoàn thành") ?? 0,
                    AverageRating = 0, // NguoiDung doesn't have DanhGiaChuyenGias
                    TotalReviews = 0 // NguoiDung doesn't have DanhGiaChuyenGias
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctor details for ID: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin bác sĩ.";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Doctors/Create
        public IActionResult Create()
        {
            return View(new CreateDoctorViewModel());
        }

        // POST: Admin/Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var doctor = new NguoiDung
                    {
                        HoTen = model.FullName,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = true,
                        VaiTroId = 2
                    };

                    var result = await _userManager.CreateAsync(doctor, model.Password);
                    if (result.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync("Doctor"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                        }
                        await _userManager.AddToRoleAsync(doctor, "Doctor");

                        // Create ChuyenGia profile
                        var chuyenGia = new ChuyenGia
                        {
                            ChuyenGiaId = Guid.NewGuid().ToString(),
                            NguoiDungId = doctor.Id,
                            NguoiDung = doctor,
                            ChuyenKhoa = model.Specialization ?? "Bác sĩ đa khoa",
                            KinhNghiem = model.ExperienceYears + " năm",
                            MoTa = model.Bio ?? "Chưa có thông tin",
                            ChungChi = model.Certifications ?? "Chưa cập nhật",
                            NoiCongTac = "Bệnh viện đa khoa", // Default
                            NgayTao = DateTime.Now,
                            NgayCapNhat = DateTime.Now,
                            TrangThai = true,
                            DanhGiaDaNhan = new List<DanhGiaChuyenGia>(),
                            TuVanSucKhoes = new List<TuVanSucKhoe>()
                        };
                        
                        _context.ChuyenGias.Add(chuyenGia);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Thêm bác sĩ mới thành công.";
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating new doctor");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm bác sĩ mới.");
                }
            }

            return View(model);
        }

        // GET: Admin/Doctors/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID bác sĩ không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                var doctor = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Id == id && u.VaiTroId == 2);

                if (doctor == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bác sĩ này.";
                    return RedirectToAction("Index");
                }

                // Load profile info
                var profile = await _context.ChuyenGias.FirstOrDefaultAsync(c => c.NguoiDungId == id);

                var viewModel = new EditDoctorViewModel
                {
                    Id = doctor.Id,
                    FullName = doctor.HoTen ?? string.Empty,
                    Email = doctor.Email ?? string.Empty,
                    PhoneNumber = doctor.PhoneNumber ?? string.Empty,
                    Specialization = profile?.ChuyenKhoa ?? "",
                    ExperienceYears = profile != null && int.TryParse(profile.KinhNghiem?.Replace(" năm", ""), out int years) ? years : 0,
                    Bio = profile?.MoTa ?? "",
                    Education = "", // Not in ChuyenGia model currently? Or mapped to something else?
                    Certifications = profile?.ChungChi ?? ""
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctor for edit: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin bác sĩ.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditDoctorViewModel model)
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
                    var doctor = await _context.NguoiDungs
                        .FirstOrDefaultAsync(u => u.Id == id && u.VaiTroId == 2);

                    if (doctor == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy bác sĩ này.";
                        return RedirectToAction("Index");
                    }

                    doctor.HoTen = model.FullName;
                    doctor.Email = model.Email;
                    doctor.UserName = model.Email; // Update username if email changed
                    doctor.PhoneNumber = model.PhoneNumber;
                    
                    // Update or Create ChuyenGia profile
                    var profile = await _context.ChuyenGias.FirstOrDefaultAsync(c => c.NguoiDungId == id);
                    if (profile == null)
                    {
                        profile = new ChuyenGia
                        {
                            ChuyenGiaId = Guid.NewGuid().ToString(),
                            NguoiDungId = doctor.Id,
                            NguoiDung = doctor,
                            NgayTao = DateTime.Now,
                            TrangThai = true,
                            DanhGiaDaNhan = new List<DanhGiaChuyenGia>(),
                            TuVanSucKhoes = new List<TuVanSucKhoe>(),
                            NoiCongTac = "Bệnh viện đa khoa", // Default
                            ChuyenKhoa = model.Specialization ?? "Khác",
                            KinhNghiem = model.ExperienceYears + " năm",
                            MoTa = model.Bio ?? "",
                            ChungChi = model.Certifications ?? ""
                        };
                        _context.ChuyenGias.Add(profile);
                    }
                    else
                    {
                        profile.ChuyenKhoa = model.Specialization ?? "Khác";
                        profile.KinhNghiem = model.ExperienceYears + " năm";
                        profile.MoTa = model.Bio ?? "";
                        profile.ChungChi = model.Certifications ?? "";
                        profile.NgayCapNhat = DateTime.Now;
                        _context.ChuyenGias.Update(profile);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật thông tin bác sĩ thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating doctor: {Id}", id);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin bác sĩ.");
                }
            }

            return View(model);
        }

        // GET: Admin/Doctors/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID bác sĩ không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                var doctor = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Id == id && u.VaiTroId == 2);

                if (doctor == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bác sĩ này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new DoctorViewModel
                {
                    Id = doctor.Id,
                    FullName = doctor.HoTen ?? "N/A",
                    Email = doctor.Email ?? "N/A",
                    PhoneNumber = doctor.PhoneNumber ?? "N/A",
                    Specialty = "Chưa cập nhật",
                    Status = GetDoctorStatus(doctor),
                    StatusClass = DoctorDetailViewModel.GetStatusClass(GetDoctorStatus(doctor)),
                    Avatar = string.IsNullOrEmpty(doctor.AnhDaiDien) ? "/images/default-avatar.png" : doctor.AnhDaiDien
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctor for delete: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin bác sĩ.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID bác sĩ không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                var doctor = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Id == id && u.VaiTroId == 2);

                if (doctor == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bác sĩ này.";
                    return RedirectToAction("Index");
                }

                _context.NguoiDungs.Remove(doctor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa bác sĩ thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa bác sĩ.";
                return RedirectToAction("Index");
            }
        }
    }
}