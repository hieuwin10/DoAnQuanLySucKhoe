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
                // Chỉ seed dữ liệu nếu bảng VaiTros trống
                if (context.VaiTros.Any())
                {
                    return; // Đã có dữ liệu, không cần seed
                }

                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<NguoiDung>>();

                // Tạo các vai trò trong bảng VaiTros
                var vaiTros = new VaiTro[]
                {
                    new VaiTro { TenVaiTro = "Admin", MoTa = "Admin", NguoiDungs = new List<NguoiDung>() },
                    new VaiTro { TenVaiTro = "Doctor", MoTa = "Doctor", NguoiDungs = new List<NguoiDung>() },
                    new VaiTro { TenVaiTro = "Patient", MoTa = "Patient", NguoiDungs = new List<NguoiDung>() },
                    new VaiTro { TenVaiTro = "Caregiver", MoTa = "Caregiver", NguoiDungs = new List<NguoiDung>() }
                };
                foreach (var vaiTro in vaiTros)
                {
                    context.VaiTros.Add(vaiTro);
                }
                await context.SaveChangesAsync();

                // Tạo các vai trò trong Identity
                string[] roleNames = { "Admin", "Doctor", "Patient", "Caregiver" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Tạo tài khoản admin mặc định
                if (await userManager.FindByEmailAsync("admin@healthmanager.com") == null)
                {
                    var vaiTroAdmin = await context.VaiTros.SingleAsync(v => v.TenVaiTro == "Admin");
                    var admin = new NguoiDung
                    {
                        UserName = "admin@healthmanager.com",
                        Email = "admin@healthmanager.com",
                        EmailConfirmed = true,
                        HoTen = "Admin",
                        VaiTroId = vaiTroAdmin.VaiTroId,
                        TrangThai = "Hoạt động",
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }

                // Create a patient user for testing
                /*
                if (await userManager.FindByEmailAsync("patient@example.com") == null)
                {
                    var vaiTroPatient = await context.VaiTros.SingleAsync(v => v.TenVaiTro == "Patient");
                    var patient = new NguoiDung
                    {
                        UserName = "patient@example.com",
                        Email = "patient@example.com",
                        EmailConfirmed = true,
                        HoTen = "Benh Nhan Mau",
                        VaiTroId = vaiTroPatient.VaiTroId,
                        TrangThai = "Hoạt động",
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(patient, "Patient@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(patient, "Patient");

                        // Also create a health profile for this patient
                        if (!context.HoSoSucKhoes.Any(h => h.NguoiDungId == patient.Id))
                        {
                            var hoSo = new HoSoSucKhoe
                            {
                                NguoiDungId = patient.Id,
                                NgayCapNhat = DateTime.Now,
                                ChieuCao = 170,
                                CanNang = 70,
                                HuyetApTamThu = 120,
                                HuyetApTamTruong = 80,
                                NhipTim = 75,
                                DuongHuyet = 5.5M,
                                TrangThai = "Tốt",
                                ChanDoan = "Không có",
                                PhuongPhapDieuTri = "Không có",
                                TienSuBenh = "Không có",
                                DiUng = "Không có",
                                ThuocDangDung = "Không có",
                                GhiChu = "Hồ sơ mẫu."
                            };
                            context.HoSoSucKhoes.Add(hoSo);
                        }
                    }
                }
                */

                // Create a caregiver user for testing
                /*
                if (await userManager.FindByEmailAsync("caregiver@example.com") == null)
                {
                    var vaiTroCaregiver = await context.VaiTros.SingleAsync(v => v.TenVaiTro == "Caregiver");
                    var caregiver = new NguoiDung
                    {
                        UserName = "caregiver@example.com",
                        Email = "caregiver@example.com",
                        EmailConfirmed = true,
                        HoTen = "Nguoi Cham Soc Mau",
                        VaiTroId = vaiTroCaregiver.VaiTroId,
                        TrangThai = "Hoạt động",
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(caregiver, "Caregiver@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(caregiver, "Caregiver");
                    }
                }

                // Create a doctor user for testing
                if (await userManager.FindByEmailAsync("doctor@example.com") == null)
                {
                    var vaiTroDoctor = await context.VaiTros.SingleAsync(v => v.TenVaiTro == "Doctor");
                    var doctor = new NguoiDung
                    {
                        UserName = "doctor@example.com",
                        Email = "doctor@example.com",
                        EmailConfirmed = true,
                        HoTen = "Bs. Trinh An",
                        VaiTroId = vaiTroDoctor.VaiTroId,
                        TrangThai = "Hoạt động",
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(doctor, "Doctor@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(doctor, "Doctor");

                        // Create ChuyenGia profile
                        if (!context.ChuyenGias.Any(c => c.NguoiDungId == doctor.Id))
                        {
                            var chuyenGia = new ChuyenGia
                            {
                                ChuyenGiaId = doctor.Id, // Using User ID as ChuyenGia ID for simplicity
                                NguoiDungId = doctor.Id,
                                NguoiDung = doctor, // Initialize required navigation property
                                HoTen = doctor.HoTen,
                                ChuyenKhoa = "Da liễu",
                                ChungChi = "Chứng chỉ hành nghề Da liễu",
                                KinhNghiem = "10 năm",
                                NoiCongTac = "Bệnh viện Da liễu TP.HCM",
                                MoTa = "Chuyên gia hàng đầu về da liễu.",
                                NgayTao = DateTime.Now,
                                NgayCapNhat = DateTime.Now,
                                TrangThai = true,
                                HinhAnh = "doctor-1.jpg",
                                DanhGiaDaNhan = new List<DanhGiaChuyenGia>(), // Initialize required collection
                                TuVanSucKhoes = new List<TuVanSucKhoe>() // Initialize required collection
                            };
                            context.ChuyenGias.Add(chuyenGia);
                        }
                    }
                }

                // Save changes to get user IDs before creating the relationship
                await context.SaveChangesAsync();

                // Assign the patient to the caregiver
                var patientUser = await userManager.FindByEmailAsync("patient@example.com");
                var caregiverUser = await userManager.FindByEmailAsync("caregiver@example.com");

                if (patientUser != null && caregiverUser != null)
                {
                    if (!context.NguoiChamSocBenhNhans.Any(r => r.NguoiChamSocId == caregiverUser.Id && r.BenhNhanId == patientUser.Id))
                    {
                        var relationship = new NguoiChamSocBenhNhan
                        {
                            NguoiChamSocId = caregiverUser.Id,
                            BenhNhanId = patientUser.Id
                        };
                        context.NguoiChamSocBenhNhans.Add(relationship);
                        await context.SaveChangesAsync();
                    }
                }
                */
            }
        }
    }
}
