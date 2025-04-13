using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Data;       // Thay YourProjectName bằng namespace dự án của bạn
using YourProjectName.Models;     // Thay YourProjectName bằng namespace dự án của bạn

namespace YourProjectName.Areas.Patient.Pages // Thay YourProjectName bằng namespace dự án của bạn
{
    [Authorize] // Chỉ người dùng đã đăng nhập mới truy cập được
    public class HealthProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Thay ApplicationUser bằng lớp User của bạn

        public HealthProfileModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Thuộc tính để binding dữ liệu từ form POST
        [BindProperty]
        public HealthMetric Input { get; set; } = new HealthMetric(); // Khởi tạo để tránh null reference

        // Danh sách lịch sử để hiển thị trên view
        public IList<HealthMetric> History { get; set; } = new List<HealthMetric>();

        // Hàm xử lý khi GET request
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            // Lấy lịch sử health metric của user hiện tại, sắp xếp theo ngày mới nhất lên đầu
            History = await _context.HealthMetrics
                                    .Where(hm => hm.PatientId == user.Id)
                                    .OrderByDescending(hm => hm.RecordedDate)
                                    .ToListAsync();

            // Đặt giá trị mặc định cho ngày ghi nhận là hôm nay khi form được tải lần đầu
            Input.RecordedDate = DateTime.Today;

            return Page();
        }

        // Hàm xử lý khi POST request (khi người dùng nhấn nút Lưu)
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            // Gán PatientId cho record mới trước khi kiểm tra ModelState
            Input.PatientId = user.Id;

            // Kiểm tra xem dữ liệu form gửi lên có hợp lệ không (dựa trên Data Annotations trong Model)
            // Lưu ý: PatientId đã được gán nên không bị lỗi validation thiếu PatientId
            if (!ModelState.IsValid)
            {
                // Nếu không hợp lệ, cần tải lại lịch sử và hiển thị lại trang với lỗi
                await LoadHistoryAsync(user.Id); // Tách hàm tải lịch sử để tái sử dụng
                return Page();
            }


            // Thêm record mới vào DbContext
            _context.HealthMetrics.Add(Input);
            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Đặt thông báo thành công (ví dụ sử dụng TempData)
            TempData["StatusMessage"] = "Đã lưu chỉ số sức khỏe thành công.";

            // Chuyển hướng về lại trang GET để hiển thị dữ liệu mới nhất và tránh double-post
            return RedirectToPage();
        }

        // Hàm hỗ trợ tải lại lịch sử (dùng khi POST không hợp lệ)
        private async Task LoadHistoryAsync(string userId)
        {
             History = await _context.HealthMetrics
                                    .Where(hm => hm.PatientId == userId)
                                    .OrderByDescending(hm => hm.RecordedDate)
                                    .ToListAsync();
        }
    }
}