document
  .querySelectorAll(".sidebar-dropdown-toggle")
  .forEach(function (toggle) {
    toggle.addEventListener("click", function (e) {
      e.preventDefault();
      this.parentElement.classList.toggle("active");
    });
  });
