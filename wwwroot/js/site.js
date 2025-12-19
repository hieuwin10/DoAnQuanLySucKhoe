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
