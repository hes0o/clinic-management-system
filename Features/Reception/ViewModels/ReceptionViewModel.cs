using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

    // ── Validation Error Messages ──────────────────────────────
    [ObservableProperty] private string _nationalIdError = string.Empty;
    [ObservableProperty] private string _phoneNumberError = string.Empty;
    [ObservableProperty] private string _emergencyContactError = string.Empty;

    /// <summary>True when all validated fields have no outstanding errors.
    /// Bind the Save/Register button's IsEnabled to this property.</summary>
    public bool IsFormValid =>
        string.IsNullOrEmpty(NationalIdError) &&
        string.IsNullOrEmpty(PhoneNumberError) &&
        string.IsNullOrEmpty(EmergencyContactError);

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
    }

    // ── Reactive search as user types ────────────────────────
    partial void OnSearchQueryChanged(string value) => RefreshSearch();
    partial void OnSelectedPatientChanged(Patient? value) => LoadSelectedPatientDetails(value);

    // ── Reactive field validation ────────────────────────────────
    partial void OnNationalIdChanged(string value)
    {
        NationalIdError = IsValidNationalId(value)
            ? string.Empty
            : string.IsNullOrEmpty(value)
                ? string.Empty   // field may be left blank – only validated on Save
                : "يجب أن يحتوي الرقم الوطني على أرقام فقط (11-12 خانة).";
        OnPropertyChanged(nameof(IsFormValid));
    }

    partial void OnPhoneNumberChanged(string value)
    {
        PhoneNumberError = string.IsNullOrWhiteSpace(value)
            ? string.Empty   // required check deferred to Save
            : IsValidPhone(value)
                ? string.Empty
                : "رقم الهاتف غير صحيح (أرقام فقط، يمكن بدء بـ +).";
        OnPropertyChanged(nameof(IsFormValid));
    }

    partial void OnEmergencyContactChanged(string? value)
    {
        EmergencyContactError = string.IsNullOrWhiteSpace(value)
            ? string.Empty   // optional field – blank is valid
            : IsValidPhone(value!)
                ? string.Empty
                : "هاتف الطوارئ غير صحيح (أرقام فقط، يمكن بدء بـ +).";
        OnPropertyChanged(nameof(IsFormValid));
    }

    // ── Validation Helpers ──────────────────────────────────────
    /// <summary>National ID: digits only, 11 or 12 characters.</summary>
    private static bool IsValidNationalId(string value) =>
        !string.IsNullOrEmpty(value) &&
        Regex.IsMatch(value, @"^\d{11,12}$");

    /// <summary>
    /// Phone number: optional leading '+', then digits only.
    /// Examples: 0501234567, +905001234567
    /// </summary>
    private static bool IsValidPhone(string value) =>
        !string.IsNullOrEmpty(value) &&
        Regex.IsMatch(value, @"^\+?\d+$");

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
    private async Task SavePatient()
    {
        // ── 1. Name check ────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(PatientName) || PatientName.Trim().Length < 3)
        { ShowError("الاسم يجب أن يكون 3 أحرف على الأقل."); return; }

        // ── 2. National ID validation (digits only, 11-12 chars) ─
        if (!string.IsNullOrWhiteSpace(NationalId) && !IsValidNationalId(NationalId.Trim()))
        { ShowError("الرقم الوطني يجب أن يتكون من 11-12 رقماً فقط."); return; }

        // ── 3. Phone validation (digits + optional leading +) ────
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        { ShowError("رقم الهاتف مطلوب."); return; }

        if (!IsValidPhone(PhoneNumber.Trim()))
        { ShowError("رقم الهاتف غير صحيح – أرقام فقط، يمكن بدء بـ +."); return; }

        // ── 4. Emergency contact validation (optional) ───────────
        if (!string.IsNullOrWhiteSpace(EmergencyContact) && !IsValidPhone(EmergencyContact!.Trim()))
        { ShowError("هاتف الطوارئ غير صحيح – أرقام فقط، يمكن بدء بـ +."); return; }

        // ── 5. Inline error indicators must be clear ─────────────
        if (!IsFormValid) { ShowError("يرجى تصحيح أخطاء الإدخال قبل الحفظ."); return; }

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
            // ── When you move persistence into SavePatientToDatabaseAsync,
            //    remove the _db.SaveChanges() above and uncomment this line:
            // await SavePatientToDatabaseAsync(existing);
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
            // ── When you move persistence into SavePatientToDatabaseAsync,
            //    remove _db.Patients.Add() and _db.SaveChanges() above
            //    and uncomment this line:
            // await SavePatientToDatabaseAsync(patient);

            SelectedPatient = patient;
            ShowSuccess($"تم تسجيل المريض بنجاح.");
        }

        ClearForm();
        RefreshSearch();
        LoadTodayQueue();
        SelectedTabIndex = 1; // Switch to search tab
    }

    // ── Database Stub (Future-proofing) ───────────────────────────
    /// <summary>
    /// Asynchronous database persistence stub.
    /// All validation has already been performed by <see cref="SavePatient"/> before this is called.
    ///
    /// HOW TO WIRE UP ENTITY FRAMEWORK CORE (EF Core + SQLite / SQL Server)
    /// -------------------------------------------------------------------------
    /// Step 1 – Inject the DbContext via constructor (already available as _db).
    ///
    /// Step 2 – For a NEW patient record:
    ///   await _db.Patients.AddAsync(patient);
    ///   await _db.SaveChangesAsync();
    ///
    /// Step 3 – For an EXISTING (edited) patient record:
    ///   _db.Patients.Update(patient);
    ///   // or: _db.Entry(patient).State = EntityState.Modified;
    ///   await _db.SaveChangesAsync();
    ///
    /// Step 4 (optional) – Wrap in a transaction for multi-table writes:
    ///   await using var tx = await _db.Database.BeginTransactionAsync();
    ///   try { ... await _db.SaveChangesAsync(); await tx.CommitAsync(); }
    ///   catch { await tx.RollbackAsync(); throw; }
    ///
    /// Step 5 (optional, raw SQLite without EF):
    ///   using var conn = new SqliteConnection("Data Source=healthcenter.db");
    ///   await conn.OpenAsync();
    ///   var cmd = conn.CreateCommand();
    ///   cmd.CommandText = "INSERT INTO Patients (...) VALUES (@name, @phone, ...);";
    ///   cmd.Parameters.AddWithValue("@name", patient.FullName);
    ///   // ... add remaining parameters ...
    ///   await cmd.ExecuteNonQueryAsync();
    /// </summary>
    private Task SavePatientToDatabaseAsync(Patient patient)
    {
        // TODO: Replace this stub with your async EF Core or raw SQLite persistence logic.
        // Until then, SavePatient() continues to use _db.SaveChanges() directly.
        _ = patient; // suppress unused-parameter warning
        return Task.CompletedTask;
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

    [RelayCommand]
    private void RefreshQueue() { LoadTodayQueue(); RefreshSearch(); }
}