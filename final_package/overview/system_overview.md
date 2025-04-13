# Tổng quan Hệ thống Quản lý Sức khỏe Cá nhân

## Giới thiệu

Hệ thống Quản lý Sức khỏe Cá nhân là một ứng dụng web được phát triển bằng ASP.NET 9.0, Bootstrap và Razor Pages, nhằm cung cấp một nền tảng toàn diện cho việc theo dõi và quản lý sức khỏe cá nhân. Hệ thống kết nối bệnh nhân với bác sĩ, cho phép tư vấn sức khỏe trực tuyến, theo dõi các chỉ số sức khỏe, và quản lý kế hoạch dinh dưỡng và tập luyện.

## Kiến trúc hệ thống

Hệ thống được xây dựng theo mô hình MVC (Model-View-Controller) với các thành phần chính:

1. **Models**: Định nghĩa cấu trúc dữ liệu và logic nghiệp vụ
2. **Views**: Giao diện người dùng sử dụng Razor Pages và Bootstrap
3. **Controllers**: Xử lý yêu cầu từ người dùng và tương tác với Models
4. **Areas**: Phân chia không gian làm việc cho các vai trò khác nhau (Bệnh nhân, Bác sĩ, Admin)
5. **Services**: Cung cấp các dịch vụ như gửi email, xác thực, v.v.

## Cơ sở dữ liệu

Hệ thống sử dụng Entity Framework Core để tương tác với cơ sở dữ liệu SQL Server. Cấu trúc cơ sở dữ liệu bao gồm 15 bảng chính:

- Quản lý người dùng (vai_tro, nguoi_dung)
- Quản lý sức khỏe (ho_so_suc_khoe, chi_so_suc_khoe, nhac_nho_suc_khoe)
- Quản lý dinh dưỡng và tập luyện (ke_hoach_dinh_duong, chi_tiet_ke_hoach_dinh_duong, ke_hoach_tap_luyen, chi_tiet_ke_hoach_tap_luyen)
- Tương tác bác sĩ-bệnh nhân (tu_van_suc_khoe, lich_hen, tin_nhan, danh_gia_chuyen_gia)
- Quản lý hệ thống (thong_bao_he_thong, phan_hoi_nguoi_dung)

## Tính năng chính

### Dành cho Bệnh nhân
- Đăng ký và quản lý tài khoản
- Theo dõi các chỉ số sức khỏe (nhịp tim, huyết áp, đường huyết, v.v.)
- Xem biểu đồ theo dõi sức khỏe theo thời gian
- Đặt lịch hẹn với bác sĩ
- Gửi câu hỏi tư vấn sức khỏe
- Nhận kế hoạch dinh dưỡng và tập luyện
- Nhận nhắc nhở sức khỏe

### Dành cho Bác sĩ
- Quản lý danh sách bệnh nhân
- Xem hồ sơ sức khỏe của bệnh nhân
- Trả lời câu hỏi tư vấn
- Quản lý lịch hẹn
- Tạo kế hoạch dinh dưỡng và tập luyện cho bệnh nhân
- Theo dõi tiến triển sức khỏe của bệnh nhân

### Dành cho Admin
- Quản lý người dùng
- Quản lý vai trò
- Tạo thông báo hệ thống
- Xem phản hồi từ người dùng

## Công nghệ sử dụng

- **Backend**: ASP.NET 9.0, Entity Framework Core
- **Frontend**: Bootstrap, HTML5, CSS3, JavaScript
- **Cơ sở dữ liệu**: SQL Server
- **Xác thực**: ASP.NET Core Identity
- **Biểu đồ**: Chart.js
- **Lịch**: FullCalendar

## Cấu trúc dự án

```
DoAnChamSocSucKhoe/
├── Areas/
│   ├── Patient/         # Không gian làm việc cho bệnh nhân
│   ├── Doctor/          # Không gian làm việc cho bác sĩ
│   └── Admin/           # Không gian làm việc cho admin
├── Controllers/         # Controllers chung
├── Data/                # Cấu hình cơ sở dữ liệu
├── Models/              # Models định nghĩa cấu trúc dữ liệu
├── Services/            # Các dịch vụ
├── Views/               # Views chung
└── wwwroot/             # Tài nguyên tĩnh (CSS, JS, images)
```

## Hướng dẫn triển khai

Để triển khai hệ thống, hãy tham khảo các hướng dẫn chi tiết trong thư mục `implementation_steps`. Các hướng dẫn được tổ chức theo từng bước triển khai, từ cơ sở dữ liệu đến giao diện người dùng và các tính năng cụ thể.

## Yêu cầu hệ thống

- .NET 9.0 SDK
- SQL Server 2019 trở lên
- Visual Studio 2022 hoặc Visual Studio Code
- Node.js và npm (cho việc quản lý các thư viện JavaScript)

## Kết luận

Hệ thống Quản lý Sức khỏe Cá nhân cung cấp một nền tảng toàn diện cho việc theo dõi và quản lý sức khỏe, kết nối bệnh nhân với bác sĩ, và cung cấp các công cụ để cải thiện sức khỏe. Với giao diện thân thiện và các tính năng phong phú, hệ thống hứa hẹn mang lại trải nghiệm tốt cho người dùng và góp phần nâng cao chất lượng chăm sóc sức khỏe.
