using DoAnChamSocSucKhoe.Models;
using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Patient.Models
{
    public class PatientDashboardViewModel
    {
        // Hồ sơ sức khỏe
        public HoSoSucKhoe HealthProfile { get; set; }
        
        // Chỉ số BMI
        public double? BMI { get; set; }
        
        // Lịch sử chỉ số sức khỏe
        public List<ChiSoSucKhoe> HealthMetrics { get; set; }
        
        // Lịch hẹn sắp tới
        public List<LichHen> UpcomingAppointments { get; set; }
        
        // Nhắc nhở sức khỏe
        public List<NhacNhoSucKhoe> HealthReminders { get; set; }
        
        // Tư vấn gần đây
        public List<TuVanSucKhoe> RecentConsultations { get; set; }
        
        // Kế hoạch dinh dưỡng
        public List<KeHoachDinhDuong> NutritionPlans { get; set; }
        
        // Kế hoạch tập luyện
        public List<KeHoachTapLuyen> ExercisePlans { get; set; }
        
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
            if (!HealthProfile.NhipTim.HasValue)
                return "Chưa có dữ liệu";
                
            int heartRate = HealthProfile.NhipTim.Value;
            
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
            if (!HealthProfile.NhipTim.HasValue)
                return "text-secondary";
                
            int heartRate = HealthProfile.NhipTim.Value;
            
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
            if (!HealthProfile.HuyetApTamThu.HasValue || !HealthProfile.HuyetApTamTruong.HasValue)
                return "Chưa có dữ liệu";
                
            int systolic = HealthProfile.HuyetApTamThu.Value;
            int diastolic = HealthProfile.HuyetApTamTruong.Value;
            
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
            if (!HealthProfile.HuyetApTamThu.HasValue || !HealthProfile.HuyetApTamTruong.HasValue)
                return "text-secondary";
                
            int systolic = HealthProfile.HuyetApTamThu.Value;
            int diastolic = HealthProfile.HuyetApTamTruong.Value;
            
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
            if (!HealthProfile.DuongHuyet.HasValue)
                return "Chưa có dữ liệu";
                
            decimal bloodSugar = HealthProfile.DuongHuyet.Value;
            
            if (bloodSugar < 3.9m)
                return "Thấp";
            else if (bloodSugar <= 5.5m)
                return "Bình thường";
            else if (bloodSugar <= 7.0m)
                return "Tiền tiểu đường";
            else
                return "Cao";
        }
        
        // Phương thức để lấy màu sắc cho đường huyết
        public string GetBloodSugarColor()
        {
            if (!HealthProfile.DuongHuyet.HasValue)
                return "text-secondary";
                
            decimal bloodSugar = HealthProfile.DuongHuyet.Value;
            
            if (bloodSugar < 3.9m)
                return "text-warning";
            else if (bloodSugar <= 5.5m)
                return "text-success";
            else if (bloodSugar <= 7.0m)
                return "text-warning";
            else
                return "text-danger";
        }
    }
}
