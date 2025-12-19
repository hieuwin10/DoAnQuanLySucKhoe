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
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            ApplicationDbContext context,
            ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _context.VaiTros
                    .Include(r => r.NguoiDungs)
                    .OrderBy(r => r.TenVaiTro)
                    .Select(r => new RoleViewModel
                    {
                        VaiTroId = r.VaiTroId,
                        TenVaiTro = r.TenVaiTro,
                        MoTa = r.MoTa,
                        UserCount = r.NguoiDungs.Count(),
                        NgayTao = r.NgayTao
                    })
                    .ToListAsync();

                return View(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách vai trò");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách vai trò.";
                return View(new List<RoleViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var role = await _context.VaiTros
                    .Include(r => r.NguoiDungs)
                    .FirstOrDefaultAsync(r => r.VaiTroId == id);

                if (role == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy vai trò này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new RoleDetailViewModel
                {
                    VaiTroId = role.VaiTroId,
                    TenVaiTro = role.TenVaiTro,
                    MoTa = role.MoTa,
                    NgayTao = role.NgayTao,
                    Users = role.NguoiDungs.Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        FullName = u.HoTen,
                        Email = u.Email ?? "",
                        Status = u.TrangThai ?? "Hoạt động",
                        Avatar = !string.IsNullOrEmpty(u.AnhDaiDien) ? u.AnhDaiDien : "/images/avatar-default.png"
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải chi tiết vai trò");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết vai trò.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var role = new VaiTro
                    {
                        TenVaiTro = model.TenVaiTro,
                        MoTa = model.MoTa,
                        NgayTao = DateTime.Now,
                        NguoiDungs = new List<NguoiDung>()
                    };

                    _context.VaiTros.Add(role);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm vai trò mới thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo vai trò mới");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo vai trò mới.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var role = await _context.VaiTros.FindAsync(id);
                if (role == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy vai trò này.";
                    return RedirectToAction("Index");
                }

                var model = new EditRoleViewModel
                {
                    VaiTroId = role.VaiTroId,
                    TenVaiTro = role.TenVaiTro,
                    MoTa = role.MoTa
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải vai trò để chỉnh sửa");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải vai trò.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _context.VaiTros.FindAsync(model.VaiTroId);
                    if (role == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy vai trò này.";
                        return RedirectToAction("Index");
                    }

                    role.TenVaiTro = model.TenVaiTro;
                    role.MoTa = model.MoTa;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật vai trò thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật vai trò");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật vai trò.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = await _context.VaiTros
                    .Include(r => r.NguoiDungs)
                    .FirstOrDefaultAsync(r => r.VaiTroId == id);

                if (role == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy vai trò này.";
                    return RedirectToAction("Index");
                }

                if (role.NguoiDungs.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa vai trò này vì vẫn còn người dùng đang sử dụng.";
                    return RedirectToAction("Index");
                }

                _context.VaiTros.Remove(role);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa vai trò thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa vai trò");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa vai trò.";
            }

            return RedirectToAction("Index");
        }
    }
}