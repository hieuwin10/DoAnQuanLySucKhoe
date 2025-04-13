# Hướng dẫn triển khai giao diện bảng điều khiển bác sĩ (Doctor Dashboard)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai giao diện người dùng cho bảng điều khiển bác sĩ (Doctor Dashboard) của hệ thống quản lý sức khỏe cá nhân. Bảng điều khiển bác sĩ là trang chính mà người dùng với vai trò bác sĩ sẽ thấy sau khi đăng nhập, hiển thị danh sách bệnh nhân, lịch hẹn, tư vấn, và các thông tin liên quan khác.

## Các thành phần UI cần triển khai

### 1. Layout cho khu vực bác sĩ

Trước tiên, cần tạo layout chung cho khu vực bác sĩ. Sử dụng file `_DoctorLayout.cshtml` trong thư mục code để triển khai layout này.

Các thành phần chính:
- Navbar với logo và menu
- Sidebar với các liên kết đến các trang trong khu vực bác sĩ
- Vùng hiển thị nội dung chính
- Footer

### 2. Tổng quan

Phần tổng quan hiển thị số lượng bệnh nhân, lịch hẹn hôm nay, tư vấn chưa trả lời. Sử dụng các thẻ card để hiển thị từng thông tin.

Thông tin cần hiển thị:
- Tổng số bệnh nhân đang theo dõi
- Số lượng lịch hẹn hôm nay
- Số lượng tư vấn chưa trả lời
- Số lượng bệnh nhân mới trong tháng

### 3. Lịch hẹn sắp tới

Phần lịch hẹn sắp tới hiển thị danh sách các lịch hẹn sắp tới với bệnh nhân. Sử dụng bảng hoặc danh sách để hiển thị.

Thông tin cần hiển thị cho mỗi lịch hẹn:
- Ngày giờ
- Tên bệnh nhân
- Lý do khám
- Trạng thái (Đã xác nhận, Chờ xác nhận, v.v.)
- Các hành động (Xác nhận, Hủy, Hoàn thành)

### 4. Danh sách bệnh nhân

Phần danh sách bệnh nhân hiển thị danh sách các bệnh nhân đang theo dõi. Sử dụng bảng để hiển thị với tính năng tìm kiếm và lọc.

Thông tin cần hiển thị cho mỗi bệnh nhân:
- Tên bệnh nhân
- Tuổi
- Giới tính
- Số điện thoại
- Email
- Ngày khám gần nhất
- Các hành động (Xem hồ sơ, Đặt lịch hẹn, Tư vấn)

### 5. Tư vấn chưa trả lời

Phần tư vấn chưa trả lời hiển thị danh sách các câu hỏi tư vấn chưa trả lời từ bệnh nhân. Sử dụng card hoặc accordion để hiển thị.

Thông tin cần hiển thị cho mỗi tư vấn:
- Tên bệnh nhân
- Tiêu đề câu hỏi
- Nội dung câu hỏi
- Thời gian gửi
- Form trả lời

### 6. Thống kê

Phần thống kê hiển thị biểu đồ thống kê số lượng bệnh nhân, lịch hẹn, tư vấn theo thời gian. Sử dụng thư viện Chart.js để tạo biểu đồ.

Các biểu đồ cần tạo:
- Biểu đồ số lượng bệnh nhân theo tháng
- Biểu đồ số lượng lịch hẹn theo ngày trong tuần
- Biểu đồ số lượng tư vấn theo tháng

## Hướng dẫn triển khai

### 1. Tạo layout cho khu vực bác sĩ

Tạo file `Views/Shared/_DoctorLayout.cshtml` với nội dung từ file `_DoctorLayout.cshtml` trong thư mục code.

### 2. Tạo trang bảng điều khiển bác sĩ

Tạo file `Views/Doctor/Index.cshtml` với nội dung từ file `DoctorDashboard.cshtml` trong thư mục code.

### 3. Tạo CSS cho bảng điều khiển bác sĩ

Tạo file `wwwroot/css/doctor.css` với nội dung từ file `doctor.css` trong thư mục code.

### 4. Tạo JavaScript cho bảng điều khiển bác sĩ

Tạo file `wwwroot/js/doctor-dashboard.js` với nội dung từ file `doctor-dashboard.js` trong thư mục code.

### 5. Thêm thư viện Chart.js

Thêm thư viện Chart.js vào file `Views/Shared/_DoctorLayout.cshtml`:

```html
<!-- Trong phần head -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
```

### 6. Thêm thư viện Bootstrap Icons

Thêm thư viện Bootstrap Icons vào file `Views/Shared/_DoctorLayout.cshtml`:

```html
<!-- Trong phần head -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css">
```

### 7. Cập nhật _ViewStart.cshtml cho khu vực bác sĩ

Tạo file `Views/Doctor/_ViewStart.cshtml` với nội dung sau:

```csharp
@{
    Layout = "_DoctorLayout";
}
```

## Lưu ý

- Đảm bảo tất cả các thành phần UI đều responsive và hiển thị tốt trên các thiết bị khác nhau
- Sử dụng các thành phần UI của Bootstrap để tạo giao diện nhất quán
- Tối ưu hóa hiệu suất của biểu đồ khi hiển thị dữ liệu lớn
- Thêm tính năng lọc và tìm kiếm cho các danh sách dài
- Sử dụng AJAX để cập nhật dữ liệu mà không cần tải lại trang
- Thêm animation và hiệu ứng để tăng trải nghiệm người dùng
- Đảm bảo giao diện thân thiện và dễ sử dụng cho bác sĩ
