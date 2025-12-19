using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Areas.Doctor.Models;
using DoAnChamSocSucKhoe.Areas.Doctor.Repositories;
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class DashboardController : Controller
    {
        private readonly IDoctorDashboardRepository _dashboardRepository;
        private readonly UserManager<NguoiDung> _userManager;

        public DashboardController(
            IDoctorDashboardRepository dashboardRepository,
            UserManager<NguoiDung> userManager)
        {
            _dashboardRepository = dashboardRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var dashboardData = await _dashboardRepository.GetDashboardDataAsync(doctorId);
            return View(dashboardData);
        }

        [HttpPost]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            // Implementation for marking notification as read
            // This would typically update the notification status in the database
            await Task.CompletedTask;
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, string status)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            // Implementation for updating appointment status
            // This would typically update the appointment status in the database
            await Task.CompletedTask;
            return Json(new { success = true });
        }
    }
} 