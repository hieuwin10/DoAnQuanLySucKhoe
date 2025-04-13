# Hướng dẫn triển khai Xác thực (Authentication)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai hệ thống xác thực (Authentication) cho hệ thống quản lý sức khỏe cá nhân. Hệ thống xác thực là một phần quan trọng, cho phép người dùng đăng ký, đăng nhập, quản lý tài khoản và phân quyền theo vai trò.

## Thiết kế hệ thống xác thực

Hệ thống xác thực sẽ bao gồm các chức năng sau:

1. **Đăng ký**: Cho phép người dùng tạo tài khoản mới
2. **Đăng nhập**: Cho phép người dùng đăng nhập vào hệ thống
3. **Quên mật khẩu**: Cho phép người dùng khôi phục mật khẩu
4. **Xác nhận email**: Xác nhận địa chỉ email của người dùng
5. **Quản lý tài khoản**: Cho phép người dùng cập nhật thông tin cá nhân
6. **Phân quyền**: Phân quyền người dùng theo vai trò (Admin, Bác sĩ, Bệnh nhân)

## Các bước triển khai

Xem chi tiết trong các tài liệu hướng dẫn:

- [Hướng dẫn triển khai frontend](frontend.md)
- [Hướng dẫn triển khai backend](backend.md)

## Mã nguồn

Các file mã nguồn cần thiết được cung cấp trong thư mục `code/`:

- Register.cshtml: Trang đăng ký
- Login.cshtml: Trang đăng nhập
- ForgotPassword.cshtml: Trang quên mật khẩu
- ResetPassword.cshtml: Trang đặt lại mật khẩu
- Manage/Index.cshtml: Trang quản lý tài khoản
- Manage/ChangePassword.cshtml: Trang đổi mật khẩu
- ApplicationUser.cs: Model người dùng
- EmailSender.cs: Dịch vụ gửi email
