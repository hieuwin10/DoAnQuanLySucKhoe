using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class UserDetailViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusClass => Status switch
        {
            "Hoạt động" => "success",
            "Chờ duyệt" => "warning",
            "Không hoạt động" => "danger",
            _ => "secondary"
        };
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastActivity { get; set; }
        public string LastActivityText => LastActivity.HasValue
            ? LastActivity.Value.ToString("dd/MM/yyyy HH:mm")
            : "Chưa có hoạt động";
        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        // Thông tin bệnh nhân được chăm sóc (nếu là người chăm sóc)
        public string? AssignedPatientName { get; set; }
        public string? AssignedPatientId { get; set; }
        public string? AssignedPatientAvatar { get; set; }
        public int? AssignedPatientHealthProfileId { get; set; }

        public bool HasUserPermissions { get; set; }
        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();
        public List<UserActivityLog> ActivityLogs { get; set; } = new List<UserActivityLog>();
    }

    public class UserPermission
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }

    public class UserActivityLog
    {
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string TimeAgo => Timestamp.ToString("dd/MM/yyyy HH:mm");
    }
}