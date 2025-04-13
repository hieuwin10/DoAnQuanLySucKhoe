# Hướng dẫn triển khai backend cơ sở dữ liệu

## Tổng quan

Tài liệu này hướng dẫn cách triển khai phần backend của cơ sở dữ liệu cho hệ thống quản lý sức khỏe cá nhân. Phần này bao gồm việc tạo các model, cấu hình Entity Framework Core, tạo migration và cập nhật cơ sở dữ liệu.

## Các bước triển khai

### 1. Tạo các model

Đầu tiên, cần tạo các class model tương ứng với các bảng trong cơ sở dữ liệu. Các model này sẽ được đặt trong thư mục `Models`.

#### Tạo model VaiTro

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    [Table("vai_tro")]
    public class VaiTro
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("ten_vai_tro")]
        [StringLength(50)]
        public string TenVaiTro { get; set; }

        // Navigation properties
        public virtual ICollection<ApplicationUser> NguoiDungs { get; set; }
    }
}
```

#### Tạo model HoSoSucKhoe

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChamSocSucKhoe.Models
{
    [Table("ho_so_suc_khoe")]
    public class HoSoSucKhoe
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nguoi_dung_id")]
        public string NguoiDungId { get; set; }

        [Column("chieu_cao")]
        public decimal? ChieuCao { get; set; }

        [Column("can_nang")]
        public decimal? CanNang { get; set; }

        [Column("nhip_tim")]
        public int? NhipTim { get; set; }

        [Column("duong_huyet")]
        public decimal? DuongHuyet { get; set; }

        [Column("huyet_ap_tam_thu")]
        public int? HuyetApTamThu { get; set; }

        [Column("huyet_ap_tam_truong")]
        public int? HuyetApTamTruong { get; set; }

        [Column("ngay_cap_nhat")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("NguoiDungId")]
        public virtual ApplicationUser NguoiDung { get; set; }
    }
}
```

Tương tự, tạo các model khác cho các bảng còn lại trong cơ sở dữ liệu.

### 2. Cập nhật ApplicationDbContext

Cập nhật file `Data/ApplicationDbContext.cs` để thêm các DbSet cho các model đã tạo:

```csharp
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<HoSoSucKhoe> HoSoSucKhoes { get; set; }
        public DbSet<ChiSoSucKhoe> ChiSoSucKhoes { get; set; }
        public DbSet<NhacNhoSucKhoe> NhacNhoSucKhoes { get; set; }
        public DbSet<KeHoachDinhDuong> KeHoachDinhDuongs { get; set; }
        public DbSet<ChiTietKeHoachDinhDuong> ChiTietKeHoachDinhDuongs { get; set; }
        public DbSet<KeHoachTapLuyen> KeHoachTapLuyens { get; set; }
        public DbSet<ChiTietKeHoachTapLuyen> ChiTietKeHoachTapLuyens { get; set; }
        public DbSet<TuVanSucKhoe> TuVanSucKhoes { get; set; }
        public DbSet<LichHen> LichHens { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }
        public DbSet<DanhGiaChuyenGia> DanhGiaChuyenGias { get; set; }
        public DbSet<ThongBaoHeThong> ThongBaoHeThongs { get; set; }
        public DbSet<PhanHoiNguoiDung> PhanHoiNguoiDungs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các mối quan hệ và ràng buộc
            
            // VaiTro - ApplicationUser (1-n)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne<VaiTro>()
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(u => u.VaiTroId);

            // ApplicationUser - HoSoSucKhoe (1-n)
            modelBuilder.Entity<HoSoSucKhoe>()
                .HasOne(h => h.NguoiDung)
                .WithMany()
                .HasForeignKey(h => h.NguoiDungId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thêm các cấu hình khác cho các mối quan hệ còn lại
        }
    }
}
```

### 3. Cấu hình kết nối cơ sở dữ liệu

Cập nhật file `appsettings.json` để thêm chuỗi kết nối cơ sở dữ liệu:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DoAnChamSocSucKhoe;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 4. Cấu hình Entity Framework Core trong Program.cs

Cập nhật file `Program.cs` để cấu hình Entity Framework Core:

```csharp
// Thêm dịch vụ DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

### 5. Tạo Migration và cập nhật cơ sở dữ liệu

Sử dụng Entity Framework Core Migrations để tạo cơ sở dữ liệu:

```bash
# Tạo migration ban đầu
dotnet ef migrations add InitialCreate

# Cập nhật cơ sở dữ liệu
dotnet ef database update
```

### 6. Tạo dữ liệu mẫu (Seeding)

Tạo file `Data/DbInitializer.cs` để khởi tạo dữ liệu mẫu:

```csharp
using DoAnChamSocSucKhoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoAnChamSocSucKhoe.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Đảm bảo cơ sở dữ liệu đã được tạo
            context.Database.EnsureCreated();

            // Kiểm tra xem đã có dữ liệu trong bảng vai_tro chưa
            if (await context.VaiTros.AnyAsync())
            {
                return; // Đã có dữ liệu, không cần seed
            }

            // Tạo các vai trò
            var vaiTros = new VaiTro[]
            {
                new VaiTro { TenVaiTro = "Admin" },
                new VaiTro { TenVaiTro = "Doctor" },
                new VaiTro { TenVaiTro = "Patient" }
            };

            foreach (var vaiTro in vaiTros)
            {
                context.VaiTros.Add(vaiTro);
            }
            await context.SaveChangesAsync();

            // Tạo các vai trò trong Identity
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

            // Tạo tài khoản mẫu
            // ...

            await context.SaveChangesAsync();
        }
    }
}
```

### 7. Cập nhật Program.cs để khởi tạo dữ liệu mẫu

Cập nhật file `Program.cs` để gọi phương thức khởi tạo dữ liệu mẫu:

```csharp
// Thêm vào cuối file Program.cs, trước app.Run()
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbInitializer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
```

## Tạo Repository và Service

### 1. Tạo Interface Repository

Tạo thư mục `Repositories` và thêm các interface repository:

```csharp
// Repositories/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### 2. Tạo Repository Implementation

```csharp
// Repositories/Repository.cs
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

### 3. Tạo Service Interface

Tạo thư mục `Services` và thêm các interface service:

```csharp
// Services/IHoSoSucKhoeService.cs
public interface IHoSoSucKhoeService
{
    Task<HoSoSucKhoe> GetHoSoSucKhoeByNguoiDungIdAsync(string nguoiDungId);
    Task<HoSoSucKhoe> UpdateHoSoSucKhoeAsync(HoSoSucKhoe hoSoSucKhoe);
    Task<IEnumerable<ChiSoSucKhoe>> GetChiSoSucKhoeByNguoiDungIdAsync(string nguoiDungId, string loaiChiSo, int limit = 30);
    Task<ChiSoSucKhoe> AddChiSoSucKhoeAsync(ChiSoSucKhoe chiSoSucKhoe);
}
```

### 4. Tạo Service Implementation

```csharp
// Services/HoSoSucKhoeService.cs
public class HoSoSucKhoeService : IHoSoSucKhoeService
{
    private readonly IRepository<HoSoSucKhoe> _hoSoSucKhoeRepository;
    private readonly IRepository<ChiSoSucKhoe> _chiSoSucKhoeRepository;
    private readonly ApplicationDbContext _context;

    public HoSoSucKhoeService(
        IRepository<HoSoSucKhoe> hoSoSucKhoeRepository,
        IRepository<ChiSoSucKhoe> chiSoSucKhoeRepository,
        ApplicationDbContext context)
    {
        _hoSoSucKhoeRepository = hoSoSucKhoeRepository;
        _chiSoSucKhoeRepository = chiSoSucKhoeRepository;
        _context = context;
    }

    public async Task<HoSoSucKhoe> GetHoSoSucKhoeByNguoiDungIdAsync(string nguoiDungId)
    {
        return await _context.HoSoSucKhoes
            .FirstOrDefaultAsync(h => h.NguoiDungId == nguoiDungId);
    }

    public async Task<HoSoSucKhoe> UpdateHoSoSucKhoeAsync(HoSoSucKhoe hoSoSucKhoe)
    {
        hoSoSucKhoe.NgayCapNhat = DateTime.Now;
        await _hoSoSucKhoeRepository.UpdateAsync(hoSoSucKhoe);
        return hoSoSucKhoe;
    }

    public async Task<IEnumerable<ChiSoSucKhoe>> GetChiSoSucKhoeByNguoiDungIdAsync(string nguoiDungId, string loaiChiSo, int limit = 30)
    {
        return await _context.ChiSoSucKhoes
            .Where(c => c.NguoiDungId == nguoiDungId && c.LoaiChiSo == loaiChiSo)
            .OrderByDescending(c => c.ThoiGianDo)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ChiSoSucKhoe> AddChiSoSucKhoeAsync(ChiSoSucKhoe chiSoSucKhoe)
    {
        chiSoSucKhoe.ThoiGianDo = DateTime.Now;
        return await _chiSoSucKhoeRepository.AddAsync(chiSoSucKhoe);
    }
}
```

### 5. Đăng ký Repository và Service trong Program.cs

```csharp
// Đăng ký Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Đăng ký Service
builder.Services.AddScoped<IHoSoSucKhoeService, HoSoSucKhoeService>();
// Đăng ký các service khác
```

## Tích hợp với frontend

Các service này sẽ được sử dụng trong các controller để xử lý yêu cầu từ frontend. Xem chi tiết trong [Hướng dẫn triển khai frontend](frontend.md).

## Lưu ý

- Đảm bảo cấu hình chuỗi kết nối cơ sở dữ liệu chính xác
- Sử dụng migration để quản lý thay đổi cơ sở dữ liệu
- Tối ưu hóa truy vấn cơ sở dữ liệu để cải thiện hiệu suất
- Sử dụng transaction khi cần thiết để đảm bảo tính toàn vẹn dữ liệu
- Xử lý ngoại lệ và ghi log để dễ dàng debug khi có lỗi
