# Hướng dẫn triển khai giao diện trang chủ (Landing Page)

## Tổng quan

Tài liệu này hướng dẫn cách triển khai giao diện người dùng cho trang chủ (Landing Page) của hệ thống quản lý sức khỏe cá nhân. Trang chủ được thiết kế với Bootstrap và HTML5 để tạo giao diện hấp dẫn và responsive.

## Các thành phần UI cần triển khai

### 1. Navbar

Thanh điều hướng (Navbar) sẽ hiển thị ở đầu trang với logo, menu và nút đăng nhập/đăng ký:

```html
<nav class="navbar navbar-expand-lg navbar-light bg-white fixed-top shadow-sm">
    <div class="container">
        <a class="navbar-brand" href="/">
            <img src="~/images/logo.png" alt="Health Manager Logo" height="40">
            <span class="ms-2 fw-bold text-primary">Health Manager</span>
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav me-auto">
                <li class="nav-item">
                    <a class="nav-link" href="#features">Tính năng</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#about">Giới thiệu</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#testimonials">Đánh giá</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="/Home/Contact">Liên hệ</a>
                </li>
            </ul>
            <partial name="_LoginPartial" />
        </div>
    </div>
</nav>
```

### 2. Hero Section

Phần giới thiệu chính (Hero Section) sẽ hiển thị ngay dưới Navbar với tiêu đề, mô tả và nút kêu gọi hành động:

```html
<section id="hero" class="py-5 mt-5">
    <div class="container py-5">
        <div class="row align-items-center">
            <div class="col-lg-6">
                <h1 class="display-4 fw-bold text-primary mb-3">Quản lý sức khỏe cá nhân thông minh</h1>
                <p class="lead mb-4">Hệ thống quản lý sức khỏe cá nhân giúp bạn theo dõi các chỉ số sức khỏe, nhận tư vấn từ bác sĩ, và cải thiện sức khỏe mỗi ngày.</p>
                <div class="d-grid gap-2 d-md-flex justify-content-md-start">
                    <a href="/Identity/Account/Register" class="btn btn-primary btn-lg px-4 me-md-2">Đăng ký ngay</a>
                    <a href="#features" class="btn btn-outline-secondary btn-lg px-4">Tìm hiểu thêm</a>
                </div>
            </div>
            <div class="col-lg-6">
                <img src="~/images/hero-image.svg" alt="Health Management" class="img-fluid">
            </div>
        </div>
    </div>
</section>
```

### 3. Features Section

Phần giới thiệu tính năng (Features Section) sẽ hiển thị các tính năng chính của hệ thống:

```html
<section id="features" class="py-5 bg-light">
    <div class="container py-5">
        <div class="text-center mb-5">
            <h2 class="fw-bold">Tính năng nổi bật</h2>
            <p class="lead">Khám phá các tính năng giúp bạn quản lý sức khỏe hiệu quả</p>
        </div>
        <div class="row g-4">
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="feature-icon bg-primary bg-gradient text-white rounded-circle mb-3">
                            <i class="fas fa-heartbeat"></i>
                        </div>
                        <h5 class="card-title">Theo dõi chỉ số sức khỏe</h5>
                        <p class="card-text">Theo dõi nhịp tim, huyết áp, đường huyết và các chỉ số sức khỏe khác theo thời gian.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="feature-icon bg-primary bg-gradient text-white rounded-circle mb-3">
                            <i class="fas fa-user-md"></i>
                        </div>
                        <h5 class="card-title">Tư vấn từ bác sĩ</h5>
                        <p class="card-text">Nhận tư vấn sức khỏe từ các bác sĩ chuyên nghiệp thông qua hệ thống trực tuyến.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="feature-icon bg-primary bg-gradient text-white rounded-circle mb-3">
                            <i class="fas fa-chart-line"></i>
                        </div>
                        <h5 class="card-title">Biểu đồ theo dõi</h5>
                        <p class="card-text">Xem biểu đồ trực quan về các chỉ số sức khỏe để dễ dàng theo dõi tiến triển.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="feature-icon bg-primary bg-gradient text-white rounded-circle mb-3">
                            <i class="fas fa-calendar-alt"></i>
                        </div>
                        <h5 class="card-title">Đặt lịch hẹn</h5>
                        <p class="card-text">Đặt lịch hẹn với bác sĩ trực tuyến và nhận thông báo nhắc nhở.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="feature-icon bg-primary bg-gradient text-white rounded-circle mb-3">
                            <i class="fas fa-utensils"></i>
                        </div>
                        <h5 class="card-title">Kế hoạch dinh dưỡng</h5>
                        <p class="card-text">Nhận kế hoạch dinh dưỡng phù hợp với tình trạng sức khỏe của bạn.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="feature-icon bg-primary bg-gradient text-white rounded-circle mb-3">
                            <i class="fas fa-running"></i>
                        </div>
                        <h5 class="card-title">Kế hoạch tập luyện</h5>
                        <p class="card-text">Nhận kế hoạch tập luyện phù hợp để cải thiện sức khỏe và thể chất.</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
```

### 4. About Section

Phần giới thiệu về hệ thống (About Section):

```html
<section id="about" class="py-5">
    <div class="container py-5">
        <div class="row align-items-center">
            <div class="col-lg-6">
                <img src="~/images/about-image.svg" alt="About Health Manager" class="img-fluid">
            </div>
            <div class="col-lg-6">
                <h2 class="fw-bold mb-3">Về Health Manager</h2>
                <p class="lead mb-4">Health Manager là hệ thống quản lý sức khỏe cá nhân toàn diện, kết nối bệnh nhân với bác sĩ và cung cấp các công cụ để theo dõi và cải thiện sức khỏe.</p>
                <p class="mb-4">Được phát triển bởi đội ngũ chuyên gia y tế và công nghệ, Health Manager cam kết mang đến trải nghiệm tốt nhất cho người dùng với giao diện thân thiện và các tính năng hữu ích.</p>
                <div class="row g-4 mb-4">
                    <div class="col-md-6">
                        <div class="d-flex">
                            <div class="flex-shrink-0">
                                <i class="fas fa-check-circle text-primary fs-4"></i>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <h5 class="fw-bold">Bảo mật dữ liệu</h5>
                                <p>Dữ liệu sức khỏe của bạn được bảo mật tuyệt đối.</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="d-flex">
                            <div class="flex-shrink-0">
                                <i class="fas fa-check-circle text-primary fs-4"></i>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <h5 class="fw-bold">Đội ngũ chuyên nghiệp</h5>
                                <p>Đội ngũ bác sĩ chuyên nghiệp và tận tâm.</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="d-flex">
                            <div class="flex-shrink-0">
                                <i class="fas fa-check-circle text-primary fs-4"></i>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <h5 class="fw-bold">Cập nhật liên tục</h5>
                                <p>Hệ thống được cập nhật thường xuyên với tính năng mới.</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="d-flex">
                            <div class="flex-shrink-0">
                                <i class="fas fa-check-circle text-primary fs-4"></i>
                            </div>
                            <div class="flex-grow-1 ms-3">
                                <h5 class="fw-bold">Hỗ trợ 24/7</h5>
                                <p>Đội ngũ hỗ trợ luôn sẵn sàng giúp đỡ bạn.</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
```

### 5. Testimonials Section

Phần đánh giá từ người dùng (Testimonials Section):

```html
<section id="testimonials" class="py-5 bg-light">
    <div class="container py-5">
        <div class="text-center mb-5">
            <h2 class="fw-bold">Người dùng nói gì về chúng tôi</h2>
            <p class="lead">Đánh giá từ những người đã sử dụng Health Manager</p>
        </div>
        <div class="row">
            <div class="col-lg-4 mb-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body p-4">
                        <div class="d-flex mb-3">
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                        </div>
                        <p class="card-text mb-4">"Health Manager đã giúp tôi theo dõi sức khỏe một cách dễ dàng. Tôi đặc biệt thích tính năng biểu đồ theo dõi chỉ số sức khỏe theo thời gian."</p>
                        <div class="d-flex align-items-center">
                            <img src="~/images/testimonial-1.jpg" alt="Nguyễn Văn A" class="rounded-circle me-3" width="50" height="50">
                            <div>
                                <h6 class="fw-bold mb-0">Nguyễn Văn A</h6>
                                <small class="text-muted">Bệnh nhân</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 mb-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body p-4">
                        <div class="d-flex mb-3">
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                        </div>
                        <p class="card-text mb-4">"Là một bác sĩ, tôi thấy Health Manager rất hữu ích trong việc theo dõi sức khỏe của bệnh nhân. Giao diện dễ sử dụng và đầy đủ tính năng."</p>
                        <div class="d-flex align-items-center">
                            <img src="~/images/testimonial-2.jpg" alt="Trần Thị B" class="rounded-circle me-3" width="50" height="50">
                            <div>
                                <h6 class="fw-bold mb-0">Trần Thị B</h6>
                                <small class="text-muted">Bác sĩ</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 mb-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body p-4">
                        <div class="d-flex mb-3">
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star text-warning"></i>
                            <i class="fas fa-star-half-alt text-warning"></i>
                        </div>
                        <p class="card-text mb-4">"Tôi đã sử dụng Health Manager để theo dõi huyết áp và đường huyết. Tính năng nhắc nhở uống thuốc rất hữu ích cho người già như tôi."</p>
                        <div class="d-flex align-items-center">
                            <img src="~/images/testimonial-3.jpg" alt="Lê Văn C" class="rounded-circle me-3" width="50" height="50">
                            <div>
                                <h6 class="fw-bold mb-0">Lê Văn C</h6>
                                <small class="text-muted">Bệnh nhân</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
```

### 6. CTA Section

Phần kêu gọi hành động (CTA Section):

```html
<section id="cta" class="py-5 bg-primary text-white">
    <div class="container py-5 text-center">
        <h2 class="fw-bold mb-3">Bắt đầu quản lý sức khỏe của bạn ngay hôm nay</h2>
        <p class="lead mb-4">Đăng ký miễn phí và trải nghiệm các tính năng tuyệt vời của Health Manager</p>
        <a href="/Identity/Account/Register" class="btn btn-light btn-lg px-4">Đăng ký ngay</a>
    </div>
</section>
```

### 7. Footer

Phần chân trang (Footer):

```html
<footer class="py-5 bg-dark text-white">
    <div class="container">
        <div class="row">
            <div class="col-lg-4 mb-4 mb-lg-0">
                <img src="~/images/logo-white.png" alt="Health Manager Logo" height="40">
                <p class="mt-3">Health Manager - Hệ thống quản lý sức khỏe cá nhân toàn diện, kết nối bệnh nhân với bác sĩ và cung cấp các công cụ để theo dõi và cải thiện sức khỏe.</p>
            </div>
            <div class="col-lg-2 mb-4 mb-lg-0">
                <h5 class="mb-3">Liên kết</h5>
                <ul class="list-unstyled">
                    <li class="mb-2"><a href="/" class="text-white text-decoration-none">Trang chủ</a></li>
                    <li class="mb-2"><a href="#features" class="text-white text-decoration-none">Tính năng</a></li>
                    <li class="mb-2"><a href="#about" class="text-white text-decoration-none">Giới thiệu</a></li>
                    <li class="mb-2"><a href="#testimonials" class="text-white text-decoration-none">Đánh giá</a></li>
                </ul>
            </div>
            <div class="col-lg-2 mb-4 mb-lg-0">
                <h5 class="mb-3">Hỗ trợ</h5>
                <ul class="list-unstyled">
                    <li class="mb-2"><a href="/Home/Contact" class="text-white text-decoration-none">Liên hệ</a></li>
                    <li class="mb-2"><a href="/Home/Privacy" class="text-white text-decoration-none">Chính sách</a></li>
                    <li class="mb-2"><a href="/Home/FAQ" class="text-white text-decoration-none">FAQ</a></li>
                </ul>
            </div>
            <div class="col-lg-4">
                <h5 class="mb-3">Liên hệ</h5>
                <ul class="list-unstyled">
                    <li class="mb-2"><i class="fas fa-map-marker-alt me-2"></i> 123 Đường ABC, Quận XYZ, TP. HCM</li>
                    <li class="mb-2"><i class="fas fa-phone me-2"></i> (028) 1234 5678</li>
                    <li class="mb-2"><i class="fas fa-envelope me-2"></i> info@healthmanager.com</li>
                </ul>
                <div class="d-flex mt-3">
                    <a href="#" class="text-white me-3"><i class="fab fa-facebook-f"></i></a>
                    <a href="#" class="text-white me-3"><i class="fab fa-twitter"></i></a>
                    <a href="#" class="text-white me-3"><i class="fab fa-instagram"></i></a>
                    <a href="#" class="text-white"><i class="fab fa-linkedin-in"></i></a>
                </div>
            </div>
        </div>
        <hr class="my-4">
        <div class="text-center">
            <p class="mb-0">&copy; @DateTime.Now.Year Health Manager. All rights reserved.</p>
        </div>
    </div>
</footer>
```

## CSS cho trang chủ

Tạo file `wwwroot/css/site.css` với nội dung sau:

```css
:root {
    --primary-color: #0d6efd;
    --secondary-color: #6c757d;
    --success-color: #198754;
    --info-color: #0dcaf0;
    --warning-color: #ffc107;
    --danger-color: #dc3545;
    --light-color: #f8f9fa;
    --dark-color: #212529;
}

body {
    font-family: 'Roboto', sans-serif;
    padding-top: 56px;
}

.feature-icon {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 4rem;
    height: 4rem;
    font-size: 2rem;
}

.btn-primary {
    background-color: var(--primary-color);
    border-color: var(--primary-color);
}

.btn-primary:hover {
    background-color: #0b5ed7;
    border-color: #0a58ca;
}

.text-primary {
    color: var(--primary-color) !important;
}

.bg-primary {
    background-color: var(--primary-color) !important;
}

#hero {
    background-color: var(--light-color);
}

#cta {
    background-color: var(--primary-color);
}

.navbar {
    transition: all 0.3s ease;
}

.navbar.scrolled {
    background-color: #fff !important;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

@media (max-width: 991.98px) {
    .navbar-collapse {
        background-color: #fff;
        padding: 1rem;
        border-radius: 0.25rem;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    }
}
```

## JavaScript cho trang chủ

Tạo file `wwwroot/js/site.js` với nội dung sau:

```javascript
// Thêm class scrolled cho navbar khi cuộn trang
$(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
        $('.navbar').addClass('scrolled');
    } else {
        $('.navbar').removeClass('scrolled');
    }
});

// Smooth scroll cho các liên kết trong trang
$(document).ready(function () {
    $('a[href^="#"]').on('click', function (e) {
        e.preventDefault();
        var target = this.hash;
        var $target = $(target);
        $('html, body').animate({
            'scrollTop': $target.offset().top - 70
        }, 1000, 'swing');
    });
});

// Hiệu ứng hiển thị các phần tử khi cuộn trang
$(document).ready(function () {
    AOS.init({
        duration: 1000,
        once: true
    });
});
```

## Hướng dẫn triển khai

### 1. Tạo layout chung

Tạo file `Views/Shared/_Layout.cshtml` với nội dung từ file code mẫu `_Layout.cshtml`.

### 2. Tạo trang chủ

Tạo file `Views/Home/Index.cshtml` với nội dung từ file code mẫu `Index.cshtml`.

### 3. Tạo partial view cho đăng nhập/đăng ký

Tạo file `Views/Shared/_LoginPartial.cshtml` với nội dung từ file code mẫu `_LoginPartial.cshtml`.

### 4. Tạo thư mục và tệp hình ảnh

Tạo thư mục `wwwroot/images` và thêm các hình ảnh cần thiết:

- logo.png: Logo của hệ thống
- logo-white.png: Logo trắng cho footer
- hero-image.svg: Hình ảnh minh họa cho hero section
- about-image.svg: Hình ảnh minh họa cho about section
- testimonial-1.jpg, testimonial-2.jpg, testimonial-3.jpg: Hình ảnh người dùng đánh giá

### 5. Thêm các thư viện bên thứ ba

Thêm các thư viện bên thứ ba vào file `_Layout.cshtml`:

```html
<!-- Font Awesome -->
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
<!-- Google Fonts -->
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" />
<!-- AOS (Animate On Scroll) -->
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/aos/2.3.4/aos.css" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/aos/2.3.4/aos.js"></script>
```

## Lưu ý

- Đảm bảo tất cả các đường dẫn tĩnh sử dụng tag helper `~/` để tránh lỗi đường dẫn
- Kiểm tra tính responsive của trang trên các thiết bị khác nhau
- Tối ưu hóa hình ảnh để trang tải nhanh hơn
- Thêm meta tags cho SEO
- Đảm bảo tất cả các liên kết đều hoạt động chính xác
