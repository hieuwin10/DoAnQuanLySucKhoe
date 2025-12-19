using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Doctor.Models
{
    public class DoctorDashboardViewModel
    {
        public required DoctorProfile Profile { get; set; }
        public required DashboardStats Stats { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public List<PatientIntermediate> RecentPatients { get; set; } = new List<PatientIntermediate>();
        public List<Consultation> RecentConsultations { get; set; } = new List<Consultation>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class DoctorProfile
    {
        public required string FullName { get; set; }
        public required string Specialization { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string ProfileImageUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public required string Hospital { get; set; }
        public required string Department { get; set; }
    }

    public class DashboardStats
    {
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int ActiveConsultations { get; set; }
        public int PendingTasks { get; set; }
    }

    public class Appointment
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public DateTime AppointmentTime { get; set; }
        public required string Status { get; set; }
        public required string Type { get; set; }
        public required string Notes { get; set; }
    }

    public class Patient
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string ProfileImageUrl { get; set; }
        public DateTime LastVisit { get; set; }
        public required string LastDiagnosis { get; set; }
    }

    public class Consultation
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Status { get; set; }
        public required string LastMessage { get; set; }
        public required string PatientImageUrl { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public required string Type { get; set; }
    }
}