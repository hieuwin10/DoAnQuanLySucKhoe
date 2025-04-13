# Hướng dẫn triển khai giao diện bảng điều khiển bệnh nhân (Patient Dashboard)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai giao diện người dùng cho bảng điều khiển bệnh nhân (Patient Dashboard) của hệ thống quản lý sức khỏe cá nhân. Bảng điều khiển bệnh nhân là trang chính mà người dùng với vai trò bệnh nhân sẽ thấy sau khi đăng nhập, hiển thị thông tin sức khỏe, biểu đồ theo dõi, lịch hẹn, và các thông tin liên quan khác.

## Các thành phần UI cần triển khai

### 1. Layout cho khu vực bệnh nhân

Trước tiên, cần tạo layout chung cho khu vực bệnh nhân. Sử dụng file `_PatientLayout.cshtml` trong thư mục code để triển khai layout này.

Các thành phần chính:
- Navbar với logo và menu
- Sidebar với các liên kết đến các trang trong khu vực bệnh nhân
- Vùng hiển thị nội dung chính
- Footer

### 2. Tổng quan sức khỏe

Phần tổng quan sức khỏe hiển thị các chỉ số sức khỏe cơ bản. Sử dụng các thẻ card để hiển thị từng chỉ số.

Các chỉ số cần hiển thị:
- BMI (Chỉ số khối cơ thể)
- Nhịp tim
- Huyết áp
- Đường huyết

### 3. Biểu đồ theo dõi

Phần biểu đồ theo dõi hiển thị biểu đồ các chỉ số sức khỏe theo thời gian. Sử dụng thư viện Chart.js để tạo biểu đồ.

Các biểu đồ cần tạo:
- Biểu đồ cân nặng theo thời gian
- Biểu đồ nhịp tim theo thời gian
- Biểu đồ huyết áp theo thời gian
- Biểu đồ đường huyết theo thời gian

### 4. Lịch hẹn sắp tới

Phần lịch hẹn sắp tới hiển thị danh sách các lịch hẹn sắp tới với bác sĩ. Sử dụng bảng hoặc danh sách để hiển thị.

Thông tin cần hiển thị cho mỗi lịch hẹn:
- Ngày giờ
- Tên bác sĩ
- Chuyên khoa
- Lý do khám
- Trạng thái (Đã xác nhận, Chờ xác nhận, v.v.)

### 5. Nhắc nhở sức khỏe

Phần nhắc nhở sức khỏe hiển thị danh sách các nhắc nhở sức khỏe. Sử dụng danh sách để hiển thị.

Thông tin cần hiển thị cho mỗi nhắc nhở:
- Loại nhắc nhở (Uống thuốc, Tập luyện, v.v.)
- Nội dung
- Thời gian
- Trạng thái (Đã hoàn thành, Chưa hoàn thành)

### 6. Tư vấn gần đây

Phần tư vấn gần đây hiển thị danh sách các câu hỏi tư vấn gần đây và câu trả lời từ bác sĩ. Sử dụng card hoặc accordion để hiển thị.

Thông tin cần hiển thị cho mỗi tư vấn:
- Tiêu đề câu hỏi
- Nội dung câu hỏi
- Tên bác sĩ trả lời
- Nội dung trả lời
- Thời gian

### 7. Kế hoạch dinh dưỡng và tập luyện

Phần kế hoạch dinh dưỡng và tập luyện hiển thị kế hoạch dinh dưỡng và tập luyện hiện tại. Sử dụng tab để chuyển đổi giữa hai loại kế hoạch.

Thông tin cần hiển thị cho kế hoạch dinh dưỡng:
- Ngày áp dụng
- Danh sách các bữa ăn trong ngày
- Thông tin dinh dưỡng (Calo, Protein, Carb, Fat)

Thông tin cần hiển thị cho kế hoạch tập luyện:
- Ngày áp dụng
- Danh sách các bài tập
- Thời gian tập luyện
- Calo tiêu thụ

## Hướng dẫn triển khai

### 1. Tạo layout cho khu vực bệnh nhân

Tạo file `Views/Shared/_PatientLayout.cshtml` với nội dung từ file `_PatientLayout.cshtml` trong thư mục code.

### 2. Tạo trang bảng điều khiển bệnh nhân

Tạo file `Views/Patient/Index.cshtml` với nội dung từ file `PatientDashboard.cshtml` trong thư mục code.

### 3. Tạo CSS cho bảng điều khiển bệnh nhân

Tạo file `wwwroot/css/patient.css` với nội dung từ file `patient.css` trong thư mục code.

### 4. Tạo JavaScript cho bảng điều khiển bệnh nhân

Tạo file `wwwroot/js/patient-dashboard.js` với nội dung từ file `patient-dashboard.js` trong thư mục code.

### 5. Thêm thư viện Chart.js

Thêm thư viện Chart.js vào file `Views/Shared/_PatientLayout.cshtml`:

```html
<!-- Trong phần head -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
```

### 6. Thêm thư viện Bootstrap Icons

Thêm thư viện Bootstrap Icons vào file `Views/Shared/_PatientLayout.cshtml`:

```html
<!-- Trong phần head -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css">
```

### 7. Cập nhật _ViewStart.cshtml cho khu vực bệnh nhân

Tạo file `Views/Patient/_ViewStart.cshtml` với nội dung sau:

```csharp
@{
    Layout = "_PatientLayout";
}
```

## Lưu ý

- Đảm bảo tất cả các thành phần UI đều responsive và hiển thị tốt trên các thiết bị khác nhau
- Sử dụng các thành phần UI của Bootstrap để tạo giao diện nhất quán
- Tối ưu hóa hiệu suất của biểu đồ khi hiển thị dữ liệu lớn
- Thêm tính năng lọc và tìm kiếm cho các danh sách dài
- Sử dụng AJAX để cập nhật dữ liệu mà không cần tải lại trang
- Thêm animation và hiệu ứng để tăng trải nghiệm người dùng
