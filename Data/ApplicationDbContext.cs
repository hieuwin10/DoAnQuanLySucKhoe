using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChiTietKeHoachDinhDuong> ChiTietKeHoachDinhDuongs { get; set; }
        public DbSet<ChiTietKeHoachTapLuyen> ChiTietKeHoachTapLuyens { get; set; }
        public DbSet<DanhGiaChuyenGia> DanhGiaChuyenGias { get; set; }
        public DbSet<HoSoSucKhoe> HoSoSucKhoes { get; set; }
        public DbSet<KeHoachDinhDuong> KeHoachDinhDuongs { get; set; }
        public DbSet<KeHoachTapLuyen> KeHoachTapLuyens { get; set; }
        public DbSet<LichHen> LichHens { get; set; } = null!;
        public DbSet<LichSuSucKhoe> LichSuSucKhoes { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<PhanHoiSucKhoe> PhanHoiSucKhoes { get; set; }
        public DbSet<ThongBaoBacSi> ThongBaoBacSis { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<ChiSoSucKhoe> ChiSoSucKhoe { get; set; }
        public DbSet<NhacNhoSucKhoe> NhacNhoSucKhoe { get; set; }
        public DbSet<TuVanSucKhoe> TuVanSucKhoes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Identity tables
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            builder.Entity<ChiTietKeHoachDinhDuong>()
                .Property(e => e.SoLuong)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<HoSoSucKhoe>()
                .Property(e => e.CanNang)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<HoSoSucKhoe>()
                .Property(e => e.ChieuCao)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<HoSoSucKhoe>()
                .Property(e => e.DuongHuyet)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<HoSoSucKhoe>()
                .Property(e => e.HuyetApTamThu)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<HoSoSucKhoe>()
                .Property(e => e.HuyetApTamTruong)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LichSuSucKhoe>()
                .Property(e => e.CanNang)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LichSuSucKhoe>()
                .Property(e => e.ChieuCao)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LichSuSucKhoe>()
                .Property(e => e.DuongHuyet)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LichSuSucKhoe>()
                .Property(e => e.HuyetApTamThu)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LichSuSucKhoe>()
                .Property(e => e.HuyetApTamTruong)
                .HasColumnType("decimal(18, 2)");
    
            builder.Entity<DanhGiaChuyenGia>()
                .HasOne(d => d.NguoiDung)
                .WithMany(n => n.DanhGiaDaGui)
                .HasForeignKey(d => d.NguoiDungId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<DanhGiaChuyenGia>()
                .HasOne(d => d.ChuyenGia)
                .WithMany(n => n.DanhGiaDaNhan)
                .HasForeignKey(d => d.ChuyenGiaId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChuyenGia>()
                .HasOne(c => c.NguoiDung)
                .WithMany(n => n.ChuyenGias)
                .HasForeignKey(c => c.NguoiDungId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<LichHen>()
                .HasOne(l => l.NguoiDung)
                .WithMany()
                .HasForeignKey(l => l.NguoiDungId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ThongBaoBacSi>()
                .HasOne(t => t.NguoiDung)
                .WithMany()
                .HasForeignKey(t => t.NguoiDungId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ThongBaoBacSi>()
                .HasOne(t => t.BacSi)
                .WithMany()
                .HasForeignKey(t => t.BacSiId)
                .OnDelete(DeleteBehavior.NoAction);

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole 
                { 
                    Id = "1", 
                    Name = "Admin", 
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole 
                { 
                    Id = "2", 
                    Name = "Doctor", 
                    NormalizedName = "DOCTOR",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole 
                { 
                    Id = "3", 
                    Name = "Patient", 
                    NormalizedName = "PATIENT",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }
    }

}
