# Bước 7: Triển khai Quản lý Hồ sơ Sức khỏe

## Mục tiêu

Bước này tập trung vào việc xây dựng chức năng cho phép bệnh nhân quản lý hồ sơ sức khỏe cá nhân của họ. Bao gồm:

- Tạo trang hiển thị và nhập liệu các chỉ số sức khỏe.
- Thiết kế form nhập liệu thân thiện sử dụng Bootstrap.
- Lưu trữ các chỉ số sức khỏe vào cơ sở dữ liệu.
- Hiển thị lịch sử các lần nhập chỉ số sức khỏe.

## Các thành phần chính

- **Model:** `HealthMetric` để lưu trữ các chỉ số sức khỏe.
- **Razor Page:** `HealthProfile.cshtml` và `HealthProfile.cshtml.cs` trong Area "Patient" để xử lý giao diện và logic.
- **Cơ sở dữ liệu:** Cập nhật `DbContext` và tạo migration để thêm bảng `HealthMetrics`.

## Kết quả mong đợi

Sau khi hoàn thành bước này, bệnh nhân có thể truy cập vào trang quản lý hồ sơ sức khỏe, nhập các chỉ số mới (cân nặng, chiều cao, huyết áp, đường huyết, nhịp tim,...) và xem lại lịch sử các chỉ số đã nhập trước đó.