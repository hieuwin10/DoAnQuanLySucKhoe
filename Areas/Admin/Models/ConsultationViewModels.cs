using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class ReplyConsultationViewModel
    {
        public int Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string? CurrentReply { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung trả lời là bắt buộc")]
        [StringLength(2000, ErrorMessage = "Nội dung trả lời không được vượt quá 2000 ký tự")]
        [Display(Name = "Nội dung trả lời")]
        public string Reply { get; set; } = string.Empty;
    }

    public class ConsultationViewModel
    {
        public int Id { get; set; }
        public required string TieuDe { get; set; }
        public required string NoiDung { get; set; }
        public string? TraLoi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayTraLoi { get; set; }
        public required string PatientName { get; set; }
        public required string PatientEmail { get; set; }
        public string? DoctorName { get; set; }
        public required string Status { get; set; }
        public bool IsAnswered { get; set; }

        public string StatusClass => Status switch
        {
            "Chờ trả lời" => "warning",
            "Đã trả lời" => "success",
            _ => "secondary"
        };

        public string TimeAgo => GetTimeAgo(NgayTao);
        public string ReplyTimeAgo => NgayTraLoi.HasValue ? GetTimeAgo(NgayTraLoi.Value) : "";

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} ngày trước";

            return dateTime.ToString("dd/MM/yyyy");
        }
    }

    public class ConsultationListViewModel
    {
        public List<ConsultationViewModel> Consultations { get; set; } = new();
        public ConsultationStats Stats { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "all";
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ConsultationStats
    {
        public int TotalConsultations { get; set; }
        public int PendingConsultations { get; set; }
        public int AnsweredConsultations { get; set; }
        public int TodayConsultations { get; set; }

        public double AnsweredPercentage => TotalConsultations > 0 ? (double)AnsweredConsultations / TotalConsultations * 100 : 0;
        public double PendingPercentage => TotalConsultations > 0 ? (double)PendingConsultations / TotalConsultations * 100 : 0;
    }

    public class ConsultationDetailViewModel
    {
        public int Id { get; set; }
        public required string TieuDe { get; set; }
        public required string NoiDung { get; set; }
        public string? TraLoi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayTraLoi { get; set; }
        public required string PatientName { get; set; }
        public required string PatientEmail { get; set; }
        public string? PatientPhone { get; set; }
        public string? DoctorName { get; set; }
        public required string Status { get; set; }
        public bool IsAnswered { get; set; }

        public string StatusClass => Status switch
        {
            "Chờ trả lời" => "warning",
            "Đã trả lời" => "success",
            _ => "secondary"
        };

        public string FormattedNgayTao => NgayTao.ToString("dd/MM/yyyy HH:mm");
        public string FormattedNgayTraLoi => NgayTraLoi?.ToString("dd/MM/yyyy HH:mm") ?? "";
    }
}