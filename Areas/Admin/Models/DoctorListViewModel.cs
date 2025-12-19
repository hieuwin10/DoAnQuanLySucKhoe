using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class DoctorListViewModel
    {
        public List<DoctorViewModel> Doctors { get; set; } = new();
        public DoctorStats Stats { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "all";
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class DoctorViewModel
    {
        public string Id { get; set; } = string.Empty;
        public required string FullName { get; set; }
        public string? Avatar { get; set; }
        public required string Specialty { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Status { get; set; }
        public required string StatusClass { get; set; }
    }

    public class DoctorStats
    {
        public int TotalDoctors { get; set; }
        public int NewDoctors { get; set; }
        public double NewDoctorsGrowthRate { get; set; }
        public int ActiveDoctors { get; set; }
        public double ActiveDoctorsGrowthRate { get; set; }
        public int InactiveDoctors { get; set; }
        public double InactiveDoctorsGrowthRate { get; set; }
        public int NewDoctorsThisMonth { get; set; }
    }
}
