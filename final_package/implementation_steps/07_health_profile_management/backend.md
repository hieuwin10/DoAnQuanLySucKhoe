# Hướng dẫn triển khai Backend - Bước 7: Quản lý Hồ sơ Sức khỏe

## Mục tiêu

Xử lý logic nghiệp vụ và tương tác cơ sở dữ liệu cho chức năng quản lý hồ sơ sức khỏe.

## Các bước thực hiện

1.  **Tạo Model `HealthMetric`:**
    * Trong thư mục `Models` (hoặc thư mục chứa các model của bạn), tạo file `HealthMetric.cs`.
    * Định nghĩa các thuộc tính cần thiết như `Id`, `PatientId` (khóa ngoại liên kết với người dùng), `RecordedDate`, `Weight`, `Height`, `SystolicPressure`, `DiastolicPressure`, `BloodSugar`, `HeartRate`, `Notes`.
    * Thêm các Data Annotations cần thiết cho validation (ví dụ: `[Required]`, `[Range]`).

2.  **Cập nhật DbContext:**
    * Mở file `DbContext` của bạn (ví dụ: `Data/ApplicationDbContext.cs`).
    * Thêm một `DbSet<HealthMetric>` mới:
        ```csharp
        public DbSet<HealthMetric> HealthMetrics { get; set; }
        ```
    * (Quan trọng) Thiết lập mối quan hệ giữa `HealthMetric` và `ApplicationUser` (hoặc model User của bạn) nếu cần thiết, đặc biệt là khóa ngoại `PatientId`. Ví dụ trong phương thức `OnModelCreating`:
        ```csharp
        // Ví dụ nếu bạn dùng ApplicationUser từ ASP.NET Identity
        builder.Entity<HealthMetric>()
            .HasOne<ApplicationUser>() // Hoặc tên lớp User của bạn
            .WithMany() // Hoặc .WithMany(u => u.HealthMetrics) nếu có collection trong User
            .HasForeignKey(hm => hm.PatientId)
            .IsRequired();
        ```

3.  **Tạo và Áp dụng Migration:**
    * Mở Package Manager Console hoặc Terminal.
    * Chạy lệnh: `dotnet ef migrations add AddHealthMetricsTable`
    * Kiểm tra lại file migration vừa tạo trong thư mục `Migrations`.
    * Chạy lệnh: `dotnet ef database update`

4.  **Triển khai PageModel (`HealthProfile.cshtml.cs`):**
    * Trong file `Areas/Patient/Pages/HealthProfile.cshtml.cs`, kế thừa từ `PageModel`.
    * **Inject Dependencies:** Inject `ApplicationDbContext` và `UserManager<ApplicationUser>` (hoặc dịch vụ tương đương để lấy thông tin người dùng hiện tại) vào constructor.
        ```csharp
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Thay ApplicationUser bằng lớp User của bạn

        public HealthProfileModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        ```
    * **Định nghĩa Thuộc tính:**
        * `[BindProperty]` cho một đối tượng `HealthMetric` để nhận dữ liệu từ form POST:
            ```csharp
            [BindProperty]
            public HealthMetric Input { get; set; }
            ```
        * Một danh sách để giữ lịch sử các chỉ số:
            ```csharp
            public IList<HealthMetric> History { get; set; } = new List<HealthMetric>();
            ```
    * **Triển khai `OnGetAsync`:**
        * Lấy ID của người dùng đang đăng nhập.
        * Truy vấn cơ sở dữ liệu để lấy tất cả `HealthMetric` của người dùng đó, sắp xếp theo ngày giảm dần.
        * Gán kết quả cho thuộc tính `History`.
    * **Triển khai `OnPostAsync`:**
        * Kiểm tra `ModelState.IsValid`. Nếu không hợp lệ, gọi lại `OnGetAsync` để tải lại lịch sử và hiển thị lại trang với lỗi validation.
        * Lấy ID của người dùng đang đăng nhập.
        * Gán `PatientId` cho `Input`.
        * Gán `RecordedDate` (ví dụ: `DateTime.UtcNow` hoặc giá trị từ form nếu cho phép chọn).
        * Thêm `Input` vào `_context.HealthMetrics`.
        * Lưu thay đổi vào cơ sở dữ liệu: `await _context.SaveChangesAsync();`.
        * Chuyển hướng người dùng về lại trang `HealthProfile` (để làm mới dữ liệu và tránh việc submit lại form khi refresh): `return RedirectToPage();`.

## Ví dụ mã nguồn

Xem chi tiết trong các file `code/Models/HealthMetric.cs` và `code/Pages/Patient/HealthProfile.cshtml.cs`.