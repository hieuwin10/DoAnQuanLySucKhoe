using Microsoft.AspNetCore.Identity;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Middleware
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (context.User.Identity.IsAuthenticated)
            {
                // Lấy thông tin người dùng
                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    // Lấy vai trò của người dùng
                    var roles = await userManager.GetRolesAsync(user);
                    
                    // Kiểm tra đường dẫn hiện tại
                    var path = context.Request.Path.Value?.ToLower();
                    
                    // Nếu đang ở trang chủ, chuyển hướng theo vai trò
                    if (path == "/" || path == "/home" || path == "/home/index")
                    {
                        if (roles.Contains("Admin"))
                        {
                            context.Response.Redirect("/Admin/Dashboard");
                            return;
                        }
                        else if (roles.Contains("Doctor"))
                        {
                            context.Response.Redirect("/Doctor/Dashboard");
                            return;
                        }
                        else if (roles.Contains("Patient"))
                        {
                            context.Response.Redirect("/Patient/Dashboard");
                            return;
                        }
                    }
                    
                    // Kiểm tra quyền truy cập vào các area
                    if (path.StartsWith("/admin") && !roles.Contains("Admin"))
                    {
                        context.Response.Redirect("/Identity/Account/AccessDenied");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}