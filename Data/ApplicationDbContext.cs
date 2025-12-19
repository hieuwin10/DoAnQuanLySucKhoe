using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Models;
using DoAnChamSocSucKhoe.Areas.Patient.Models;

namespace DoAnChamSocSucKhoe.Data
{
    public class ApplicationDbContext : IdentityDbContext<NguoiDung>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; } = default!;
        public DbSet<DanhGiaChuyenGia> DanhGiaChuyenGias { get; set; } = default!;
        public DbSet<HoSoSucKhoe> HoSoSucKhoes { get; set; } = default!;
        public DbSet<LichSuSucKhoe> LichSuSucKhoes { get; set; } = default!;
        public DbSet<VaiTro> VaiTros { get; set; } = default!;
        public DbSet<LichHen> LichHens { get; set; } = default!;
        public DbSet<TuVanSucKhoe> TuVanSucKhoes { get; set; } = default!;
        public DbSet<ChuyenGia> ChuyenGias { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;
        public DbSet<PhanHoiSucKhoe> PhanHoiSucKhoes { get; set; } = default!;
        public DbSet<KeHoachDinhDuong> KeHoachDinhDuongs { get; set; } = default!;
        public DbSet<ChiTietKeHoachDinhDuong> ChiTietKeHoachDinhDuongs { get; set; } = default!;
        public DbSet<KeHoachTapLuyen> KeHoachTapLuyens { get; set; } = default!;
        public DbSet<ChiTietKeHoachTapLuyen> ChiTietKeHoachTapLuyens { get; set; } = default!;
        public DbSet<ThongBaoBacSi> ThongBaoBacSis { get; set; } = default!;
        public DbSet<ChiSoSucKhoe> ChiSoSucKhoes { get; set; } = default!;
        public DbSet<NhacNhoSucKhoe> NhacNhoSucKhoes { get; set; } = default!;
        public DbSet<LichSuHoSoSucKhoe> LichSuHoSoSucKhoes { get; set; } = default!;
        public DbSet<FileHoSo> FileHoSos { get; set; } = default!;
        public DbSet<NguoiChamSocBenhNhan> NguoiChamSocBenhNhans { get; set; } = default!;
        public DbSet<global::DoAnChamSocSucKhoe.Models.Message> Messages { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ một-nhiều giữa NguoiDung và DanhGiaChuyenGia
            modelBuilder.Entity<DanhGiaChuyenGia>()
                .HasOne(dg => dg.NguoiDung)
                .WithMany(nd => nd.DanhGiaDaGui)
                .HasForeignKey(dg => dg.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DanhGiaChuyenGia>()
                .HasOne(dg => dg.TuVanSucKhoe)
                .WithOne(tv => tv.DanhGia)
                .HasForeignKey<DanhGiaChuyenGia>(dg => dg.TuVanSucKhoeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DanhGiaChuyenGia>()
                .HasOne(dg => dg.ChuyenGia)
                .WithMany(cg => cg.DanhGiaDaNhan)
                .HasForeignKey(dg => dg.ChuyenGiaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ một-nhiều giữa NguoiDung và VaiTro
            modelBuilder.Entity<NguoiDung>()
                .HasOne(nd => nd.VaiTro)
                .WithMany(vt => vt.NguoiDungs)
                .HasForeignKey(nd => nd.VaiTroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ một-nhiều giữa NguoiDung và HoSoSucKhoe
            modelBuilder.Entity<HoSoSucKhoe>()
                .HasOne(h => h.NguoiDung)
                .WithMany()
                .HasForeignKey(h => h.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Cấu hình mối quan hệ một-nhiều giữa NguoiDung và LichSuSucKhoe
            modelBuilder.Entity<LichSuSucKhoe>()
                .HasOne(l => l.NguoiDung)
                .WithMany()
                .HasForeignKey(l => l.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Cấu hình mối quan hệ một-nhiều giữa NguoiDung và TuVanSucKhoe
            modelBuilder.Entity<TuVanSucKhoe>()
                .HasOne(tv => tv.NguoiDung)
                .WithMany()
                .HasForeignKey(tv => tv.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Cấu hình mối quan hệ một-nhiều giữa NguoiDung và ChuyenGia
            modelBuilder.Entity<ChuyenGia>()
                .HasOne(cg => cg.NguoiDung)
                .WithMany(nd => nd.ChuyenGias)
                .HasForeignKey(cg => cg.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<LichHen>()
                .HasOne(lh => lh.NguoiDung)
               .WithMany()
               .HasForeignKey(lh => lh.NguoiDungId)
               .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa NguoiDung nếu có LichHen liên quan

            modelBuilder.Entity<LichHen>()
                .HasOne(lh => lh.ChuyenGia)
                .WithMany()
                .HasForeignKey(lh => lh.ChuyenGiaId)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa NguoiDung (ChuyenGia) nếu có LichHen liên quan

            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(h => h.ChieuCao)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(h => h.CanNang)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(h => h.DuongHuyet)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(h => h.HuyetApTamThu)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(h => h.HuyetApTamTruong)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<LichSuSucKhoe>()
                .Property(l => l.ChieuCao)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<LichSuSucKhoe>()
                .Property(l => l.CanNang)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<LichSuSucKhoe>()
                .Property(l => l.DuongHuyet)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<LichSuSucKhoe>()
                .Property(l => l.HuyetApTamThu)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<LichSuSucKhoe>()
                .Property(l => l.HuyetApTamTruong)
                .HasColumnType("decimal(5, 2)");
                
            modelBuilder.Entity<ChiTietKeHoachDinhDuong>()
                .Property(c => c.SoLuong)
                .HasPrecision(18, 2);

            // Cấu hình cho mô hình Appointment
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Cấu hình quan hệ 1-1 giữa NguoiDung và HoSoSucKhoe
            modelBuilder.Entity<NguoiDung>()
                .HasOne(nd => nd.HoSoSucKhoe)
                .WithOne(hssk => hssk.NguoiDung)
                .HasForeignKey<HoSoSucKhoe>(hssk => hssk.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ giữa ThongBaoBacSi và NguoiDung/BacSi để tránh multiple cascade paths
            modelBuilder.Entity<ThongBaoBacSi>()
                .HasOne(tb => tb.NguoiDung)
                .WithMany()
                .HasForeignKey(tb => tb.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThongBaoBacSi>()
                .HasOne(tb => tb.BacSi)
                .WithMany()
                .HasForeignKey(tb => tb.BacSiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình mối quan hệ giữa ChiTietKeHoachTapLuyen và NguoiDung để tránh multiple cascade paths
            modelBuilder.Entity<ChiTietKeHoachTapLuyen>()
                .HasOne(ct => ct.NguoiDung)
                .WithMany()
                .HasForeignKey(ct => ct.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho Message để tránh multiple cascade paths
            modelBuilder.Entity<global::DoAnChamSocSucKhoe.Models.Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<global::DoAnChamSocSucKhoe.Models.Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<global::DoAnChamSocSucKhoe.Models.Message>()
                .HasOne(m => m.TuVanSucKhoe)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TuVanSucKhoeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship for NguoiChamSocBenhNhan
            modelBuilder.Entity<NguoiChamSocBenhNhan>()
                .HasOne(ncs => ncs.NguoiChamSoc)
                .WithMany()
                .HasForeignKey(ncs => ncs.NguoiChamSocId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NguoiChamSocBenhNhan>()
                .HasOne(ncs => ncs.BenhNhan)
                .WithMany()
                .HasForeignKey(ncs => ncs.BenhNhanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
