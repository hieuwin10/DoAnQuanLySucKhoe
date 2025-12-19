using System.Threading.Tasks;
using DoAnChamSocSucKhoe.Areas.Doctor.Models;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Repositories
{
    public interface IDoctorDashboardRepository
    {
        Task<DoctorDashboardViewModel> GetDashboardDataAsync(string doctorId);
        Task<DoctorProfile> GetDoctorProfileAsync(string doctorId);
        Task<DashboardStats> GetDashboardStatsAsync(string doctorId);
        Task<List<Appointment>> GetUpcomingAppointmentsAsync(string doctorId, int limit = 5);
        Task<List<PatientIntermediate>> GetRecentPatientsAsync(string doctorId, int limit = 5);
        Task<List<Consultation>> GetRecentConsultationsAsync(string doctorId, int limit = 5);
        Task<List<Notification>> GetNotificationsAsync(string doctorId, int limit = 10);
    }
} 