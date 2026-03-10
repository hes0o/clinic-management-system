using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using HealthCenter.Desktop.Services;

namespace HealthCenter.Desktop.Features.Reception.ViewModels;

/// <summary>
/// Reception panel: registers/searches patients, issues queue tickets.
/// Uses EF Core directly — data persists to the database.
/// </summary>
public partial class ReceptionViewModel : ObservableObject
{
    private readonly HealthCenterDbContext _db;
    private readonly DispatcherTimer _refreshTimer;

    // ── Search ────────────────────────────────────────────────
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private Patient? _selectedPatient;
    public ObservableCollection<Patient> SearchResults { get; } = new();

    // ── Registration Form Fields ──────────────────────────────
    [ObservableProperty] private string _patientName = string.Empty;
    [ObservableProperty] private string _nationalId = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string? _emergencyContact;
    [ObservableProperty] private DateTime? _selectedBirthDate;
    [ObservableProperty] private string? _selectedGender;
    [ObservableProperty] private string? _selectedBloodType;
    [ObservableProperty] private string? _address;
    [ObservableProperty] private string? _allergies;
    [ObservableProperty] private string? _chronicConditions;
    [ObservableProperty] private string? _insuranceProvider;
    [ObservableProperty] private string? _insuranceNumber;

    // ── State ─────────────────────────────────────────────────
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isError;

    // ── Today's Queue ─────────────────────────────────────────
    public ObservableCollection<QueueTicket> TodayQueue { get; } = new();
    [ObservableProperty] private int _todayCount;
    [ObservableProperty] private int _waitingCount;

    private Guid? _editingPatientId;

    // ── Static Dropdowns ──────────────────────────────────────
    public ObservableCollection<string> GenderOptions { get; } = new() { "ذكر", "أنثى" };
    public ObservableCollection<string> BloodTypes { get; } = new()
        { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

    public ReceptionViewModel()
    {
        _db = new HealthCenterDbContext();
        _db.Database.EnsureCreated();
        RefreshSearch();
        LoadTodayQueue();

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _refreshTimer.Tick += (s, e) => LoadQueueSilent();
        _refreshTimer.Start();
    }

    // ── Reactive search as user types ────────────────────────
    partial void OnSearchQueryChanged(string value) => RefreshSearch();
    partial void OnSelectedPatientChanged(Patient? value) => LoadSelectedPatientDetails(value);

    // ── Search ────────────────────────────────────────────────
    private void RefreshSearch()
    {
        SearchResults.Clear();
        var q = SearchQuery.Trim();

        var patients = string.IsNullOrWhiteSpace(q)
            ? _db.Patients.OrderByDescending(p => p.CreatedAt).Take(50).ToList()
            : _db.Patients.Where(p =>
                p.FullName.Contains(q) ||
                (p.NationalId != null && p.NationalId.Contains(q)) ||
                p.PhoneNumber.Contains(q))
              .OrderBy(p => p.FullName)
              .Take(50)
              .ToList();

        foreach (var p in patients)
            SearchResults.Add(p);
    }

    [RelayCommand]
    private void Search() => RefreshSearch();

    // ── Load queue ────────────────────────────────────────────
    private void LoadTodayQueue()
    {
        TodayQueue.Clear();
        var today = DateTime.Today;
        var tickets = _db.QueueTickets
            .Include(t => t.Patient)
            .Where(t => t.CreatedAt.Date == today)
            .OrderBy(t => t.TicketNumber)
            .ToList();

        TodayCount = tickets.Count;
        WaitingCount = tickets.Count(t => t.Status == TicketStatus.Waiting || t.Status == TicketStatus.AwaitingRecall);

        foreach (var t in tickets) TodayQueue.Add(t);
    }

    // ── Patient selection → pre-fill read-only details ────────
    private void LoadSelectedPatientDetails(Patient? p)
    {
        if (p == null) return;
        // Populate form for viewing (switches to Edit mode when Edit is clicked)
    }

    // ── Registration: Save / Update ───────────────────────────
    [RelayCommand]
    private void SavePatient()
    {
        if (string.IsNullOrWhiteSpace(PatientName) || PatientName.Trim().Length < 3)
        { ShowError("الاسم يجب أن يكون 3 أحرف على الأقل."); return; }

        if (string.IsNullOrWhiteSpace(PhoneNumber) || PhoneNumber.Trim().Length < 9)
        { ShowError("رقم الهاتف غير صحيح."); return; }

        if (IsEditMode && _editingPatientId.HasValue)
        {
            // UPDATE
            var existing = _db.Patients.Find(_editingPatientId.Value);
            if (existing == null) { ShowError("المريض غير موجود."); return; }

            existing.FullName = PatientName.Trim();
            existing.NationalId = string.IsNullOrWhiteSpace(NationalId) ? null : NationalId.Trim();
            existing.PhoneNumber = PhoneNumber.Trim();
            existing.EmergencyContact = EmergencyContact;
            existing.DateOfBirth = SelectedBirthDate;
            existing.Gender = SelectedGender == "ذكر" ? Gender.Male : (SelectedGender == "أنثى" ? Gender.Female : null);
            existing.Address = Address;
            existing.BloodType = SelectedBloodType;
            existing.Allergies = Allergies;
            existing.ChronicConditions = ChronicConditions;
            existing.InsuranceProvider = InsuranceProvider;
            existing.InsuranceNumber = InsuranceNumber;
            existing.UpdatedAt = DateTime.UtcNow;

            _db.SaveChanges();
            ShowSuccess("تم تحديث بيانات المريض بنجاح.");
        }
        else
        {
            // Check NationalId uniqueness
            if (!string.IsNullOrWhiteSpace(NationalId) &&
                _db.Patients.Any(p => p.NationalId == NationalId.Trim()))
            { ShowError("يوجد مريض مسجّل بهذا الرقم الوطني مسبقاً."); return; }

            var patient = new Patient
            {
                FullName = PatientName.Trim(),
                NationalId = string.IsNullOrWhiteSpace(NationalId) ? null : NationalId.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                EmergencyContact = EmergencyContact,
                DateOfBirth = SelectedBirthDate,
                Gender = SelectedGender == "ذكر" ? Gender.Male : (SelectedGender == "أنثى" ? Gender.Female : null),
                Address = Address,
                BloodType = SelectedBloodType,
                Allergies = Allergies,
                ChronicConditions = ChronicConditions,
                InsuranceProvider = InsuranceProvider,
                InsuranceNumber = InsuranceNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Patients.Add(patient);
            _db.SaveChanges();

            SelectedPatient = patient;
            ShowSuccess($"تم تسجيل المريض بنجاح.");
        }

        ClearForm();
        RefreshSearch();
        LoadTodayQueue();
        SelectedTabIndex = 1; // Switch to search tab
    }

    // ── Queue Ticket ──────────────────────────────────────────
    [RelayCommand]
    private void IssueTicket(Patient patient)
    {
        // Prevent double-ticketing today
        if (_db.QueueTickets.Any(t =>
            t.PatientId == patient.Id &&
            t.CreatedAt.Date == DateTime.Today &&
            t.Status != TicketStatus.Completed))
        {
            ShowError($"المريض {patient.FullName} لديه تذكرة نشطة اليوم.");
            return;
        }

        // Find today's max ticket number
        var today = DateTime.Today;
        int nextNum = _db.QueueTickets.Any(t => t.CreatedAt.Date == today)
            ? _db.QueueTickets.Where(t => t.CreatedAt.Date == today).Max(t => t.TicketNumber) + 1
            : 1;

        // Assign to first available doctor
        var doctor = _db.Users.FirstOrDefault(u => u.Role == UserRole.Doctor && u.IsActive);

        var ticket = new QueueTicket
        {
            PatientId = patient.Id,
            DoctorId = doctor?.Id,
            TicketNumber = nextNum,
            Status = TicketStatus.Waiting,
            CreatedAt = DateTime.Now,
        };

        _db.QueueTickets.Add(ticket);
        _db.SaveChanges();

        ShowSuccess($"تم إصدار تذكرة رقم {nextNum} للمريض {patient.FullName}");
        LoadTodayQueue();
    }

    // ── Edit ──────────────────────────────────────────────────
    [RelayCommand]
    private void EditPatient(Patient patient)
    {
        PatientName = patient.FullName;
        NationalId = patient.NationalId ?? string.Empty;
        PhoneNumber = patient.PhoneNumber;
        EmergencyContact = patient.EmergencyContact;
        SelectedBirthDate = patient.DateOfBirth;
        SelectedGender = patient.Gender == Gender.Male ? "ذكر" : patient.Gender == Gender.Female ? "أنثى" : null;
        SelectedBloodType = patient.BloodType;
        Address = patient.Address;
        Allergies = patient.Allergies;
        ChronicConditions = patient.ChronicConditions;
        InsuranceProvider = patient.InsuranceProvider;
        InsuranceNumber = patient.InsuranceNumber;

        _editingPatientId = patient.Id;
        IsEditMode = true;
        SelectedTabIndex = 0; // Switch to form tab
    }

    [RelayCommand]
    private void CancelEdit()
    {
        ClearForm();
        IsEditMode = false;
    }

    // ── Cancel / Clear ────────────────────────────────────────
    private void ClearForm()
    {
        PatientName = string.Empty; NationalId = string.Empty; PhoneNumber = string.Empty;
        EmergencyContact = null; SelectedBirthDate = null; SelectedGender = null;
        SelectedBloodType = null; Address = null; Allergies = null;
        ChronicConditions = null; InsuranceProvider = null; InsuranceNumber = null;
        _editingPatientId = null; IsEditMode = false;
    }

    // ── Helpers ───────────────────────────────────────────────
    private void ShowError(string msg) { StatusMessage = msg; IsError = true; }
    private void ShowSuccess(string msg) { StatusMessage = msg; IsError = false; }

    private void LoadQueueSilent()
    {
        var today = DateTime.Today;
        var tickets = _db.QueueTickets
            .Include(t => t.Patient)
            .Where(t => t.CreatedAt.Date == today)
            .OrderBy(t => t.TicketNumber)
            .ToList();

        if (tickets.Count != TodayQueue.Count)
        {
            TodayQueue.Clear();
            foreach (var t in tickets) TodayQueue.Add(t);
        }

        TodayCount = tickets.Count;
        WaitingCount = tickets.Count(t => t.Status == TicketStatus.Waiting || t.Status == TicketStatus.AwaitingRecall);
    }
}