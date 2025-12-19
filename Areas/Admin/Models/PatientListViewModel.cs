using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class PatientListViewModel
    {
        // Các tham số tìm kiếm và lọc
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "all";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        // Danh sách bệnh nhân hiển thị trong trang
        public List<PatientViewModel> Patients { get; set; } = new List<PatientViewModel>();

        // Thống kê bệnh nhân
        public PatientStats Stats { get; set; } = new PatientStats();
    }

    public class PatientViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public string StatusClass => Status switch
        {
            "Đang điều trị" => "primary",
            "Cần theo dõi" => "warning",
            "Khỏi bệnh" => "success",
            "Chờ khám" => "info",
            _ => "secondary"
        };
    }

    public class PatientStats
    {
        public int TotalPatients { get; set; }
        public int NewPatients { get; set; }
        public int UnderTreatmentPatients { get; set; }
        public int MonitoringPatients { get; set; }
        public decimal NewPatientsGrowthRate { get; set; }
        public decimal UnderTreatmentGrowthRate { get; set; }
        public decimal MonitoringGrowthRate { get; set; }

        // Dữ liệu cho biểu đồ phân bố theo độ tuổi
        public List<AgeGroupData> AgeDistribution { get; set; } = new List<AgeGroupData>();

        // Dữ liệu cho biểu đồ phân bố bệnh lý
        public List<DiagnosisData> DiagnosisDistribution { get; set; } = new List<DiagnosisData>();
    }

    public class AgeGroupData
    {
        public string AgeGroup { get; set; } = string.Empty;
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
    }

    public class DiagnosisData
    {
        public string DiagnosisName { get; set; } = string.Empty;
        public int PatientCount { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}