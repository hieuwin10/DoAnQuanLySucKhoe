using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Patient.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại lịch hẹn")]
        public required string Type { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public required string Specialty { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày hẹn")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian")]
        public TimeSpan AppointmentTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn địa điểm")]
        public required string Location { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do khám")]
        public required string Reason { get; set; }

        public bool UseInsurance { get; set; }
        public bool WantReminder { get; set; }

        [Required]
        public required string Status { get; set; } = "Pending"; // Default value

        public required string Notes { get; set; } = ""; // Default empty string

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default to current time
        public DateTime? UpdatedAt { get; set; }
    }
}