// Password strength meter
function checkPasswordStrength(password) {
  let strength = 0;

  // Length check
  if (password.length >= 8) strength++;

  // Contains number
  if (/\d/.test(password)) strength++;

  // Contains lowercase
  if (/[a-z]/.test(password)) strength++;

  // Contains uppercase
  if (/[A-Z]/.test(password)) strength++;

  // Contains special character
  if (/[^A-Za-z0-9]/.test(password)) strength++;

  return strength;
}

// Update password strength meter
function updatePasswordStrengthMeter(password) {
  const strength = checkPasswordStrength(password);
  const meter = document.querySelector(".password-strength-meter div");

  if (!meter) return;

  meter.className = "";
  if (strength === 0) meter.className = "";
  else if (strength <= 2) meter.className = "password-strength-weak";
  else if (strength <= 3) meter.className = "password-strength-fair";
  else if (strength <= 4) meter.className = "password-strength-good";
  else meter.className = "password-strength-strong";
}

// Profile picture preview
function previewProfilePicture(input) {
  if (input.files && input.files[0]) {
    const reader = new FileReader();

    reader.onload = function (e) {
      const preview = document.querySelector("#profile-picture-preview");
      if (preview) {
        preview.src = e.target.result;
      }
    };

    reader.readAsDataURL(input.files[0]);
  }
}

// Form validation
document.addEventListener("DOMContentLoaded", function () {
  // Password strength meter
  const passwordInput = document.querySelector('input[type="password"]');
  if (passwordInput) {
    passwordInput.addEventListener("input", function () {
      updatePasswordStrengthMeter(this.value);
    });
  }

  // Profile picture preview
  const profilePictureInput = document.querySelector('input[type="file"]');
  if (profilePictureInput) {
    profilePictureInput.addEventListener("change", function () {
      previewProfilePicture(this);
    });
  }

  // Form validation
  const forms = document.querySelectorAll(".needs-validation");
  Array.from(forms).forEach((form) => {
    form.addEventListener(
      "submit",
      (event) => {
        if (!form.checkValidity()) {
          event.preventDefault();
          event.stopPropagation();
        }
        form.classList.add("was-validated");
      },
      false
    );
  });
});
