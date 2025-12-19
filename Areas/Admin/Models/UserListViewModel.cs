using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class UserListViewModel
    {
        public UserStats Stats { get; set; } = new UserStats();
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "all";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
    }

    public class UserStats
    {
        public int TotalUsers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int InactiveUsers { get; set; }
        public decimal TotalGrowthRate { get; set; }
        public decimal DoctorGrowthRate { get; set; }
        public decimal PatientGrowthRate { get; set; }
        public decimal InactiveGrowthRate { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastActivity { get; set; }
        public string Avatar { get; set; } = string.Empty;

        public string StatusClass => Status switch
        {
            "Hoạt động" => "success",
            "Chờ duyệt" => "warning",
            "Không hoạt động" => "danger",
            _ => "secondary"
        };

        public string LastActivityText => LastActivity.HasValue
            ? LastActivity.Value.ToString("dd/MM/yyyy HH:mm")
            : "Chưa có hoạt động";
    }
}