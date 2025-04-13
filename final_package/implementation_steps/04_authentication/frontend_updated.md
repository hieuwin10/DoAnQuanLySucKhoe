# Hướng dẫn triển khai giao diện xác thực (Authentication)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai giao diện người dùng cho hệ thống xác thực (Authentication) của hệ thống quản lý sức khỏe cá nhân. Giao diện xác thực bao gồm các trang đăng ký, đăng nhập, quên mật khẩu, đặt lại mật khẩu và quản lý tài khoản.

## Các thành phần UI cần triển khai

### 1. Trang đăng ký (Register)

Trang đăng ký cho phép người dùng tạo tài khoản mới. Sử dụng file `Register.cshtml` trong thư mục code để triển khai trang này.

Các thành phần chính:
- Form đăng ký với các trường: Họ tên, Email, Mật khẩu, Xác nhận mật khẩu, Vai trò
- Validation cho các trường
- Checkbox đồng ý điều khoản
- Nút đăng ký
- Liên kết đến trang đăng nhập

### 2. Trang đăng nhập (Login)

Trang đăng nhập cho phép người dùng đăng nhập vào hệ thống. Sử dụng file `Login.cshtml` trong thư mục code để triển khai trang này.

Các thành phần chính:
- Form đăng nhập với các trường: Email, Mật khẩu
- Checkbox ghi nhớ đăng nhập
- Nút đăng nhập
- Liên kết đến trang quên mật khẩu và đăng ký

### 3. Trang quên mật khẩu (ForgotPassword)

Trang quên mật khẩu cho phép người dùng khôi phục mật khẩu. Sử dụng file `ForgotPassword.cshtml` trong thư mục code để triển khai trang này.

Các thành phần chính:
- Form quên mật khẩu với trường Email
- Nút gửi
- Liên kết quay lại trang đăng nhập

### 4. Trang đặt lại mật khẩu (ResetPassword)

Trang đặt lại mật khẩu cho phép người dùng tạo mật khẩu mới. Sử dụng file `ResetPassword.cshtml` trong thư mục code để triển khai trang này.

Các thành phần chính:
- Form đặt lại mật khẩu với các trường: Email, Mật khẩu mới, Xác nhận mật khẩu
- Trường ẩn chứa mã xác nhận
- Nút đặt lại mật khẩu

### 5. Trang quản lý tài khoản (Manage/Index)

Trang quản lý tài khoản cho phép người dùng cập nhật thông tin cá nhân. Sử dụng file `ManageIndex.cshtml` trong thư mục code để triển khai trang này.

Các thành phần chính:
- Form cập nhật thông tin với các trường: Ảnh đại diện, Họ tên, Số điện thoại, Giới tính, Ngày sinh, Địa chỉ
- Hiển thị thông tin tài khoản hiện tại
- Nút lưu thay đổi
- Liên kết đến trang đổi mật khẩu

### 6. Trang đổi mật khẩu (Manage/ChangePassword)

Trang đổi mật khẩu cho phép người dùng thay đổi mật khẩu. Sử dụng file `ChangePassword.cshtml` trong thư mục code để triển khai trang này.

Các thành phần chính:
- Form đổi mật khẩu với các trường: Mật khẩu hiện tại, Mật khẩu mới, Xác nhận mật khẩu mới
- Nút cập nhật mật khẩu

### 7. Partial View cho Navigation trong trang quản lý tài khoản (Manage/_ManageNav)

Partial View cho Navigation trong trang quản lý tài khoản. Sử dụng file `ManageNav.cshtml` trong thư mục code để triển khai phần này.

Các thành phần chính:
- Danh sách các liên kết đến các trang quản lý tài khoản
- Hiển thị trạng thái active cho trang hiện tại

### 8. Layout cho trang quản lý tài khoản (Manage/_Layout)

Layout cho trang quản lý tài khoản. Sử dụng file `ManageLayout.cshtml` trong thư mục code để triển khai phần này.

Các thành phần chính:
- Cấu trúc trang với sidebar và content
- Hiển thị partial view _ManageNav trong sidebar
- Hiển thị nội dung trang trong content

## CSS cho trang xác thực

Sử dụng file `auth.css` trong thư mục code để định dạng giao diện các trang xác thực.

Các style chính:
- Container cho các trang xác thực
- Card cho form đăng ký, đăng nhập, v.v.
- Style cho form, input, button
- Style cho profile picture
- Style cho navigation trong trang quản lý tài khoản

## JavaScript cho trang xác thực

Sử dụng file `auth.js` trong thư mục code để thêm các tính năng JavaScript cho các trang xác thực.

Các tính năng chính:
- Validation cho form đăng ký và đăng nhập
- Password strength meter
- Preview ảnh đại diện khi upload

## Hướng dẫn triển khai

### 1. Tạo Razor Pages cho Identity

Chạy lệnh sau để tạo các trang Razor cho Identity:

```bash
dotnet aspnet-codegenerator identity -dc DoAnChamSocSucKhoe.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.ForgotPassword;Account.ResetPassword;Account.Manage.Index;Account.Manage.ChangePassword"
```

### 2. Tùy chỉnh trang đăng ký

Thay thế nội dung file `Areas/Identity/Pages/Account/Register.cshtml` bằng nội dung từ file `Register.cshtml` trong thư mục code.

### 3. Tùy chỉnh trang đăng nhập

Thay thế nội dung file `Areas/Identity/Pages/Account/Login.cshtml` bằng nội dung từ file `Login.cshtml` trong thư mục code.

### 4. Tùy chỉnh trang quên mật khẩu

Thay thế nội dung file `Areas/Identity/Pages/Account/ForgotPassword.cshtml` bằng nội dung từ file `ForgotPassword.cshtml` trong thư mục code.

### 5. Tùy chỉnh trang đặt lại mật khẩu

Thay thế nội dung file `Areas/Identity/Pages/Account/ResetPassword.cshtml` bằng nội dung từ file `ResetPassword.cshtml` trong thư mục code.

### 6. Tùy chỉnh trang quản lý tài khoản

Thay thế nội dung file `Areas/Identity/Pages/Account/Manage/Index.cshtml` bằng nội dung từ file `ManageIndex.cshtml` trong thư mục code.

### 7. Tùy chỉnh trang đổi mật khẩu

Thay thế nội dung file `Areas/Identity/Pages/Account/Manage/ChangePassword.cshtml` bằng nội dung từ file `ChangePassword.cshtml` trong thư mục code.

### 8. Tùy chỉnh layout cho trang quản lý tài khoản

Thay thế nội dung file `Areas/Identity/Pages/Account/Manage/_Layout.cshtml` bằng nội dung từ file `ManageLayout.cshtml` trong thư mục code.

### 9. Tùy chỉnh navigation cho trang quản lý tài khoản

Thay thế nội dung file `Areas/Identity/Pages/Account/Manage/_ManageNav.cshtml` bằng nội dung từ file `ManageNav.cshtml` trong thư mục code.

### 10. Thêm CSS cho trang xác thực

Tạo file `wwwroot/css/auth.css` với nội dung từ file `auth.css` trong thư mục code.

### 11. Thêm JavaScript cho trang xác thực

Tạo file `wwwroot/js/auth.js` với nội dung từ file `auth.js` trong thư mục code.

### 12. Cập nhật _Layout.cshtml để thêm CSS và JavaScript

Thêm các thẻ link và script vào file `Views/Shared/_Layout.cshtml`:

```html
<!-- Trong phần head -->
<link rel="stylesheet" href="~/css/auth.css" />

<!-- Trước thẻ đóng body -->
<script src="~/js/auth.js"></script>
```

## Lưu ý

- Đảm bảo tất cả các trang đều responsive và hiển thị tốt trên các thiết bị khác nhau
- Thêm validation phía client để cải thiện trải nghiệm người dùng
- Sử dụng các thành phần UI của Bootstrap để tạo giao diện nhất quán
- Tùy chỉnh các thông báo lỗi và xác nhận để phù hợp với ngôn ngữ tiếng Việt
- Thêm các tính năng bảo mật như CAPTCHA để tránh tấn công brute force
