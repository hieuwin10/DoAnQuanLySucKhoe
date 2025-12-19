using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class AppointmentDetailViewModel
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string PatientEmail { get; set; }
        public required string PatientPhone { get; set; }
        public required string DoctorName { get; set; }
        public required string DoctorEmail { get; set; }
        public DateTime DateTime { get; set; }
        public required string Location { get; set; }
        public required string Reason { get; set; }
        public required string Status { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        public required string StatusClass { get; set; }
    }

    public class CreateAppointmentViewModel
    {
        [Display(Name = "Bệnh nhân")]
        public string PatientId { get; set; } = "";

        [Display(Name = "Bác sĩ")]
        public string DoctorId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn ngày giờ")]
        [Display(Name = "Ngày giờ hẹn")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa điểm")]
        [Display(Name = "Địa điểm")]
        [StringLength(200, ErrorMessage = "Địa điểm không được vượt quá 200 ký tự")]
        public string Location { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập lý do")]
        [Display(Name = "Lý do")]
        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string Reason { get; set; } = "";

        [Display(Name = "Chẩn đoán")]
        [StringLength(1000, ErrorMessage = "Chẩn đoán không được vượt quá 1000 ký tự")]
        public string? Diagnosis { get; set; }

        [Display(Name = "Đơn thuốc")]
        [StringLength(1000, ErrorMessage = "Đơn thuốc không được vượt quá 1000 ký tự")]
        public string? Prescription { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        public List<SelectListItem> AvailableDoctors { get; set; } = new();
        public List<SelectListItem> AvailablePatients { get; set; } = new();
    }

    public class EditAppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        [Display(Name = "Bệnh nhân")]
        public string PatientId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        [Display(Name = "Bác sĩ")]
        public string DoctorId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn ngày giờ")]
        [Display(Name = "Ngày giờ hẹn")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa điểm")]
        [Display(Name = "Địa điểm")]
        [StringLength(200, ErrorMessage = "Địa điểm không được vượt quá 200 ký tự")]
        public string Location { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập lý do")]
        [Display(Name = "Lý do")]
        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string Reason { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "";

        [Display(Name = "Chẩn đoán")]
        [StringLength(1000, ErrorMessage = "Chẩn đoán không được vượt quá 1000 ký tự")]
        public string? Diagnosis { get; set; }

        [Display(Name = "Đơn thuốc")]
        [StringLength(1000, ErrorMessage = "Đơn thuốc không được vượt quá 1000 ký tự")]
        public string? Prescription { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        public List<SelectListItem> AvailableDoctors { get; set; } = new();
        public List<SelectListItem> AvailablePatients { get; set; } = new();

        public List<SelectListItem> StatusOptions => new List<SelectListItem>
        {
            new SelectListItem { Value = "Chờ xác nhận", Text = "Chờ xác nhận" },
            new SelectListItem { Value = "Đã xác nhận", Text = "Đã xác nhận" },
            new SelectListItem { Value = "Hoàn thành", Text = "Hoàn thành" },
            new SelectListItem { Value = "Đã hủy", Text = "Đã hủy" }
        };
    }

    public class AppointmentListItemViewModel
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string PatientAvatar { get; set; }
        public required string DoctorName { get; set; }
        public DateTime DateTime { get; set; }
        public required string Location { get; set; }
        public required string Reason { get; set; }
        public required string Status { get; set; }
        public required string StatusClass { get; set; }
    }

    public class AppointmentListViewModel
    {
        public List<AppointmentListItemViewModel> Appointments { get; set; } = new();
        public AppointmentStats Stats { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public DateTime? DateFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class AppointmentStats
    {
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public double GrowthRate { get; set; }
        public int CanceledAppointments { get; set; }
        public double UpcomingGrowthRate { get; set; }
        public double CompletedGrowthRate { get; set; }
        public double CanceledGrowthRate { get; set; }
    }
}