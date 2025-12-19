// Admin Dashboard JavaScript
document.addEventListener("DOMContentLoaded", function () {
  // Sidebar toggle
  const sidebarToggle = document.getElementById("sidebarToggle");
  const wrapper = document.getElementById("wrapper");

  if (sidebarToggle) {
    sidebarToggle.addEventListener("click", function () {
      wrapper.classList.toggle("toggled");
    });
  }

  // Initialize tooltips
  const tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Initialize popovers
  const popoverTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="popover"]')
  );
  popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl);
  });

  // Handle active menu items
  const currentPath = window.location.pathname;
  const menuItems = document.querySelectorAll(".list-group-item-action");

  menuItems.forEach((item) => {
    const href = item.getAttribute("href");
    if (currentPath.includes(href)) {
      item.classList.add("active");
    }
  });

  // Handle notifications
  const notificationDropdown = document.getElementById("notificationDropdown");
  if (notificationDropdown) {
    // Add click handler for notifications
    notificationDropdown.addEventListener("click", function () {
      // Here you can add logic to mark notifications as read
      const badge = this.querySelector(".badge");
      if (badge) {
        badge.style.display = "none";
      }
    });
  }

  // Handle user dropdown
  const userDropdown = document.getElementById("userDropdown");
  if (userDropdown) {
    userDropdown.addEventListener("click", function () {
      // Add any user-specific functionality here
    });
  }

  // Handle responsive adjustments
  function handleResponsive() {
    if (window.innerWidth < 768) {
      wrapper.classList.remove("toggled");
    } else {
      wrapper.classList.add("toggled");
    }
  }

  // Initial call
  handleResponsive();

  // Add resize listener
  window.addEventListener("resize", handleResponsive);
});
