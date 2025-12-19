using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class PatientDetailViewModel
    {
        // Thông tin cơ bản
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }

        // Chỉ số sức khỏe
        public string Height { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public string BMI { get; set; } = string.Empty;
        public string BMIStatus { get; set; } = string.Empty;
        public string BloodPressure { get; set; } = string.Empty;
        public string BloodPressureStatus { get; set; } = string.Empty;
        public string HeartRate { get; set; } = string.Empty;
        public string HeartRateStatus { get; set; } = string.Empty;
        public string GlucoseLevel { get; set; } = string.Empty;
        public string GlucoseLevelStatus { get; set; } = string.Empty;
        public string Cholesterol { get; set; } = string.Empty;
        public string CholesterolStatus { get; set; } = string.Empty;

        // Tiền sử bệnh lý
        public string MedicalHistory { get; set; } = string.Empty;
        public string Allergies { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
        public string FamilyMedicalHistory { get; set; } = string.Empty;
        public string Lifestyle { get; set; } = string.Empty;

        // Thông tin điều trị hiện tại
        public string CurrentDiagnosis { get; set; } = string.Empty;
        public string Treatments { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Lịch sử khám bệnh
        public List<MedicalHistoryItem> MedicalHistoryItems { get; set; } = new List<MedicalHistoryItem>();

        // Lịch hẹn sắp tới
        public List<AppointmentItem> UpcomingAppointments { get; set; } = new List<AppointmentItem>();

        // Lịch sử chỉ số sức khỏe
        public List<HealthMetricItem> HealthMetrics { get; set; } = new List<HealthMetricItem>();

        // Phương thức tiện ích
        public string StatusClass => Status switch
        {
            "Đang điều trị" => "primary",
            "Cần theo dõi" => "warning",
            "Khỏi bệnh" => "success",
            "Chờ khám" => "info",
            _ => "secondary"
        };

        public string StatusBadge => $"<span class='badge bg-{StatusClass}'>{Status}</span>";

        public string LastAppointmentDate => UpcomingAppointments.Count > 0
            ? UpcomingAppointments[0].AppointmentDate.ToString("dd/MM/yyyy HH:mm")
            : "Không có lịch hẹn";
    }

    public class MedicalHistoryItem
    {
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Prescription { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public string AppointmentDateFormatted => AppointmentDate.ToString("dd/MM/yyyy");
    }

    public class AppointmentItem
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;

        public string AppointmentDateFormatted => AppointmentDate.ToString("dd/MM/yyyy HH:mm");

        public string StatusClass => Status switch
        {
            "Đã xác nhận" => "success",
            "Chờ xác nhận" => "warning",
            "Đã hủy" => "danger",
            "Đã hoàn thành" => "info",
            _ => "secondary"
        };
    }

    public class HealthMetricItem
    {
        public string MetricType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty; // normal, warning, danger

        public string StatusClass => Status switch
        {
            "normal" => "success",
            "warning" => "warning",
            "danger" => "danger",
            _ => "secondary"
        };

        public string DateFormatted => Date.ToString("dd/MM/yyyy");
    }
}