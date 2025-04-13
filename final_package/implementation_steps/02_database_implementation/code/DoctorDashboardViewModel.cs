using DoAnChamSocSucKhoe.Models;
using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Models
{
    public class DoctorDashboardViewModel
    {
        // Thống kê tổng quan
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalConsultations { get; set; }
        public int TotalRatings { get; set; }
        public double AverageRating { get; set; }
        
        // Lịch hẹn sắp tới
        public List<LichHen> UpcomingAppointments { get; set; }
        
        // Câu hỏi tư vấn chưa trả lời
        public List<TuVanSucKhoe> PendingConsultations { get; set; }
        
        // Bệnh nhân gần đây
        public List<ApplicationUser> RecentPatients { get; set; }
        
        // Phương thức để lấy màu sắc cho trạng thái lịch hẹn
        public string GetAppointmentStatusColor(string status)
        {
            switch (status)
            {
                case "cho_xac_nhan":
                    return "warning";
                case "da_xac_nhan":
                    return "primary";
                case "dang_dien_ra":
                    return "info";
                case "da_hoan_thanh":
                    return "success";
                case "da_huy":
                    return "danger";
                default:
                    return "secondary";
            }
        }
        
        // Phương thức để lấy tên hiển thị cho trạng thái lịch hẹn
        public string GetAppointmentStatusName(string status)
        {
            switch (status)
            {
                case "cho_xac_nhan":
                    return "Chờ xác nhận";
                case "da_xac_nhan":
                    return "Đã xác nhận";
                case "dang_dien_ra":
                    return "Đang diễn ra";
                case "da_hoan_thanh":
                    return "Đã hoàn thành";
                case "da_huy":
                    return "Đã hủy";
                default:
                    return "Không xác định";
            }
        }
        
        // Phương thức để lấy màu sắc cho đánh giá sao
        public string GetRatingColor()
        {
            if (AverageRating < 2)
                return "danger";
            else if (AverageRating < 3.5)
                return "warning";
            else if (AverageRating < 4.5)
                return "primary";
            else
                return "success";
        }
        
        // Phương thức để lấy tên hiển thị cho đánh giá sao
        public string GetRatingText()
        {
            if (AverageRating < 2)
                return "Kém";
            else if (AverageRating < 3.5)
                return "Trung bình";
            else if (AverageRating < 4.5)
                return "Tốt";
            else
                return "Xuất sắc";
        }
        
        // Phương thức để lấy số lượng lịch hẹn theo trạng thái
        public int GetAppointmentCountByStatus(string status)
        {
            int count = 0;
            foreach (var appointment in UpcomingAppointments)
            {
                if (appointment.TrangThai == status)
                {
                    count++;
                }
            }
            return count;
        }
        
        // Phương thức để lấy thời gian còn lại cho lịch hẹn
        public string GetTimeRemaining(DateTime appointmentTime)
        {
            TimeSpan timeRemaining = appointmentTime - DateTime.Now;
            
            if (timeRemaining.TotalDays > 1)
            {
                return $"{Math.Floor(timeRemaining.TotalDays)} ngày";
            }
            else if (timeRemaining.TotalHours > 1)
            {
                return $"{Math.Floor(timeRemaining.TotalHours)} giờ";
            }
            else if (timeRemaining.TotalMinutes > 1)
            {
                return $"{Math.Floor(timeRemaining.TotalMinutes)} phút";
            }
            else
            {
                return "Sắp diễn ra";
            }
        }
    }
}
