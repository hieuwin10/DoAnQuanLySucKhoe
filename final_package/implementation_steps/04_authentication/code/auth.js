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
