# Hướng dẫn triển khai Giao diện - Bước 7: Quản lý Hồ sơ Sức khỏe

## Mục tiêu

Tạo giao diện người dùng cho phép bệnh nhân nhập và xem lịch sử các chỉ số sức khỏe trong Area "Patient".

## Các bước thực hiện

1.  **Tạo Razor Page:**
    * Trong thư mục `Areas/Patient/Pages/`, tạo một Razor Page mới tên là `HealthProfile.cshtml`.
    * Page này sẽ bao gồm hai phần chính: Form nhập liệu và Bảng hiển thị lịch sử.

2.  **Thiết kế Form Nhập liệu (`HealthProfile.cshtml`):**
    * Sử dụng các thành phần form của Bootstrap (`form-group`, `form-control`, `btn`, etc.) để tạo form nhập liệu.
    * Các trường cần có (có thể tùy chỉnh thêm):
        * Ngày ghi nhận (`RecordedDate` - có thể để mặc định là ngày hiện tại hoặc cho phép chọn).
        * Cân nặng (`Weight` - kg).
        * Chiều cao (`Height` - cm).
        * Huyết áp tâm thu (`SystolicPressure` - mmHg).
        * Huyết áp tâm trương (`DiastolicPressure` - mmHg).
        * Đường huyết (`BloodSugar` - mg/dL hoặc mmol/L).
        * Nhịp tim (`HeartRate` - bpm).
        * Ghi chú (`Notes` - tùy chọn).
    * Sử dụng thẻ `asp-for` để liên kết các input với thuộc tính `Input` trong PageModel.
    * Thêm nút "Lưu" (Submit) để gửi dữ liệu form.
    * Sử dụng `asp-validation-summary` và `asp-validation-for` để hiển thị thông báo lỗi validation.

3.  **Thiết kế Bảng Hiển thị Lịch sử (`HealthProfile.cshtml`):**
    * Sử dụng bảng của Bootstrap (`table`, `table-striped`, `table-bordered`, etc.) để hiển thị danh sách các `HealthMetric` đã lưu.
    * Các cột cần hiển thị: Ngày ghi nhận, Cân nặng, Chiều cao, Huyết áp, Đường huyết, Nhịp tim, Ghi chú.
    * Duyệt qua danh sách `Model.History` (sẽ được định nghĩa trong PageModel) để hiển thị dữ liệu.

4.  **(Tùy chọn) Thêm CSS/JS tùy chỉnh:**
    * Nếu cần style đặc biệt, tạo file `wwwroot/css/health-profile.css` và liên kết trong `HealthProfile.cshtml` hoặc `_Layout.cshtml` của Area.
    * Nếu cần xử lý JavaScript phía client (ví dụ: validation phức tạp, date picker), tạo file `wwwroot/js/health-profile.js` và liên kết.

## Ví dụ mã nguồn (`HealthProfile.cshtml`)

Xem chi tiết trong thư mục `code/Pages/Patient/`. File này sẽ chứa mã HTML và Razor để render form và bảng lịch sử sử dụng Bootstrap.