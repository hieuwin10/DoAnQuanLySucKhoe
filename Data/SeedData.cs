using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Kiểm tra xem đã có dữ liệu trong bảng vai_tro chưa
                if (context.VaiTros.Any())
                {
                    return; // Đã có dữ liệu, không cần seed
                }

                // Tạo các vai trò
                var vaiTros = new VaiTro[]
                {
                    new VaiTro { TenVaiTro = "Admin", MoTa = "Admin", NguoiDungs = new List<NguoiDung>() },
                    new VaiTro { TenVaiTro = "Doctor", MoTa = "Doctor", NguoiDungs = new List<NguoiDung>() },
                    new VaiTro { TenVaiTro = "Patient", MoTa = "Patient", NguoiDungs = new List<NguoiDung>() }
                };

                foreach (var vaiTro in vaiTros)
                {
                    context.VaiTros.Add(vaiTro);
                }
                await context.SaveChangesAsync();

                // Tạo các vai trò trong Identity
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("Doctor"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Doctor"));
                }
                if (!await roleManager.RoleExistsAsync("Patient"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Patient"));
                }

                // Tạo tài khoản admin mặc định
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var adminUser = await userManager.FindByEmailAsync("admin@healthmanager.com");
                
                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = "admin@healthmanager.com",
                        Email = "admin@healthmanager.com",
                        EmailConfirmed = true,
                        HoTen = "Admin",
                        VaiTroId = context.VaiTros.FirstOrDefault(v => v.TenVaiTro == "Admin")?.VaiTroId ?? 0
                    };
                    
                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
        }
    }
}