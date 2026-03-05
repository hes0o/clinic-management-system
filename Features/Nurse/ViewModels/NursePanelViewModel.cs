using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
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
    [ObservableProperty] private bool _hasNoPatients;

    private readonly DispatcherTimer _refreshTimer;

    public NursePanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadQueue();

        // Setup background polling every 5 seconds
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _refreshTimer.Tick += (sender, e) => LoadQueueSilent();
        _refreshTimer.Start();
    }

    private void LoadQueueSilent()
    {
        try
        {
            var today = DateTime.Today;
            var tickets = _db.QueueTickets
                .Include(q => q.Patient)
                .Where(q => q.CreatedAt.Date == today &&
                            (q.Status == TicketStatus.Waiting || q.Status == TicketStatus.AwaitingRecall))
                .OrderBy(q => q.TicketNumber)
                .ToList();

            // Only update if count changed or top ticket changed to avoid UI flickering
            if (WaitingQueue.Count != tickets.Count ||
                (tickets.Count > 0 && WaitingQueue.Count > 0 && tickets[0].Id != WaitingQueue[0].Id))
            {
                WaitingQueue = new ObservableCollection<QueueTicket>(tickets);
                HasNoPatients = WaitingQueue.Count == 0;
            }
        }
        catch (Exception)
        {
            // Fail silently in the background
        }
    }

    // Clears vitals fields whenever a different patient is selected,
    // so the nurse cannot accidentally submit stale data from the previous patient.
    partial void OnSelectedTicketChanged(QueueTicket? value)
    {
        BloodPressure = string.Empty;
        Temperature = string.Empty;
        HeartRate = string.Empty;
        Weight = string.Empty;
        StatusMessage = string.Empty;
        IsError = false;
    }

    private void LoadQueue()
    {
        try
        {
            var today = DateTime.Today;
            var tickets = _db.QueueTickets
                .Include(q => q.Patient)
                .Where(q => q.CreatedAt.Date == today &&
                            (q.Status == TicketStatus.Waiting || q.Status == TicketStatus.AwaitingRecall))
                .OrderBy(q => q.TicketNumber)
                .ToList();

            WaitingQueue = new ObservableCollection<QueueTicket>(tickets);
            HasNoPatients = WaitingQueue.Count == 0;
        }
        catch (Exception ex)
        {
            ShowError($"خطأ في تحميل قائمة الانتظار: {ex.Message}");
            WaitingQueue = new ObservableCollection<QueueTicket>();
            HasNoPatients = true;
        }
    }

    [RelayCommand]
    private void SaveVitals()
    {
        if (SelectedTicket == null)
        {
            ShowError("الرجاء تحديد مريض من القائمة أولاً.");
            return;
        }

        try
        {
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
                    ShowError("لا يوجد طبيب مسجّل في النظام.");
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

            // Advance ticket status so the Doctor panel picks up the patient
            SelectedTicket.Status = TicketStatus.ReadyForDoctor;

            _db.SaveChanges();
            ShowSuccess($"تم حفظ العلامات الحيوية وإرسال المريض {SelectedTicket.Patient?.FullName} إلى الطبيب");

            // Clear fields
            BloodPressure = string.Empty;
            Temperature = string.Empty;
            HeartRate = string.Empty;
            Weight = string.Empty;
            SelectedTicket = null;

            // Reload queue — patient is no longer Waiting
            LoadQueue();
        }
        catch (Exception ex)
        {
            ShowError($"خطأ في حفظ العلامات الحيوية: {ex.Message}");
        }
    }
}
