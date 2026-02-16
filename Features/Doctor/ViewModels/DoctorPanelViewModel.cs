using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Desktop.Features.Doctor.ViewModels;

public partial class DoctorPanelViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _waitingPatients = new();

    [ObservableProperty]
    private QueueTicket? _currentPatient;

    [ObservableProperty]
    private string _diagnosis = string.Empty;

    [ObservableProperty]
    private string _prescriptions = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private int _waitingCount;

    [ObservableProperty]
    private int _completedToday;

    // Task 1: Visit History
    [ObservableProperty]
    private ObservableCollection<Visit> _patientHistory = new();

    [ObservableProperty]
    private bool _isHistoryExpanded = false;

    // Task 2: Enhanced Diagnosis - Common Diagnoses
    public ObservableCollection<string> CommonDiagnoses { get; } = new()
    {
        "نزلة برد",
        "إنفلونزا",
        "صداع",
        "ألم المعدة",
        "التهاب الحلق",
        "ارتفاع ضغط الدم",
        "السكري",
        "حساسية",
        "أخرى..."
    };

    [ObservableProperty]
    private string? _selectedDiagnosis;

    // Task 2: Common Medications
    public ObservableCollection<string> CommonMedications { get; } = new()
    {
        "باراسيتامول 500mg - مرتين يومياً",
        "أموكسيسيلين 500mg - ثلاث مرات يومياً",
        "إيبوبروفين 400mg - عند الحاجة",
        "أوميبرازول 20mg - قبل الفطور",
        "أسبرين 100mg - مرة يومياً",
        "فيتامين د 1000 وحدة - يومياً"
    };

    [ObservableProperty]
    private string? _selectedMedication;

    // Task 2: Vital Signs
    [ObservableProperty]
    private string _bloodPressure = string.Empty; // e.g., "120/80"

    [ObservableProperty]
    private decimal? _temperature; // in Celsius

    [ObservableProperty]
    private int? _heartRate; // BPM

    [ObservableProperty]
    private decimal? _weight; // in KG

    // Task 3: Statistics
    [ObservableProperty]
    private int _todayPatients;

    [ObservableProperty]
    private int _weekPatients;

    [ObservableProperty]
    private int _monthPatients;

    public DoctorPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadQueue();
        LoadStatistics();
    }

    public void LoadQueue()
    {
        var today = DateTime.Today;
        
        // Load waiting patients
        var waiting = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today && 
                       (q.Status == TicketStatus.Waiting || q.Status == TicketStatus.AwaitingRecall))
            .OrderBy(q => q.Status == TicketStatus.AwaitingRecall ? 0 : 1) // Recall first
            .ThenBy(q => q.TicketNumber)
            .ToList();
        
        WaitingPatients = new ObservableCollection<QueueTicket>(waiting);
        WaitingCount = waiting.Count;

        // Get current patient (being served)
        CurrentPatient = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today && 
                       (q.Status == TicketStatus.Called || q.Status == TicketStatus.InProgress || q.Status == TicketStatus.Present))
            .OrderByDescending(q => q.CalledAt)
            .FirstOrDefault();

        // Stats
        CompletedToday = _db.QueueTickets
            .Count(q => q.CreatedAt.Date == today && q.Status == TicketStatus.Completed);

        // Load visit history if current patient changed
        if (CurrentPatient != null)
        {
            LoadPatientHistory(CurrentPatient.PatientId);
        }
        else
        {
            PatientHistory.Clear();
        }
    }

    // Task 1: Load Patient Visit History
    private void LoadPatientHistory(Guid patientId)
    {
        var history = _db.Visits
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.VisitDate)
            .Take(10)
            .ToList();
        
        PatientHistory = new ObservableCollection<Visit>(history);
    }

    // Task 3: Load Statistics
    private void LoadStatistics()
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        TodayPatients = _db.Visits.Count(v => v.VisitDate.Date == today);
        WeekPatients = _db.Visits.Count(v => v.VisitDate >= weekStart);
        MonthPatients = _db.Visits.Count(v => v.VisitDate >= monthStart);
    }

    [RelayCommand]
    private void CallNext()
    {
        if (WaitingPatients.Count == 0)
            return;

        // If there's a current patient, mark as absent if not already processed
        if (CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called)
        {
            CurrentPatient.Status = TicketStatus.AwaitingRecall;
            CurrentPatient.CallCount++;
            _db.SaveChanges();
        }

        // Get next patient
        var next = WaitingPatients.FirstOrDefault();
        if (next == null) return;

        next.Status = TicketStatus.Called;
        next.CalledAt = DateTime.Now;
        next.CallCount++;
        _db.SaveChanges();

        LoadQueue();
    }

    [RelayCommand]
    private void CallSpecific(QueueTicket ticket)
    {
        if (ticket == null) return;

        // If there's a current patient, mark as awaiting recall
        if (CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called)
        {
            CurrentPatient.Status = TicketStatus.AwaitingRecall;
            _db.SaveChanges();
        }

        ticket.Status = TicketStatus.Called;
        ticket.CalledAt = DateTime.Now;
        ticket.CallCount++;
        _db.SaveChanges();

        LoadQueue();
    }

    [RelayCommand]
    private void MarkPresent()
    {
        if (CurrentPatient == null) return;

        CurrentPatient.Status = TicketStatus.InProgress;
        _db.SaveChanges();
        LoadQueue();
    }

    [RelayCommand]
    private void MarkAbsent()
    {
        if (CurrentPatient == null) return;

        CurrentPatient.Status = TicketStatus.AwaitingRecall;
        _db.SaveChanges();
        LoadQueue();
    }

    [RelayCommand]
    private void CompleteVisit()
    {
        if (CurrentPatient == null) return;

        // Create visit record
        var visit = new Visit
        {
            PatientId = CurrentPatient.PatientId,
            DoctorId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // Default doctor for now
            Diagnosis = Diagnosis,
            Prescriptions = Prescriptions,
            Notes = Notes,
            VisitDate = DateTime.Now
        };

        _db.Visits.Add(visit);

        // Complete the ticket
        CurrentPatient.Status = TicketStatus.Completed;
        CurrentPatient.CompletedAt = DateTime.Now;
        _db.SaveChanges();

        // Clear form
        Diagnosis = string.Empty;
        Prescriptions = string.Empty;
        Notes = string.Empty;

        LoadQueue();
        LoadStatistics();
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadQueue();
        LoadStatistics();
    }

    // Task 2: Add selected diagnosis to diagnosis field
    partial void OnSelectedDiagnosisChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && value != "أخرى...")
        {
            Diagnosis = value;
        }
    }

    // Task 2: Add selected medication to prescriptions field
    partial void OnSelectedMedicationChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (!string.IsNullOrWhiteSpace(Prescriptions))
                Prescriptions += "\n";
            Prescriptions += value;
        }
    }
}
