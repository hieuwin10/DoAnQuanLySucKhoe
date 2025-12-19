using DoAnChamSocSucKhoe.Models;
using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Patient.Models
{
    //Các nhắc nhở được truyền vào
    public class PatientDashboardViewModel
    {
        // Hồ sơ sức khỏe
        public HoSoSucKhoe? HealthProfile { get; set; }
        
        // Chỉ số BMI
        public double? BMI { get; set; }
        
        // Lịch sử chỉ số sức khỏe
        public List<ChiSoSucKhoe> HealthMetrics { get; set; } = new List<ChiSoSucKhoe>();
        
        // Lịch hẹn sắp tới
        public List<LichHen> UpcomingAppointments { get; set; } = new List<LichHen>();
        
        // Tư vấn gần đây
        public List<TuVanSucKhoe> RecentConsultations { get; set; } = new List<TuVanSucKhoe>();
        
        // Kế hoạch dinh dưỡng
        public List<KeHoachDinhDuong> NutritionPlans { get; set; } = new List<KeHoachDinhDuong>();
        
        // Kế hoạch tập luyện
        public List<KeHoachTapLuyen> ExercisePlans { get; set; } = new List<KeHoachTapLuyen>();
        
        // Danh sách nhắc nhở
        public List<NhacNhoSucKhoe> Reminders { get; set; } = new List<NhacNhoSucKhoe>();
        
        // Phương thức để lấy trạng thái BMI
        public string GetBMIStatus()
        {
            if (!BMI.HasValue)
                return "Chưa có dữ liệu";
                
            if (BMI.Value < 18.5)
                return "Thiếu cân";
            else if (BMI.Value < 25)
                return "Bình thường";
            else if (BMI.Value < 30)
                return "Thừa cân";
            else
                return "Béo phì";
        }
        
        // Phương thức để lấy màu sắc cho BMI
        public string GetBMIColor()
        {
            if (!BMI.HasValue)
                return "text-secondary";
                
            if (BMI.Value < 18.5)
                return "text-warning";
            else if (BMI.Value < 25)
                return "text-success";
            else if (BMI.Value < 30)
                return "text-warning";
            else
                return "text-danger";
        }
        
        // Phương thức để lấy trạng thái nhịp tim
        public string GetHeartRateStatus()
        {
            if (HealthProfile == null)
                return "Chưa có dữ liệu";
                
            int heartRate = HealthProfile.NhipTim;
            
            if (heartRate < 60)
                return "Thấp";
            else if (heartRate <= 100)
                return "Bình thường";
            else
                return "Cao";
        }
        
        // Phương thức để lấy màu sắc cho nhịp tim
        public string GetHeartRateColor()
        {
            if (HealthProfile == null)
                return "text-secondary";
                
            int heartRate = HealthProfile.NhipTim;
            
            if (heartRate < 60)
                return "text-warning";
            else if (heartRate <= 100)
                return "text-success";
            else
                return "text-danger";
        }
        
        // Phương thức để lấy trạng thái huyết áp
        public string GetBloodPressureStatus()
        {
            if (HealthProfile == null)
                return "Chưa có dữ liệu";
                
            float systolic = (float)HealthProfile.HuyetApTamThu;
            float diastolic = (float)HealthProfile.HuyetApTamTruong;
            
            if (systolic < 90 || diastolic < 60)
                return "Thấp";
            else if (systolic < 120 && diastolic < 80)
                return "Lý tưởng";
            else if (systolic < 130 && diastolic < 85)
                return "Bình thường";
            else if (systolic < 140 && diastolic < 90)
                return "Bình thường cao";
            else
                return "Cao";
        }
        
        // Phương thức để lấy màu sắc cho huyết áp
        public string GetBloodPressureColor()
        {
            if (HealthProfile == null)
                return "text-secondary";
                
            float systolic = (float)HealthProfile.HuyetApTamThu;
            float diastolic = (float)HealthProfile.HuyetApTamTruong;
            
            if (systolic < 90 || diastolic < 60)
                return "text-warning";
            else if (systolic < 120 && diastolic < 80)
                return "text-success";
            else if (systolic < 130 && diastolic < 85)
                return "text-success";
            else if (systolic < 140 && diastolic < 90)
                return "text-warning";
            else
                return "text-danger";
        }
        
        // Phương thức để lấy trạng thái đường huyết
        public string GetBloodSugarStatus()
        {
            if (HealthProfile == null)
                return "Chưa có dữ liệu";
                
            float bloodSugar = (float)HealthProfile.DuongHuyet;
            
            if (bloodSugar < 3.9f)
                return "Thấp";
            else if (bloodSugar <= 5.5f)
                return "Bình thường";
            else if (bloodSugar <= 7.0f)
                return "Tiền tiểu đường";
            else
                return "Cao";
        }
        
        // Phương thức để lấy màu sắc cho đường huyết
        public string GetBloodSugarColor()
        {
            if (HealthProfile == null)
                return "text-secondary";
                
            float bloodSugar = (float)HealthProfile.DuongHuyet;
            
            if (bloodSugar < 3.9f)
                return "text-warning";
            else if (bloodSugar <= 5.5f)
                return "text-success";
            else if (bloodSugar <= 7.0f)
                return "text-warning";
            else
                return "text-danger";
        }
    }
}
