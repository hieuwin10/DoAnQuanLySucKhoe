# Hướng dẫn triển khai Bảng điều khiển Bệnh nhân (Patient Dashboard)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai bảng điều khiển bệnh nhân (Patient Dashboard) cho hệ thống quản lý sức khỏe cá nhân. Bảng điều khiển bệnh nhân là trang chính mà người dùng với vai trò bệnh nhân sẽ thấy sau khi đăng nhập, hiển thị thông tin sức khỏe, biểu đồ theo dõi, lịch hẹn, và các thông tin liên quan khác.

## Thiết kế giao diện

Bảng điều khiển bệnh nhân sẽ bao gồm các phần chính sau:

1. **Tổng quan sức khỏe**: Hiển thị các chỉ số sức khỏe cơ bản (BMI, nhịp tim, huyết áp, đường huyết)
2. **Biểu đồ theo dõi**: Hiển thị biểu đồ theo dõi các chỉ số sức khỏe theo thời gian
3. **Lịch hẹn sắp tới**: Danh sách các lịch hẹn sắp tới với bác sĩ
4. **Nhắc nhở sức khỏe**: Danh sách các nhắc nhở sức khỏe (uống thuốc, tập luyện, v.v.)
5. **Tư vấn gần đây**: Danh sách các câu hỏi tư vấn gần đây và câu trả lời từ bác sĩ
6. **Kế hoạch dinh dưỡng và tập luyện**: Hiển thị kế hoạch dinh dưỡng và tập luyện hiện tại

## Các bước triển khai

Xem chi tiết trong các tài liệu hướng dẫn:

- [Hướng dẫn triển khai frontend](frontend.md)
- [Hướng dẫn triển khai backend](backend.md)

## Mã nguồn

Các file mã nguồn cần thiết được cung cấp trong thư mục `code/`:

- PatientDashboardController.cs: Controller cho bảng điều khiển bệnh nhân
- PatientDashboardViewModel.cs: ViewModel cho bảng điều khiển bệnh nhân
- Index.cshtml: View cho bảng điều khiển bệnh nhân
- _PatientLayout.cshtml: Layout cho khu vực bệnh nhân
- patient-dashboard.js: JavaScript cho bảng điều khiển bệnh nhân
- patient.css: CSS cho bảng điều khiển bệnh nhân
