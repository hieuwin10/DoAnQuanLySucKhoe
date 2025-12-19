$(document).ready(function () {
  // Toggle sidebar
  $("#sidebarCollapse").on("click", function () {
    $("#sidebar").toggleClass("active");
  });

  // Initialize tooltips
  var tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Initialize popovers
  var popoverTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="popover"]')
  );
  var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl);
  });

  // Handle patient search
  $("#patientSearch").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $(".patient-card").filter(function () {
      $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });
  });

  // Handle consultation filter
  $(".consultation-filter").on("change", function () {
    var filter = $(this).val();
    if (filter === "all") {
      $(".consultation-item").show();
    } else {
      $(".consultation-item").hide();
      $(".consultation-item." + filter).show();
    }
  });

  // Handle appointment calendar
  $(".appointment-slot").on("click", function () {
    if ($(this).hasClass("available")) {
      $(this).removeClass("available").addClass("booked");
      // Show appointment form modal
      $("#appointmentModal").modal("show");
    }
  });

  // Handle nutrition plan form
  $("#nutritionPlanForm").on("submit", function (e) {
    e.preventDefault();
    // Add form submission logic here
  });

  // Handle exercise plan form
  $("#exercisePlanForm").on("submit", function (e) {
    e.preventDefault();
    // Add form submission logic here
  });

  // Handle message form
  $("#messageForm").on("submit", function (e) {
    e.preventDefault();
    // Add form submission logic here
  });

  // Handle profile form
  $("#profileForm").on("submit", function (e) {
    e.preventDefault();
    // Add form submission logic here
  });

  // Initialize charts
  if ($("#patientChart").length) {
    var ctx = document.getElementById("patientChart").getContext("2d");
    var patientChart = new Chart(ctx, {
      type: "line",
      data: {
        labels: [
          "Tháng 1",
          "Tháng 2",
          "Tháng 3",
          "Tháng 4",
          "Tháng 5",
          "Tháng 6",
        ],
        datasets: [
          {
            label: "Số bệnh nhân",
            data: [12, 19, 3, 5, 2, 3],
            borderColor: "rgb(75, 192, 192)",
            tension: 0.1,
          },
        ],
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
  }

  // Initialize calendar
  if ($("#appointmentCalendar").length) {
    var calendar = new FullCalendar.Calendar(
      document.getElementById("appointmentCalendar"),
      {
        initialView: "dayGridMonth",
        headerToolbar: {
          left: "prev,next today",
          center: "title",
          right: "dayGridMonth,timeGridWeek,timeGridDay",
        },
        events: [
          {
            title: "Khám bệnh",
            start: "2024-04-18T10:00:00",
            end: "2024-04-18T11:00:00",
          },
          {
            title: "Tư vấn online",
            start: "2024-04-19T14:00:00",
            end: "2024-04-19T15:00:00",
          },
        ],
      }
    );
    calendar.render();
  }
});
