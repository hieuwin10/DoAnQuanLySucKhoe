# Hướng dẫn triển khai giao diện cơ sở dữ liệu

## Tổng quan

Tài liệu này hướng dẫn cách triển khai giao diện người dùng liên quan đến cơ sở dữ liệu cho hệ thống quản lý sức khỏe cá nhân. Mặc dù phần cơ sở dữ liệu chủ yếu là backend, nhưng có một số giao diện người dùng cần thiết để quản lý và hiển thị dữ liệu.

## Giao diện quản lý dữ liệu

### 1. Trang quản lý người dùng (Admin)

Trang này cho phép admin quản lý người dùng trong hệ thống:

- Hiển thị danh sách người dùng với thông tin cơ bản
- Tìm kiếm và lọc người dùng
- Thêm, sửa, xóa người dùng
- Phân quyền cho người dùng

### 2. Trang quản lý vai trò (Admin)

Trang này cho phép admin quản lý vai trò trong hệ thống:

- Hiển thị danh sách vai trò
- Thêm, sửa, xóa vai trò
- Phân quyền cho vai trò

### 3. Trang quản lý hồ sơ sức khỏe (Bác sĩ)

Trang này cho phép bác sĩ xem và quản lý hồ sơ sức khỏe của bệnh nhân:

- Hiển thị thông tin hồ sơ sức khỏe
- Cập nhật thông tin hồ sơ sức khỏe
- Xem lịch sử chỉ số sức khỏe

## Các thành phần UI cần triển khai

### 1. Bảng dữ liệu (Data Tables)

Sử dụng Bootstrap Data Tables để hiển thị dữ liệu từ cơ sở dữ liệu:

```html
<table class="table table-striped table-hover" id="dataTable">
    <thead>
        <tr>
            <th>ID</th>
            <th>Họ tên</th>
            <th>Email</th>
            <th>Vai trò</th>
            <th>Ngày tạo</th>
            <th>Thao tác</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Model.Users)
        {
            <tr>
                <td>@user.Id</td>
                <td>@user.HoTen</td>
                <td>@user.Email</td>
                <td>@user.VaiTro.TenVaiTro</td>
                <td>@user.NgayTao.ToString("dd/MM/yyyy")</td>
                <td>
                    <a href="/Admin/Users/Edit/@user.Id" class="btn btn-sm btn-primary">
                        <i class="fas fa-edit"></i>
                    </a>
                    <a href="/Admin/Users/Delete/@user.Id" class="btn btn-sm btn-danger">
                        <i class="fas fa-trash"></i>
                    </a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

### 2. Form nhập liệu

Sử dụng Bootstrap Forms để tạo form nhập liệu:

```html
<form asp-action="Create" method="post">
    <div class="form-group">
        <label asp-for="HoTen">Họ tên</label>
        <input asp-for="HoTen" class="form-control" />
        <span asp-validation-for="HoTen" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Email">Email</label>
        <input asp-for="Email" class="form-control" />
        <span asp-validation-for="Email" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="VaiTroId">Vai trò</label>
        <select asp-for="VaiTroId" class="form-control" asp-items="ViewBag.VaiTroId"></select>
        <span asp-validation-for="VaiTroId" class="text-danger"></span>
    </div>
    <button type="submit" class="btn btn-primary">Lưu</button>
</form>
```

### 3. Card hiển thị thông tin

Sử dụng Bootstrap Cards để hiển thị thông tin:

```html
<div class="card">
    <div class="card-header">
        <h5 class="card-title">Thông tin hồ sơ sức khỏe</h5>
    </div>
    <div class="card-body">
        <div class="row">
            <div class="col-md-6">
                <p><strong>Chiều cao:</strong> @Model.ChieuCao cm</p>
                <p><strong>Cân nặng:</strong> @Model.CanNang kg</p>
                <p><strong>BMI:</strong> @Model.BMI</p>
            </div>
            <div class="col-md-6">
                <p><strong>Nhịp tim:</strong> @Model.NhipTim bpm</p>
                <p><strong>Huyết áp:</strong> @Model.HuyetApTamThu/@Model.HuyetApTamTruong mmHg</p>
                <p><strong>Đường huyết:</strong> @Model.DuongHuyet mmol/L</p>
            </div>
        </div>
    </div>
    <div class="card-footer">
        <small class="text-muted">Cập nhật lần cuối: @Model.NgayCapNhat.ToString("dd/MM/yyyy HH:mm")</small>
    </div>
</div>
```

## Hướng dẫn triển khai

### 1. Tạo layout chung

Tạo file `Views/Shared/_AdminLayout.cshtml` cho giao diện admin:

```html
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Quản trị hệ thống</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/admin.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
</head>
<body>
    <div class="d-flex" id="wrapper">
        <!-- Sidebar -->
        <div class="bg-light border-right" id="sidebar-wrapper">
            <div class="sidebar-heading">Quản trị hệ thống</div>
            <div class="list-group list-group-flush">
                <a href="/Admin/Dashboard" class="list-group-item list-group-item-action bg-light">
                    <i class="fas fa-tachometer-alt mr-2"></i> Tổng quan
                </a>
                <a href="/Admin/Users" class="list-group-item list-group-item-action bg-light">
                    <i class="fas fa-users mr-2"></i> Quản lý người dùng
                </a>
                <a href="/Admin/Roles" class="list-group-item list-group-item-action bg-light">
                    <i class="fas fa-user-tag mr-2"></i> Quản lý vai trò
                </a>
                <!-- Thêm các mục menu khác -->
            </div>
        </div>
        <!-- Page Content -->
        <div id="page-content-wrapper">
            <nav class="navbar navbar-expand-lg navbar-light bg-light border-bottom">
                <button class="btn btn-primary" id="menu-toggle">
                    <i class="fas fa-bars"></i>
                </button>
                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul class="navbar-nav ml-auto mt-2 mt-lg-0">
                        <li class="nav-item dropdown">
                            <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i class="fas fa-user mr-2"></i> Admin
                            </a>
                            <div class="dropdown-menu dropdown-menu-right" aria-labelledby="navbarDropdown">
                                <a class="dropdown-item" href="/Identity/Account/Manage">Tài khoản</a>
                                <div class="dropdown-divider"></div>
                                <form class="form-inline" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Action("Index", "Home", new { area = "" })">
                                    <button type="submit" class="dropdown-item">Đăng xuất</button>
                                </form>
                            </div>
                        </li>
                    </ul>
                </div>
            </nav>
            <div class="container-fluid p-4">
                @RenderBody()
            </div>
        </div>
    </div>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/admin.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### 2. Tạo CSS cho giao diện admin

Tạo file `wwwroot/css/admin.css`:

```css
#wrapper {
    overflow-x: hidden;
}

#sidebar-wrapper {
    min-height: 100vh;
    margin-left: -15rem;
    transition: margin .25s ease-out;
}

#sidebar-wrapper .sidebar-heading {
    padding: 0.875rem 1.25rem;
    font-size: 1.2rem;
}

#sidebar-wrapper .list-group {
    width: 15rem;
}

#page-content-wrapper {
    min-width: 100vw;
}

#wrapper.toggled #sidebar-wrapper {
    margin-left: 0;
}

@media (min-width: 768px) {
    #sidebar-wrapper {
        margin-left: 0;
    }

    #page-content-wrapper {
        min-width: 0;
        width: 100%;
    }

    #wrapper.toggled #sidebar-wrapper {
        margin-left: -15rem;
    }
}

.list-group-item i {
    margin-right: 10px;
}
```

### 3. Tạo JavaScript cho giao diện admin

Tạo file `wwwroot/js/admin.js`:

```javascript
$(document).ready(function () {
    // Toggle sidebar
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

    // Initialize DataTables
    if ($.fn.DataTable) {
        $('#dataTable').DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json'
            }
        });
    }
});
```

## Tích hợp với backend

Các giao diện người dùng này sẽ tương tác với backend thông qua các controller và service. Xem chi tiết trong [Hướng dẫn triển khai backend](backend.md).

## Lưu ý

- Đảm bảo tất cả các form đều có validation phía client và server
- Sử dụng partial view để tái sử dụng các thành phần UI
- Tối ưu hóa hiệu suất khi hiển thị dữ liệu lớn
- Đảm bảo giao diện responsive trên các thiết bị khác nhau
