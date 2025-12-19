using System;
using System.Collections.Generic;
using DoAnChamSocSucKhoe.Models;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        // Tổng quan
        public int TotalUsers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }

        // Thống kê tăng trưởng
        public decimal UserGrowthRate { get; set; }
        public decimal DoctorGrowthRate { get; set; }
        public decimal PatientGrowthRate { get; set; }
        public decimal AppointmentGrowthRate { get; set; }

        // Lịch hẹn gần đây
        public List<AppointmentViewModel> RecentAppointments { get; set; } = new List<AppointmentViewModel>();

        // Thống kê theo thời gian
        public List<DailyStats> DailyStats { get; set; } = new List<DailyStats>();
        public List<MonthlyStats> MonthlyStats { get; set; } = new List<MonthlyStats>();

        // Thống kê theo trạng thái
        public AppointmentStatusStats AppointmentStatusStats { get; set; } = new AppointmentStatusStats();

        // Thống kê theo chuyên khoa
        public List<SpecialtyStats> SpecialtyStats { get; set; } = new List<SpecialtyStats>();

        // Số lượng tư vấn mới (chờ xử lý)
        public int NewConsultationCount { get; set; }
        // Số lượng lịch hẹn chờ xác nhận
        public int PendingAppointmentCount { get; set; }
    }

    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string DoctorName { get; set; }
        public required string Specialty { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public required string AppointmentStatus { get; set; }
        public string? Status { get; set; } // Cho phép gán trực tiếp, nullable
        public string StatusClass => (Status ?? AppointmentStatus) switch
        {
            "Confirmed" => "success",
            "Pending" => "warning",
            "Cancelled" => "danger",
            _ => "secondary"
        };
    }

    public class DailyStats
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int NewAppointments { get; set; }
        public int CompletedAppointments { get; set; }
    }

    public class MonthlyStats
    {
        public required string Month { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAppointments { get; set; }
        public decimal Revenue { get; set; }
    }

    public class AppointmentStatusStats
    {
        public int Total { get; set; }
        public int Confirmed { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
        public int Completed { get; set; }
    }

    public class SpecialtyStats
    {
        public required string SpecialtyName { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public decimal Revenue { get; set; }
    }
}