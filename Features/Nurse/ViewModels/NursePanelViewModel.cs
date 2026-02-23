using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using HealthCenter.Desktop.Services;

namespace HealthCenter.Desktop.Features.Nurse.ViewModels;

public partial class NursePanelViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    [ObservableProperty] private ObservableCollection<QueueTicket> _waitingQueue = new();
    [ObservableProperty] private QueueTicket? _selectedTicket;
    [ObservableProperty] private string _bloodPressure = string.Empty;
    [ObservableProperty] private string _temperature = string.Empty;
    [ObservableProperty] private string _heartRate = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public NursePanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadQueue();
    }

    private void LoadQueue()
    {
        var today = DateTime.Today;
        var tickets = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today &&
                        (q.Status == TicketStatus.Waiting || q.Status == TicketStatus.AwaitingRecall))
            .OrderBy(q => q.TicketNumber)
            .ToList();

        WaitingQueue = new ObservableCollection<QueueTicket>(tickets);
    }

    [RelayCommand]
    private void SaveVitals()
    {
        if (SelectedTicket == null)
        {
            StatusMessage = "الرجاء تحديد مريض من القائمة أولاً.";
            return;
        }

        // Find or initialize a Visit for this patient today
        var today = DateTime.Today;
        var visit = _db.Visits.FirstOrDefault(v =>
            v.PatientId == SelectedTicket.PatientId &&
            v.VisitDate.Date == today);

        if (visit == null)
        {
            // Get current nurse
            var nurse = AuthService.Instance.CurrentUser;

            // Doctor must be set — try to find one
            var doctor = _db.Users.FirstOrDefault(u => u.Role == UserRole.Doctor || u.Role == UserRole.SuperAdmin);
            if (doctor == null)
            {
                StatusMessage = "لا يوجد طبيب مسجّل في النظام.";
                return;
            }

            visit = new Visit
            {
                PatientId = SelectedTicket.PatientId,
                DoctorId = doctor.Id,
                NurseId = nurse?.Id,
                VisitDate = DateTime.Now,
                CreatedAt = DateTime.UtcNow
            };
            _db.Visits.Add(visit);
        }

        // Save vitals
        if (!string.IsNullOrWhiteSpace(BloodPressure)) visit.BloodPressure = BloodPressure;
        if (decimal.TryParse(Temperature, out var temp)) visit.Temperature = temp;
        if (int.TryParse(HeartRate, out var hr)) visit.HeartRate = hr;
        if (decimal.TryParse(Weight, out var wt)) visit.Weight = wt;

        _db.SaveChanges();
        StatusMessage = $"تم حفظ العلامات الحيوية للمريض: {SelectedTicket.Patient?.FullName}";

        // Clear fields
        BloodPressure = string.Empty;
        Temperature = string.Empty;
        HeartRate = string.Empty;
        Weight = string.Empty;
    }

    [RelayCommand]
    private void RefreshQueue()
    {
        LoadQueue();
        StatusMessage = "تم تحديث قائمة المرضى.";
    }
}
