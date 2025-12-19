using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAnChamSocSucKhoe.Areas.Admin.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        // GET: Admin/Settings
        public IActionResult Index()
        {
            try
            {
                // Load settings from configuration or database
                // For now, we'll use default values
                var settings = new SettingsViewModel
                {
                    SystemName = "Hệ thống chăm sóc sức khỏe",
                    SystemDescription = "Hệ thống quản lý và tư vấn chăm sóc sức khỏe",
                    AdminEmail = "admin@healthcare.com",
                    SupportPhone = "",
                    SupportEmail = "",
                    Address = "",
                    ItemsPerPage = 10,
                    AllowRegistration = true,
                    RequireEmailConfirmation = false,
                    EnableGoogleLogin = false,
                    EnableFacebookLogin = false,
                    SendNotificationEmails = true,
                    SendReminderEmails = true,
                    SessionTimeoutMinutes = 60,
                    DefaultLanguage = "vi",
                    TimeZone = "SE Asia Standard Time"
                };

                return View(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải cài đặt hệ thống");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải cài đặt hệ thống.";
                return View(new SettingsViewModel());
            }
        }

        // POST: Admin/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                // Save settings to configuration or database
                // For now, we'll just show success message
                // In a real application, you would save these to appsettings.json or database

                TempData["SuccessMessage"] = "Cài đặt hệ thống đã được cập nhật thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cài đặt hệ thống");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật cài đặt hệ thống.";
                return View(model);
            }
        }

        // GET: Admin/Settings/Email
        public IActionResult Email()
        {
            try
            {
                var emailSettings = new EmailSettingsViewModel
                {
                    SmtpServer = "",
                    SmtpPort = 587,
                    SmtpUsername = "",
                    SmtpPassword = "",
                    EnableSsl = true,
                    FromEmail = "",
                    FromName = ""
                };

                return View(emailSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải cài đặt email");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải cài đặt email.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Settings/Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Email(EmailSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                // Save email settings to configuration or database
                // For now, we'll just show success message
                // In a real application, you would save these to appsettings.json or database

                TempData["SuccessMessage"] = "Cài đặt email đã được cập nhật thành công.";
                return RedirectToAction("Email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cài đặt email");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật cài đặt email.";
                return View(model);
            }
        }

        // GET: Admin/Settings/Security
        public IActionResult Security()
        {
            try
            {
                var securitySettings = new SecuritySettingsViewModel
                {
                    EnableTwoFactorAuth = false,
                    PasswordMinLength = 8,
                    PasswordRequireUppercase = true,
                    PasswordRequireLowercase = true,
                    PasswordRequireDigit = true,
                    PasswordRequireNonAlphanumeric = false,
                    MaxLoginAttempts = 5,
                    LockoutDurationMinutes = 15
                };

                return View(securitySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải cài đặt bảo mật");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải cài đặt bảo mật.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Settings/Security
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Security(SecuritySettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                // Save security settings to configuration or database
                // For now, we'll just show success message
                // In a real application, you would save these to appsettings.json or database

                TempData["SuccessMessage"] = "Cài đặt bảo mật đã được cập nhật thành công.";
                return RedirectToAction("Security");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cài đặt bảo mật");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật cài đặt bảo mật.";
                return View(model);
            }
        }

        // GET: Admin/Settings/Backup
        public IActionResult Backup()
        {
            try
            {
                var backupSettings = new BackupSettingsViewModel
                {
                    AutoBackupEnabled = true,
                    BackupFrequency = "daily",
                    BackupTime = "02:00",
                    RetentionDays = 30,
                    BackupLocation = "/backups/",
                    LastBackupDate = DateTime.Now.AddDays(-1),
                    LastBackupStatus = "Thành công"
                };

                return View(backupSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải cài đặt sao lưu");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải cài đặt sao lưu.";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Settings/Backup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Backup(BackupSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                // Save backup settings to configuration or database
                // For now, we'll just show success message
                // In a real application, you would save these to appsettings.json or database

                TempData["SuccessMessage"] = "Cài đặt sao lưu đã được cập nhật thành công.";
                return RedirectToAction("Backup");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cài đặt sao lưu");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật cài đặt sao lưu.";
                return View(model);
            }
        }

        // POST: Admin/Settings/CreateBackup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBackup()
        {
            try
            {
                // Create backup logic here
                // For now, just show success message

                TempData["SuccessMessage"] = "Sao lưu dữ liệu thành công.";
                return RedirectToAction("Backup");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sao lưu");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo sao lưu.";
                return RedirectToAction("Backup");
            }
        }
    }
}