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
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            ApplicationDbContext context,
            ILogger<FeedbackController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Feedback
        public async Task<IActionResult> Index(string searchTerm = "", string statusFilter = "all", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.PhanHoiSucKhoes
                    .Include(f => f.NguoiDung)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(f => f.NoiDung.Contains(searchTerm) ||
                                            (f.NguoiDung != null && f.NguoiDung.HoTen != null && f.NguoiDung.HoTen.Contains(searchTerm)) ||
                                            (f.NguoiDung != null && f.NguoiDung.Email != null && f.NguoiDung.Email.Contains(searchTerm)));
                }

                // Apply status filter
                if (statusFilter != "all")
                {
                    bool statusValue = statusFilter == "processed";
                    query = query.Where(f => f.TrangThai == statusValue);
                }

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var feedbacks = await query
                    .OrderByDescending(f => f.NgayTao)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => new FeedbackViewModel
                    {
                        Id = f.PhanHoiSucKhoeId,
                        SenderName = f.NguoiDung != null ? f.NguoiDung.HoTen ?? "N/A" : "N/A",
                        SenderEmail = f.NguoiDung != null ? f.NguoiDung.Email ?? "N/A" : "N/A",
                        Content = f.NoiDung,
                        CreatedAt = f.NgayTao,
                        Status = f.TrangThai ? "Đã xử lý" : "Chưa xử lý",
                        StatusClass = f.TrangThai ? "success" : "warning"
                    })
                    .ToListAsync();

                var viewModel = new FeedbackListViewModel
                {
                    Feedbacks = feedbacks,
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách phản hồi");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách phản hồi.";
                return View(new FeedbackListViewModel
                {
                    Feedbacks = new List<FeedbackViewModel>(),
                    SearchTerm = "",
                    StatusFilter = "all",
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                });
            }
        }

        // GET: Admin/Feedback/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var feedback = await _context.PhanHoiSucKhoes
                    .Include(f => f.NguoiDung)
                    .FirstOrDefaultAsync(f => f.PhanHoiSucKhoeId == id);

                if (feedback == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phản hồi này.";
                    return RedirectToAction("Index");
                }

                var viewModel = new FeedbackViewModel
                {
                    Id = feedback.PhanHoiSucKhoeId,
                    SenderName = feedback.NguoiDung?.HoTen ?? "N/A",
                    SenderEmail = feedback.NguoiDung?.Email ?? "N/A",
                    Content = feedback.NoiDung,
                    CreatedAt = feedback.NgayTao,
                    Status = feedback.TrangThai ? "Đã xử lý" : "Chưa xử lý",
                    StatusClass = feedback.TrangThai ? "success" : "warning"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải chi tiết phản hồi: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết phản hồi.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Feedback/MarkAsProcessed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsProcessed(int id)
        {
            try
            {
                var feedback = await _context.PhanHoiSucKhoes.FindAsync(id);
                if (feedback == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phản hồi này.";
                    return RedirectToAction("Index");
                }

                feedback.TrangThai = true;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã đánh dấu phản hồi là đã xử lý.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái phản hồi: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái phản hồi.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Feedback/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var feedback = await _context.PhanHoiSucKhoes.FindAsync(id);
                if (feedback == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phản hồi này.";
                    return RedirectToAction("Index");
                }

                _context.PhanHoiSucKhoes.Remove(feedback);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã xóa phản hồi thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa phản hồi: {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa phản hồi.";
                return RedirectToAction("Index");
            }
        }
    }
}