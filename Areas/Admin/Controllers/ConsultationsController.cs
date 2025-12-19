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
    public class ConsultationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConsultationsController> _logger;

        public ConsultationsController(ApplicationDbContext context, ILogger<ConsultationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Consultations
        public async Task<IActionResult> Index(string searchTerm = "", string statusFilter = "all", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.TuVanSucKhoes
                    .Include(t => t.NguoiDung)
                    .Include(t => t.ChuyenGia)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(t => t.TieuDe.Contains(searchTerm) ||
                                            t.NoiDung.Contains(searchTerm) ||
                                            (t.NguoiDung != null && t.NguoiDung.HoTen != null && t.NguoiDung.HoTen.Contains(searchTerm)) ||
                                            (t.ChuyenGia != null && t.ChuyenGia.HoTen != null && t.ChuyenGia.HoTen.Contains(searchTerm)));
                }

                // Apply status filter
                if (statusFilter != "all")
                {
                    query = statusFilter switch
                    {
                        "pending" => query.Where(t => string.IsNullOrEmpty(t.TraLoi)),
                        "answered" => query.Where(t => !string.IsNullOrEmpty(t.TraLoi)),
                        _ => query
                    };
                }

                var totalRecords = await query.CountAsync();
                var consultations = await query
                    .OrderByDescending(t => t.NgayTao)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new ConsultationViewModel
                    {
                        Id = t.TuVanSucKhoeId,
                        TieuDe = t.TieuDe,
                        NoiDung = t.NoiDung.Length > 100 ? t.NoiDung.Substring(0, 100) + "..." : t.NoiDung,
                        TraLoi = t.TraLoi,
                        NgayTao = t.NgayTao,
                        NgayTraLoi = t.NgayTraLoi,
                        PatientName = t.NguoiDung.HoTen,
                        PatientEmail = t.NguoiDung.Email ?? "",
                        DoctorName = t.ChuyenGia != null ? t.ChuyenGia.HoTen : "Chưa phân công",
                        Status = string.IsNullOrEmpty(t.TraLoi) ? "Chờ trả lời" : "Đã trả lời",
                        IsAnswered = !string.IsNullOrEmpty(t.TraLoi)
                    })
                    .ToListAsync();

                var viewModel = new ConsultationListViewModel
                {
                    Consultations = consultations,
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Stats = new ConsultationStats
                    {
                        TotalConsultations = await _context.TuVanSucKhoes.CountAsync(),
                        PendingConsultations = await _context.TuVanSucKhoes.CountAsync(t => string.IsNullOrEmpty(t.TraLoi)),
                        AnsweredConsultations = await _context.TuVanSucKhoes.CountAsync(t => !string.IsNullOrEmpty(t.TraLoi)),
                        TodayConsultations = await _context.TuVanSucKhoes.CountAsync(t => t.NgayTao.Date == DateTime.Today)
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading consultations list");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách tư vấn.";
                return View(new ConsultationListViewModel());
            }
        }

        // GET: Admin/Consultations/Pending
        public async Task<IActionResult> Pending(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.TuVanSucKhoes
                    .Include(t => t.NguoiDung)
                    .Include(t => t.ChuyenGia)
                    .Where(t => string.IsNullOrEmpty(t.TraLoi));

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(t => t.TieuDe.Contains(searchTerm) ||
                                            t.NoiDung.Contains(searchTerm) ||
                                            (t.NguoiDung != null && t.NguoiDung.HoTen.Contains(searchTerm)));
                }

                var totalRecords = await query.CountAsync();
                var consultations = await query
                    .OrderByDescending(t => t.NgayTao)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new ConsultationViewModel
                    {
                        Id = t.TuVanSucKhoeId,
                        TieuDe = t.TieuDe,
                        NoiDung = t.NoiDung.Length > 100 ? t.NoiDung.Substring(0, 100) + "..." : t.NoiDung,
                        TraLoi = t.TraLoi,
                        NgayTao = t.NgayTao,
                        NgayTraLoi = t.NgayTraLoi,
                        PatientName = t.NguoiDung.HoTen,
                        PatientEmail = t.NguoiDung.Email ?? "",
                        DoctorName = t.ChuyenGia != null ? t.ChuyenGia.HoTen : "Chưa phân công",
                        Status = "Chờ trả lời",
                        IsAnswered = false
                    })
                    .ToListAsync();

                var viewModel = new ConsultationListViewModel
                {
                    Consultations = consultations,
                    SearchTerm = searchTerm,
                    StatusFilter = "pending",
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Stats = new ConsultationStats
                    {
                        TotalConsultations = await _context.TuVanSucKhoes.CountAsync(),
                        PendingConsultations = totalRecords,
                        AnsweredConsultations = await _context.TuVanSucKhoes.CountAsync(t => !string.IsNullOrEmpty(t.TraLoi)),
                        TodayConsultations = await _context.TuVanSucKhoes.CountAsync(t => t.NgayTao.Date == DateTime.Today && string.IsNullOrEmpty(t.TraLoi))
                    }
                };

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading pending consultations");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách tư vấn chờ trả lời.";
                return View("Index", new ConsultationListViewModel());
            }
        }

        // GET: Admin/Consultations/Answered
        public async Task<IActionResult> Answered(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.TuVanSucKhoes
                    .Include(t => t.NguoiDung)
                    .Include(t => t.ChuyenGia)
                    .Where(t => !string.IsNullOrEmpty(t.TraLoi));

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(t => t.TieuDe.Contains(searchTerm) ||
                                            t.NoiDung.Contains(searchTerm) ||
                                            (t.NguoiDung != null && t.NguoiDung.HoTen != null && t.NguoiDung.HoTen.Contains(searchTerm)) ||
                                            (t.ChuyenGia != null && t.ChuyenGia.HoTen != null && t.ChuyenGia.HoTen.Contains(searchTerm)));
                }

                var totalRecords = await query.CountAsync();
                var consultations = await query
                    .OrderByDescending(t => t.NgayTraLoi)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new ConsultationViewModel
                    {
                        Id = t.TuVanSucKhoeId,
                        TieuDe = t.TieuDe,
                        NoiDung = t.NoiDung.Length > 100 ? t.NoiDung.Substring(0, 100) + "..." : t.NoiDung,
                        TraLoi = t.TraLoi,
                        NgayTao = t.NgayTao,
                        NgayTraLoi = t.NgayTraLoi,
                        PatientName = t.NguoiDung.HoTen,
                        PatientEmail = t.NguoiDung.Email ?? "",
                        DoctorName = t.ChuyenGia != null ? t.ChuyenGia.HoTen : "Chưa phân công",
                        Status = "Đã trả lời",
                        IsAnswered = true
                    })
                    .ToListAsync();

                var viewModel = new ConsultationListViewModel
                {
                    Consultations = consultations,
                    SearchTerm = searchTerm,
                    StatusFilter = "answered",
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Stats = new ConsultationStats
                    {
                        TotalConsultations = await _context.TuVanSucKhoes.CountAsync(),
                        PendingConsultations = await _context.TuVanSucKhoes.CountAsync(t => string.IsNullOrEmpty(t.TraLoi)),
                        AnsweredConsultations = totalRecords,
                        TodayConsultations = await _context.TuVanSucKhoes.CountAsync(t => t.NgayTraLoi.HasValue && t.NgayTraLoi.Value.Date == DateTime.Today)
                    }
                };

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading answered consultations");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách tư vấn đã trả lời.";
                return View("Index", new ConsultationListViewModel());
            }
        }

        // GET: Admin/Consultations/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var consultation = await _context.TuVanSucKhoes
                    .Include(t => t.NguoiDung)
                    .Include(t => t.ChuyenGia)
                    .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

                if (consultation == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tư vấn này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new ConsultationDetailViewModel
                {
                    Id = consultation.TuVanSucKhoeId,
                    TieuDe = consultation.TieuDe,
                    NoiDung = consultation.NoiDung,
                    TraLoi = consultation.TraLoi,
                    NgayTao = consultation.NgayTao,
                    NgayTraLoi = consultation.NgayTraLoi,
                    PatientName = consultation.NguoiDung.HoTen,
                    PatientEmail = consultation.NguoiDung.Email ?? "",
                    PatientPhone = consultation.NguoiDung.PhoneNumber ?? "",
                    DoctorName = consultation.ChuyenGia != null ? consultation.ChuyenGia.HoTen : "Chưa phân công",
                    Status = string.IsNullOrEmpty(consultation.TraLoi) ? "Chờ trả lời" : "Đã trả lời",
                    IsAnswered = !string.IsNullOrEmpty(consultation.TraLoi)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading consultation details for ID: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết tư vấn.";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Consultations/Reply/5
        public async Task<IActionResult> Reply(int id)
        {
            try
            {
                var consultation = await _context.TuVanSucKhoes
                    .Include(t => t.NguoiDung)
                    .Include(t => t.ChuyenGia)
                    .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

                if (consultation == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tư vấn này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new ReplyConsultationViewModel
                {
                    Id = consultation.TuVanSucKhoeId,
                    TieuDe = consultation.TieuDe,
                    NoiDung = consultation.NoiDung,
                    CurrentReply = consultation.TraLoi,
                    PatientName = consultation.NguoiDung.HoTen,
                    PatientEmail = consultation.NguoiDung.Email ?? ""
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading consultation for reply: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải tư vấn để trả lời.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Consultations/Reply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, ReplyConsultationViewModel model)
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
                    var consultation = await _context.TuVanSucKhoes
                        .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

                    if (consultation == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy tư vấn này.";
                        return RedirectToAction("Index");
                    }

                    consultation.TraLoi = model.Reply;
                    consultation.NgayTraLoi = DateTime.Now;

                    // Optional: Assign current admin as the doctor who replied
                    // consultation.ChuyenGiaId = GetCurrentDoctorId();

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Trả lời tư vấn thành công.";
                    return RedirectToAction("Details", new { id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error replying to consultation: {Id}", id);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi trả lời tư vấn.");
                }
            }

            // Reload the model data if validation failed
            var consultationForView = await _context.TuVanSucKhoes
                .Include(t => t.NguoiDung)
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

            if (consultationForView != null)
            {
                model.TieuDe = consultationForView.TieuDe;
                model.NoiDung = consultationForView.NoiDung;
                model.CurrentReply = consultationForView.TraLoi;
                model.PatientName = consultationForView.NguoiDung?.HoTen ?? "N/A";
                model.PatientEmail = consultationForView.NguoiDung?.Email ?? "N/A";
            }

            return View(model);
        }

        // POST: Admin/Consultations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var consultation = await _context.TuVanSucKhoes
                    .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

                if (consultation == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tư vấn này.";
                    return RedirectToAction("Index");
                }

                _context.TuVanSucKhoes.Remove(consultation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa tư vấn thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting consultation: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa tư vấn.";
                return RedirectToAction("Index");
            }
        }
    }
}