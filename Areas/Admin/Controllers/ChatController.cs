using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Data;
using DoAnChamSocSucKhoe.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var consultations = await _context.TuVanSucKhoes
                .Include(t => t.NguoiDung)
                .Include(t => t.ChuyenGia)
                .Include(t => t.Messages)
                .AsNoTracking()
                .ToListAsync();

            return View(consultations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var consultation = await _context.TuVanSucKhoes
                .Include(t => t.NguoiDung)
                .Include(t => t.ChuyenGia)
                .Include(t => t.Messages.OrderBy(m => m.SentTime))
                .FirstOrDefaultAsync(t => t.TuVanSucKhoeId == id);

            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConversation(int id)
        {
            var consultation = await _context.TuVanSucKhoes.FindAsync(id);
            if (consultation != null)
            {
                _context.TuVanSucKhoes.Remove(consultation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reports()
        {
            var totalMessages = await _context.Messages.CountAsync();
            var totalConversations = await _context.TuVanSucKhoes.CountAsync();
            var messagesByDate = await _context.Messages
                .GroupBy(m => m.SentTime.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Date)
                .Take(30)
                .ToListAsync();

            ViewBag.TotalMessages = totalMessages;
            ViewBag.TotalConversations = totalConversations;
            ViewBag.MessagesByDate = messagesByDate;

            return View();
        }
    }
}