using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DoAnChamSocSucKhoe.Data;

namespace DoAnChamSocSucKhoe.Models
{
    public class LichHenViewModel
    {
        public int Id { get; set; }
        public required string BenhNhanId { get; set; }
        public required string BacSiId { get; set; }
        public DateTime ThoiGianHen { get; set; }
        public required string LyDo { get; set; }
        public required string TrangThai { get; set; }
        public required string GhiChu { get; set; }
    }

    public class DoctorDashboardViewModel
    {
        public required List<LichHenViewModel> LichHenList { get; set; }
        public required List<TuVanSucKhoe> TuVanList { get; set; }
        public required List<ApplicationUser> BenhNhanList { get; set; }

        public string GetAppointmentStatusColor(string status)
        {
            switch (status.ToLower())
            {
                case "pending":
                    return "text-warning";
                case "confirmed":
                    return "text-success";
                case "cancelled":
                    return "text-danger";
                case "completed":
                    return "text-info";
                default:
                    return "text-secondary";
            }
        }

        public string GetAppointmentStatusName(string status)
        {
            switch (status.ToLower())
            {
                case "pending":
                    return "Chờ xác nhận";
                case "confirmed":
                    return "Đã xác nhận";
                case "cancelled":
                    return "Đã hủy";
                case "completed":
                    return "Đã hoàn thành";
                default:
                    return "Không xác định";
            }
        }

        public string GetRatingColor(double rating)
        {
            if (rating >= 4.5) return "text-success";
            if (rating >= 3.5) return "text-info";
            if (rating >= 2.5) return "text-warning";
            return "text-danger";
        }

        public string GetRatingText(double rating)
        {
            if (rating >= 4.5) return "Xuất sắc";
            if (rating >= 3.5) return "Tốt";
            if (rating >= 2.5) return "Trung bình";
            return "Cần cải thiện";
        }

        public int GetAppointmentCountByStatus(string status)
        {
            return LichHenList?.Count(x => x.TrangThai.ToLower() == status.ToLower()) ?? 0;
        }

        public string GetTimeRemaining(DateTime appointmentTime)
        {
            var timeRemaining = appointmentTime - DateTime.Now;
            if (timeRemaining.TotalDays > 1)
                return $"{timeRemaining.Days} ngày";
            if (timeRemaining.TotalHours > 1)
                return $"{timeRemaining.Hours} giờ";
            if (timeRemaining.TotalMinutes > 1)
                return $"{timeRemaining.Minutes} phút";
            return "Sắp đến";
        }
    }
} 