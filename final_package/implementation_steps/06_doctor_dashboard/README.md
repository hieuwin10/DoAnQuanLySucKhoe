# Hướng dẫn triển khai Bảng điều khiển Bác sĩ (Doctor Dashboard)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai bảng điều khiển bác sĩ (Doctor Dashboard) cho hệ thống quản lý sức khỏe cá nhân. Bảng điều khiển bác sĩ là trang chính mà người dùng với vai trò bác sĩ sẽ thấy sau khi đăng nhập, hiển thị danh sách bệnh nhân, lịch hẹn, tư vấn, và các thông tin liên quan khác.

## Thiết kế giao diện

Bảng điều khiển bác sĩ sẽ bao gồm các phần chính sau:

1. **Tổng quan**: Hiển thị số lượng bệnh nhân, lịch hẹn hôm nay, tư vấn chưa trả lời
2. **Lịch hẹn sắp tới**: Danh sách các lịch hẹn sắp tới với bệnh nhân
3. **Danh sách bệnh nhân**: Danh sách các bệnh nhân đang theo dõi
4. **Tư vấn chưa trả lời**: Danh sách các câu hỏi tư vấn chưa trả lời từ bệnh nhân
5. **Thống kê**: Biểu đồ thống kê số lượng bệnh nhân, lịch hẹn, tư vấn theo thời gian

## Các bước triển khai

Xem chi tiết trong các tài liệu hướng dẫn:

- [Hướng dẫn triển khai frontend](frontend.md)
- [Hướng dẫn triển khai backend](backend.md)

## Mã nguồn

Các file mã nguồn cần thiết được cung cấp trong thư mục `code/`:

- DoctorDashboardController.cs: Controller cho bảng điều khiển bác sĩ
- DoctorDashboardViewModel.cs: ViewModel cho bảng điều khiển bác sĩ
- Index.cshtml: View cho bảng điều khiển bác sĩ
- _DoctorLayout.cshtml: Layout cho khu vực bác sĩ
- doctor-dashboard.js: JavaScript cho bảng điều khiển bác sĩ
- doctor.css: CSS cho bảng điều khiển bác sĩ
