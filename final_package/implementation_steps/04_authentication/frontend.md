# Hướng dẫn triển khai giao diện xác thực (Authentication)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai giao diện người dùng cho hệ thống xác thực (Authentication) của hệ thống quản lý sức khỏe cá nhân. Giao diện xác thực bao gồm các trang đăng ký, đăng nhập, quên mật khẩu, đặt lại mật khẩu và quản lý tài khoản.

## Các thành phần UI cần triển khai

### 1. Trang đăng ký (Register)

Trang đăng ký cho phép người dùng tạo tài khoản mới:

```html
@page
@model RegisterModel
@{
    ViewData["Title"] = "Đăng ký";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            <div class="card border-0 shadow-sm">
                <div class="card-body p-4">
                    <h2 class="text-center mb-4">@ViewData["Title"]</h2>
                    <form id="registerForm" asp-route-returnUrl="@Model.ReturnUrl" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.HoTen" class="form-control" autocomplete="name" aria-required="true" placeholder="Nguyễn Văn A" />
                            <label asp-for="Input.HoTen">Họ tên</label>
                            <span asp-validation-for="Input.HoTen" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                            <label asp-for="Input.Email">Email</label>
                            <span asp-validation-for="Input.Email" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                            <label asp-for="Input.Password">Mật khẩu</label>
                            <span asp-validation-for="Input.Password" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                            <label asp-for="Input.ConfirmPassword">Xác nhận mật khẩu</label>
                            <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <select asp-for="Input.VaiTro" class="form-select" aria-required="true">
                                <option value="">-- Chọn vai trò --</option>
                                <option value="Patient">Bệnh nhân</option>
                                <option value="Doctor">Bác sĩ</option>
                            </select>
                            <label asp-for="Input.VaiTro">Vai trò</label>
                            <span asp-validation-for="Input.VaiTro" class="text-danger"></span>
                        </div>
                        <div class="form-check mb-3">
                            <input class="form-check-input" type="checkbox" id="acceptTerms" name="acceptTerms" required />
                            <label class="form-check-label" for="acceptTerms">
                                Tôi đồng ý với <a href="/Home/Privacy">điều khoản sử dụng</a> và <a href="/Home/Privacy">chính sách bảo mật</a>
                            </label>
                        </div>
                        <button id="registerSubmit" type="submit" class="w-100 btn btn-lg btn-primary">Đăng ký</button>
                    </form>
                    <hr class="my-4">
                    <div class="text-center">
                        <p>Đã có tài khoản? <a asp-page="./Login" asp-route-returnUrl="@Model.ReturnUrl">Đăng nhập</a></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 2. Trang đăng nhập (Login)

Trang đăng nhập cho phép người dùng đăng nhập vào hệ thống:

```html
@page
@model LoginModel
@{
    ViewData["Title"] = "Đăng nhập";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            <div class="card border-0 shadow-sm">
                <div class="card-body p-4">
                    <h2 class="text-center mb-4">@ViewData["Title"]</h2>
                    <form id="account" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                            <label asp-for="Input.Email">Email</label>
                            <span asp-validation-for="Input.Email" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Password" class="form-control" autocomplete="current-password" aria-required="true" placeholder="password" />
                            <label asp-for="Input.Password">Mật khẩu</label>
                            <span asp-validation-for="Input.Password" class="text-danger"></span>
                        </div>
                        <div class="form-check mb-3">
                            <input class="form-check-input" asp-for="Input.RememberMe" />
                            <label class="form-check-label" asp-for="Input.RememberMe">Ghi nhớ đăng nhập</label>
                        </div>
                        <div class="d-grid">
                            <button id="login-submit" type="submit" class="btn btn-primary btn-lg">Đăng nhập</button>
                        </div>
                        <div class="d-flex justify-content-between mt-3">
                            <a id="forgot-password" asp-page="./ForgotPassword">Quên mật khẩu?</a>
                            <a asp-page="./Register" asp-route-returnUrl="@Model.ReturnUrl">Đăng ký tài khoản mới</a>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 3. Trang quên mật khẩu (ForgotPassword)

Trang quên mật khẩu cho phép người dùng khôi phục mật khẩu:

```html
@page
@model ForgotPasswordModel
@{
    ViewData["Title"] = "Quên mật khẩu";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            <div class="card border-0 shadow-sm">
                <div class="card-body p-4">
                    <h2 class="text-center mb-4">@ViewData["Title"]</h2>
                    <h5 class="text-center mb-4">Nhập email của bạn để đặt lại mật khẩu</h5>
                    <form method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                            <label asp-for="Input.Email">Email</label>
                            <span asp-validation-for="Input.Email" class="text-danger"></span>
                        </div>
                        <button type="submit" class="w-100 btn btn-lg btn-primary">Gửi</button>
                    </form>
                    <hr class="my-4">
                    <div class="text-center">
                        <p>Quay lại <a asp-page="./Login">đăng nhập</a></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 4. Trang đặt lại mật khẩu (ResetPassword)

Trang đặt lại mật khẩu cho phép người dùng tạo mật khẩu mới:

```html
@page
@model ResetPasswordModel
@{
    ViewData["Title"] = "Đặt lại mật khẩu";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            <div class="card border-0 shadow-sm">
                <div class="card-body p-4">
                    <h2 class="text-center mb-4">@ViewData["Title"]</h2>
                    <form method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
                        <input asp-for="Input.Code" type="hidden" />
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                            <label asp-for="Input.Email">Email</label>
                            <span asp-validation-for="Input.Email" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                            <label asp-for="Input.Password">Mật khẩu mới</label>
                            <span asp-validation-for="Input.Password" class="text-danger"></span>
                        </div>
                        <div class="form-floating mb-3">
                            <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                            <label asp-for="Input.ConfirmPassword">Xác nhận mật khẩu</label>
                            <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
                        </div>
                        <button type="submit" class="w-100 btn btn-lg btn-primary">Đặt lại mật khẩu</button>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 5. Trang quản lý tài khoản (Manage/Index)

Trang quản lý tài khoản cho phép người dùng cập nhật thông tin cá nhân:

```html
@page
@model IndexModel
@{
    ViewData["Title"] = "Thông tin cá nhân";
    ViewData["ActivePage"] = ManageNavPages.Index;
}

<h3>@ViewData["Title"]</h3>
<partial name="_StatusMessage" for="StatusMessage" />
<div class="row">
    <div class="col-md-8">
        <form id="profile-form" method="post" enctype="multipart/form-data">
            <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
            <div class="row mb-3">
                <div class="col-md-3">
                    <div class="text-center mb-3">
                        @if (!string.IsNullOrEmpty(Model.Input.ProfilePicture))
                        {
                            <img src="@Model.Input.ProfilePicture" alt="Profile Picture" class="rounded-circle img-thumbnail" style="width: 150px; height: 150px; object-fit: cover;" />
                        }
                        else
                        {
                            <img src="~/images/default-avatar.png" alt="Default Avatar" class="rounded-circle img-thumbnail" style="width: 150px; height: 150px; object-fit: cover;" />
                        }
                    </div>
                    <div class="mb-3">
                        <label asp-for="Input.ProfilePictureFile" class="form-label">Ảnh đại diện</label>
                        <input asp-for="Input.ProfilePictureFile" type="file" class="form-control" accept="image/*" />
                        <span asp-validation-for="Input.ProfilePictureFile" class="text-danger"></span>
                    </div>
                </div>
                <div class="col-md-9">
                    <div class="form-floating mb-3">
                        <input asp-for="Username" class="form-control" disabled aria-required="true" />
                        <label asp-for="Username">Email</label>
                    </div>
                    <div class="form-floating mb-3">
                        <input asp-for="Input.PhoneNumber" class="form-control" placeholder="Số điện thoại" />
                        <label asp-for="Input.PhoneNumber">Số điện thoại</label>
                        <span asp-validation-for="Input.PhoneNumber" class="text-danger"></span>
                    </div>
                    <div class="form-floating mb-3">
                        <input asp-for="Input.HoTen" class="form-control" placeholder="Họ tên" />
                        <label asp-for="Input.HoTen">Họ tên</label>
                        <span asp-validation-for="Input.HoTen" class="text-danger"></span>
                    </div>
                    <div class="form-floating mb-3">
                        <select asp-for="Input.GioiTinh" class="form-select">
                            <option value="">-- Chọn giới tính --</option>
                            <option value="Nam">Nam</option>
                            <option value="Nữ">Nữ</option>
                            <option value="Khác">Khác</option>
                        </select>
                        <label asp-for="Input.GioiTinh">Giới tính</label>
                        <span asp-validation-for="Input.GioiTinh" class="text-danger"></span>
                    </div>
                    <div class="form-floating mb-3">
                        <input asp-for="Input.NgaySinh" class="form-control" type="date" />
                        <label asp-for="Input.NgaySinh">Ngày sinh</label>
                        <span asp-validation-for="Input.NgaySinh" class="text-danger"></span>
                    </div>
                    <div class="form-floating mb-3">
                        <input asp-for="Input.DiaChi" class="form-control" placeholder="Địa chỉ" />
                        <label asp-for="Input.DiaChi">Địa chỉ</label>
                        <span asp-validation-for="Input.DiaChi" class="text-danger"></span>
                    </div>
                </div>
            </div>
            <button id="update-profile-button" type="submit" class="btn btn-primary">Lưu thay đổi</button>
        </form>
    </div>
    <div class="col-md-4">
        <div class="card border-0 shadow-sm">
            <div class="card-body">
                <h5 class="card-title">Thông tin tài khoản</h5>
                <p class="card-text">
                    <strong>Email:</strong> @Model.Username<br />
                    <strong>Vai trò:</strong> @Model.UserRole<br />
                    <strong>Ngày tham gia:</strong> @Model.JoinDate.ToString("dd/MM/yyyy")
                </p>
                <a asp-page="./ChangePassword" class="btn btn-outline-primary">Đổi mật khẩu</a>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 6. Trang đổi mật khẩu (Manage/ChangePassword)

Trang đổi mật khẩu cho phép người dùng thay đổi mật khẩu:

```html
@page
@model ChangePasswordModel
@{
    ViewData["Title"] = "Đổi mật khẩu";
    ViewData["ActivePage"] = ManageNavPages.ChangePassword;
}

<h3>@ViewData["Title"]</h3>
<partial name="_StatusMessage" for="StatusMessage" />
<div class="row">
    <div class="col-md-8">
        <form id="change-password-form" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.OldPassword" class="form-control" autocomplete="current-password" aria-required="true" placeholder="Mật khẩu hiện tại" />
                <label asp-for="Input.OldPassword">Mật khẩu hiện tại</label>
                <span asp-validation-for="Input.OldPassword" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.NewPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Mật khẩu mới" />
                <label asp-for="Input.NewPassword">Mật khẩu mới</label>
                <span asp-validation-for="Input.NewPassword" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Xác nhận mật khẩu mới" />
                <label asp-for="Input.ConfirmPassword">Xác nhận mật khẩu mới</label>
                <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
            </div>
            <button type="submit" class="btn btn-primary">Cập nhật mật khẩu</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 7. Partial View cho Navigation trong trang quản lý tài khoản (Manage/_ManageNav)

Partial View cho Navigation trong trang quản lý tài khoản:

```html
@inject SignInManager<ApplicationUser> SignInManager
@{
    var hasExternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).Any();
}

<ul class="nav nav-pills flex-column">
    <li class="nav-item">
        <a class="nav-link @ManageNavPages.IndexNavClass(ViewContext)" id="profile" asp-page="./Index">Thông tin cá nhân</a>
    </li>
    <li class="nav-item">
        <a class="nav-link @ManageNavPages.ChangePasswordNavClass(ViewContext)" id="change-password" asp-page="./ChangePassword">Mật khẩu</a>
    </li>
    @if (hasExternalLogins)
    {
        <li class="nav-item">
            <a class="nav-link @ManageNavPages.ExternalLoginsNavClass(ViewContext)" id="external-logins" asp-page="./ExternalLogins">Đăng nhập bên ngoài</a>
        </li>
    }
    <li class="nav-item">
        <a class="nav-link @ManageNavPages.TwoFactorAuthenticationNavClass(ViewContext)" id="two-factor" asp-page="./TwoFactorAuthentication">Xác thực hai yếu tố</a>
    </li>
    <li class="nav-item">
        <a class="nav-link @ManageNavPages.PersonalDataNavClass(ViewContext)" id="personal-data" asp-page="./PersonalData">Dữ liệu cá nhân</a>
    </li>
</ul>
```

### 8. Layout cho trang quản lý tài khoản (Manage/_Layout)

Layout cho trang quản lý tài khoản:

```html
@{
    Layout = "/Views/Shared/_Layout.cshtml";
}

<div class="container py-5">
    <h1>Quản lý tài khoản</h1>

    <div class="row">
        <div class="col-md-3">
            <partial name="_ManageNav" />
        </div>
        <div class="col-md-9">
            @RenderBody()
        </div>
    </div>
</div>

@section Scripts {
    @RenderSection("Scripts", required: false)
}
```

## CSS cho trang xác thực

Thêm CSS sau vào file `wwwroot/css/site.css`:

```css
/* Styles for authentication pages */
.auth-container {
    max-width: 500px;
    margin: 0 auto;
}

.auth-card {
    border-radius: 10px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.auth-header {
    text-align: center;
    margin-bottom: 2rem;
}

.auth-footer {
    text-align: center;
    margin-top: 1.5rem;
}

.form-floating > .form-control,
.form-floating > .form-select {
    height: calc(3.5rem + 2px);
    line-height: 1.25;
}

.form-floating > label {
    padding: 1rem 0.75rem;
}

.btn-auth {
    padding: 0.75rem 1.5rem;
    font-size: 1.1rem;
}

.profile-picture-container {
    position: relative;
    width: 150px;
    height: 150px;
    margin: 0 auto;
}

.profile-picture {
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 50%;
}

.profile-picture-upload {
    position: absolute;
    bottom: 0;
    right: 0;
    background-color: #fff;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    cursor: pointer;
}
```

## JavaScript cho trang xác thực

Thêm JavaScript sau vào file `wwwroot/js/site.js`:

```javascript
// Validation for registration form
$(document).ready(function () {
    // Password strength meter
    $('#Input_Password').on('keyup', function () {
        var password = $(this).val();
        var strength = 0;
        
        if (password.length > 7) strength += 1;
        if (password.match(/[a-z]+/)) strength += 1;
        if (password.match(/[A-Z]+/)) strength += 1;
        if (password.match(/[0-9]+/)) strength += 1;
        if (password.match(/[$@#&!]+/)) strength += 1;
        
        var strengthBar = $('.password-strength-meter');
        
        switch (strength) {
            case 0:
            case 1:
                strengthBar.css('width', '20%').removeClass().addClass('progress-bar bg-danger').text('Rất yếu');
                break;
            case 2:
                strengthBar.css('width', '40%').removeClass().addClass('progress-bar bg-warning').text('Yếu');
                break;
            case 3:
                strengthBar.css('width', '60%').removeClass().addClass('progress-bar bg-info').text('Trung bình');
                break;
            case 4:
                strengthBar.css('width', '80%').removeClass().addClass('progress-bar bg-primary').text('Mạnh');
                break;
            case 5:
                strengthBar.css('width', '100%').removeClass().addClass('progress-bar bg-success').text('Rất mạnh');
                break;
        }
    });
    
    // Profile picture preview
    $('#Input_ProfilePictureFile').change(function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('.profile-picture').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        }
    });
});
```

## Hướng dẫn triển khai

### 1. Tạo Razor Pages cho Identity

Chạy lệnh sau để tạo các trang Razor cho Identity:

```bash
dotnet aspnet-codegenerator identity -dc DoAnChamSocSucKhoe.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.ForgotPassword;Account.ResetPassword;Account.Manage.Index;Account.Manage.ChangePassword"
```

### 2. Tùy chỉnh trang đăng ký

Cập nhật file `Areas/Identity/Pages/Account/Register.cshtml` với nội dung từ mẫu ở trên.

### 3. Tùy chỉnh trang đăng nhập

Cập nhật file `Areas/Identity/Pages/Account/Login.cshtml` với nội dung từ mẫu ở trên.

### 4. Tùy chỉnh trang quên mật khẩu

Cập nhật file `Areas/Identity/Pages/Account/ForgotPassword.cshtml` với nội dung từ mẫu ở trên.

### 5. Tùy chỉnh trang đặt lại mật khẩu

Cập nhật file `Areas/Identity/Pages/Account/ResetPassword.cshtml` với nội dung từ mẫu ở trên.

### 6. Tùy chỉnh trang quản lý tài khoản

Cập nhật file `Areas/Identity/Pages/Account/Manage/Index.cshtml` với nội dung từ mẫu ở trên.

### 7. Tùy chỉnh trang đổi mật khẩu

Cập nhật file `Areas/Identity/Pages/Account/Manage/ChangePassword.cshtml` với nội dung từ mẫu ở trên.

### 8. Tùy chỉnh layout cho trang quản lý tài khoản

Cập nhật file `Areas/Identity/Pages/Account/Manage/_Layout.cshtml` với nội dung từ mẫu ở trên.

### 9. Tùy chỉnh navigation cho trang quản lý tài khoản

Cập nhật file `Areas/Identity/Pages/Account/Manage/_ManageNav.cshtml` với nội dung từ mẫu ở trên.

## Lưu ý

- Đảm bảo tất cả các trang đều responsive và hiển thị tốt trên các thiết bị khác nhau
- Thêm validation phía client để cải thiện trải nghiệm người dùng
- Sử dụng các thành phần UI của Bootstrap để tạo giao diện nhất quán
- Tùy chỉnh các thông báo lỗi và xác nhận để phù hợp với ngôn ngữ tiếng Việt
- Thêm các tính năng bảo mật như CAPTCHA để tránh tấn công brute force
