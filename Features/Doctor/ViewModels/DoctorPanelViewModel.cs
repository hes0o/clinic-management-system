using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Desktop.Features.Doctor.ViewModels;

public partial class DoctorPanelViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private const int MaxTextLength = 1000;
    private const int MaxLabRequestLength = 300;

    private readonly HealthCenterDbContext _db;
    private readonly DispatcherTimer _refreshTimer;

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _waitingPatients = new();

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _absentRecallPatients = new();

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

    // Status banner (success/error messages)
    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isError;

    /// <summary>True when status message is shown and it is a success (not error). Used by UI to show success banner only.</summary>
    public bool IsSuccessMessage => !string.IsNullOrEmpty(StatusMessage) && !IsError;

    // Modal dialog (box message)
    [ObservableProperty]
    private bool _isDialogVisible;

    [ObservableProperty]
    private string _dialogTitle = string.Empty;

    [ObservableProperty]
    private string _dialogMessage = string.Empty;

    [ObservableProperty]
    private bool _isDialogError;

    [ObservableProperty]
    private bool _isDialogConfirm;

    [ObservableProperty]
    private string _dialogPrimaryText = "حسناً";

    [ObservableProperty]
    private string _dialogSecondaryText = "إلغاء";

    private Action? _dialogPrimaryAction;
    private Action? _dialogSecondaryAction;

    [ObservableProperty]
    private bool _canCompleteVisit;

    [ObservableProperty]
    private bool _isAwaitingCompletionConfirmation;

    // Task 1: Visit History
    [ObservableProperty]
    private ObservableCollection<Visit> _patientHistory = new();

    [ObservableProperty]
    private Visit? _selectedHistoryVisit;

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

    // Task 2: Vital Signs (string inputs for safe parsing and Arabic validation messages)
    [ObservableProperty]
    private string _bloodPressure = string.Empty;

    [ObservableProperty]
    private string _temperatureInput = string.Empty;

    [ObservableProperty]
    private string _heartRateInput = string.Empty;

    [ObservableProperty]
    private string _weightInput = string.Empty;

    [ObservableProperty]
    private string _labTestName = string.Empty;

    // Task 4: Common Lab Tests
    public ObservableCollection<string> CommonLabTests { get; } = new()
    {
        "تحليل دم كامل CBC",
        "فحص السكر التراكمي HbA1c",
        "فحص وظائف الكلى",
        "فحص وظائف الكبد",
        "تحليل بول",
        "فحص الغدة الدرقية TSH"
    };

    [ObservableProperty]
    private string? _selectedLabTest;

    // Task 3: Statistics
    [ObservableProperty]
    private int _todayPatients;

    [ObservableProperty]
    private int _weekPatients;

    [ObservableProperty]
    private int _monthPatients;

    // Vital Signs Empty Check
    public bool IsVitalSignsEmpty => string.IsNullOrWhiteSpace(BloodPressure) &&
                                      string.IsNullOrWhiteSpace(TemperatureInput) &&
                                      string.IsNullOrWhiteSpace(HeartRateInput) &&
                                      string.IsNullOrWhiteSpace(WeightInput);

    public DoctorPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadQueue();
        LoadStatistics();
        UpdateCanCompleteVisit();

        // Setup background polling every 5 seconds
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _refreshTimer.Tick += (sender, e) => LoadQueueSilent();
        _refreshTimer.Start();
    }

    public bool IsAttendanceActionsVisible => CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called;

    private new void ShowError(string msg)
    {
        // Prefer modal dialog for user-facing errors (Arabic).
        ShowDialog(title: "تنبيه", message: msg, isError: true);
    }

    private new void ShowSuccess(string msg)
    {
        StatusMessage = msg;
        IsError = false;
    }

    private new void ClearStatus()
    {
        StatusMessage = string.Empty;
        IsError = false;
        IsAwaitingCompletionConfirmation = false;
    }

    private void ShowDialog(string title, string message, bool isError, bool isConfirm = false,
        string? primaryText = null, string? secondaryText = null,
        Action? primaryAction = null, Action? secondaryAction = null)
    {
        DialogTitle = title;
        DialogMessage = message;
        IsDialogError = isError;
        IsDialogConfirm = isConfirm;
        DialogPrimaryText = primaryText ?? (isConfirm ? "تأكيد" : "حسناً");
        DialogSecondaryText = secondaryText ?? "إلغاء";
        _dialogPrimaryAction = primaryAction;
        _dialogSecondaryAction = secondaryAction;
        IsDialogVisible = true;
    }

    [RelayCommand]
    private void DialogPrimary()
    {
        IsDialogVisible = false;
        var action = _dialogPrimaryAction;
        _dialogPrimaryAction = null;
        _dialogSecondaryAction = null;
        action?.Invoke();
    }

    [RelayCommand]
    private void DialogSecondary()
    {
        IsDialogVisible = false;
        var action = _dialogSecondaryAction;
        _dialogPrimaryAction = null;
        _dialogSecondaryAction = null;
        action?.Invoke();
    }

    private void UpdateCanCompleteVisit()
    {
        CanCompleteVisit = CurrentPatient != null &&
            (!string.IsNullOrWhiteSpace(Diagnosis) || !string.IsNullOrWhiteSpace(Notes) || !string.IsNullOrWhiteSpace(Prescriptions));
    }

    private void LoadQueueSilent()
    {
        try
        {
            var today = DateTime.Today;

            // Load waiting patients - use same status as LoadQueue for consistency
            var waiting = _db.QueueTickets
                .Include(q => q.Patient)
                .Where(q => q.CreatedAt.Date == today &&
                           q.Status == TicketStatus.Waiting)
                .OrderBy(q => q.TicketNumber)
                .ToList();

            // Only update if count changed or top ticket changed to avoid UI flickering
            if (WaitingPatients.Count != waiting.Count ||
                (waiting.Count > 0 && WaitingPatients.Count > 0 && waiting[0].Id != WaitingPatients[0].Id))
            {
                WaitingPatients = new ObservableCollection<QueueTicket>(waiting);
                WaitingCount = waiting.Count;
            }

            // Stats
            CompletedToday = _db.QueueTickets
                .Count(q => q.CreatedAt.Date == today && q.Status == TicketStatus.Completed);
        }
        catch (Exception)
        {
            // Fail silently in the background
        }
    }

    public void LoadQueue()
    {
        var today = DateTime.Today;

        // Load waiting patients
        var waiting = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today &&
                       q.Status == TicketStatus.Waiting)
            .OrderBy(q => q.TicketNumber)
            .ToList();

        WaitingPatients = new ObservableCollection<QueueTicket>(waiting);
        WaitingCount = waiting.Count;

        // Absent/Recall list (separate from waiting list)
        var recalls = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today && q.Status == TicketStatus.AwaitingRecall)
            .OrderBy(q => q.CalledAt ?? q.CreatedAt)
            .ThenBy(q => q.TicketNumber)
            .ToList();
        AbsentRecallPatients = new ObservableCollection<QueueTicket>(recalls);

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
            LoadVitalSigns(CurrentPatient.PatientId);
        }
        else
        {
            PatientHistory.Clear();
            ClearVitalSigns();
        }

        OnPropertyChanged(nameof(IsAttendanceActionsVisible));
    }

    // Load Vital Signs from today's visit (filled by nurse)
    private void LoadVitalSigns(Guid patientId)
    {
        var today = DateTime.Today;
        var visit = _db.Visits
            .Where(v => v.PatientId == patientId && v.VisitDate.Date == today)
            .OrderByDescending(v => v.VisitDate)
            .FirstOrDefault();

        if (visit != null)
        {
            BloodPressure = visit.BloodPressure ?? string.Empty;
            TemperatureInput = visit.Temperature?.ToString("0.0") ?? string.Empty;
            HeartRateInput = visit.HeartRate?.ToString() ?? string.Empty;
            WeightInput = visit.Weight?.ToString("0.0") ?? string.Empty;
        }
        else
        {
            ClearVitalSigns();
        }
    }

    private void ClearVitalSigns()
    {
        BloodPressure = string.Empty;
        TemperatureInput = string.Empty;
        HeartRateInput = string.Empty;
        WeightInput = string.Empty;
    }

    // Task 1: Load Patient Visit History
    private void LoadPatientHistory(Guid patientId)
    {
        var history = _db.Visits
            .Include(v => v.Doctor)
            .Include(v => v.LabTests)
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.VisitDate)
            .Take(10)
            .ToList();

        PatientHistory = new ObservableCollection<Visit>(history);
        SelectedHistoryVisit = null;
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

    private void ClearDiagnosisForm()
    {
        Diagnosis = string.Empty;
        Prescriptions = string.Empty;
        Notes = string.Empty;
        SelectedDiagnosis = null;
        SelectedMedication = null;
        LabTestName = string.Empty;
        ClearStatus();
        UpdateCanCompleteVisit();
        // Note: Vital signs are NOT cleared here as they are filled by nurses
    }

    /// <summary>Validates form data; returns false and sets Arabic error message if invalid.</summary>
    private bool ValidateVisitData(out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Diagnosis) && string.IsNullOrWhiteSpace(Notes) && string.IsNullOrWhiteSpace(Prescriptions))
        {
            errorMessage = "لا يمكن إنهاء الزيارة بدون إدخال أي معلومات. يرجى إدخال التشخيص أو الملاحظات أو الأدوية الموصوفة.";
            return false;
        }

        if (Diagnosis.Length > MaxTextLength)
        {
            errorMessage = $"التشخيص طويل جداً. الحد الأقصى {MaxTextLength} حرف.";
            return false;
        }
        if (Prescriptions.Length > MaxTextLength)
        {
            errorMessage = $"الأدوية الموصوفة طويلة جداً. الحد الأقصى {MaxTextLength} حرف.";
            return false;
        }
        if (Notes.Length > MaxTextLength)
        {
            errorMessage = $"الملاحظات طويلة جداً. الحد الأقصى {MaxTextLength} حرف.";
            return false;
        }

        // Note: Vital signs validation removed - they are filled and validated by nurses

        if (LabTestName.Length > MaxLabRequestLength)
        {
            errorMessage = $"طلب الفحوصات طويل جداً. الحد الأقصى {MaxLabRequestLength} حرف.";
            return false;
        }

        return true;
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return decimal.TryParse(value.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static int? ParseInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return int.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    [RelayCommand]
    private void CallNext()
    {
        ClearStatus();
        if (WaitingPatients.Count == 0)
        {
            ShowError("لا يوجد مرضى في قائمة الانتظار حالياً.");
            return;
        }

        if (CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called)
        {
            CurrentPatient.Status = TicketStatus.AwaitingRecall;
            CurrentPatient.CallCount++;
            _db.SaveChanges();
        }

        var next = WaitingPatients.FirstOrDefault();
        if (next == null) return;

        next.Status = TicketStatus.Called;
        next.CalledAt = DateTime.Now;
        next.CallCount++;
        _db.SaveChanges();

        LoadQueue(); // This will load vital signs for the new patient
        ClearDiagnosisForm(); // Clear form AFTER loading queue so vital signs are preserved
    }

    [RelayCommand]
    private void CallSpecific(QueueTicket ticket)
    {
        ClearStatus();
        if (ticket == null) return;

        if (CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called)
        {
            CurrentPatient.Status = TicketStatus.AwaitingRecall;
            _db.SaveChanges();
        }

        ticket.Status = TicketStatus.Called;
        ticket.CalledAt = DateTime.Now;
        ticket.CallCount++;
        _db.SaveChanges();

        LoadQueue(); // This will load vital signs for the new patient
        ClearDiagnosisForm(); // Clear form AFTER loading queue so vital signs are preserved
    }

    [RelayCommand]
    private void RecallAbsent(QueueTicket ticket)
    {
        ClearStatus();
        if (ticket == null) return;

        // Move current called patient to recall if still called
        if (CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called)
        {
            CurrentPatient.Status = TicketStatus.AwaitingRecall;
            CurrentPatient.CallCount++;
            _db.SaveChanges();
        }

        ticket.Status = TicketStatus.Called;
        ticket.CalledAt = DateTime.Now;
        ticket.CallCount++;
        _db.SaveChanges();

        LoadQueue(); // This will load vital signs for the new patient
        ClearDiagnosisForm(); // Clear form AFTER loading queue so vital signs are preserved
    }

    [RelayCommand]
    private void MarkPresent()
    {
        ClearStatus();
        if (CurrentPatient == null)
        {
            ShowError("لا يوجد مريض حالي لتأكيد حضوره.");
            return;
        }

        CurrentPatient.Status = TicketStatus.InProgress;
        _db.SaveChanges();
        LoadQueue();
        ShowSuccess("تم تأكيد حضور المريض. يمكنك الآن إدخال البيانات الطبية.");
    }

    [RelayCommand]
    private void MarkAbsent()
    {
        ClearStatus();
        if (CurrentPatient == null)
        {
            ShowError("لا يوجد مريض حالي لتسجيله كغائب.");
            return;
        }

        var ticketId = CurrentPatient.Id;
        ShowDialog(
            title: "تأكيد",
            message: "هل أنت متأكد من تسجيل المريض كغائب؟ سيتم نقله إلى قائمة الغائبين لإعادة النداء لاحقاً.",
            isError: false,
            isConfirm: true,
            primaryText: "نعم، تسجيل كغائب",
            secondaryText: "إلغاء",
            primaryAction: () =>
            {
                var t = _db.QueueTickets.Include(q => q.Patient).FirstOrDefault(q => q.Id == ticketId);
                if (t == null) { ShowDialog("تنبيه", "تعذر العثور على التذكرة.", isError: true); return; }
                t.Status = TicketStatus.AwaitingRecall;
                t.CallCount++;
                _db.SaveChanges();
                LoadQueue();
                ShowSuccess("تم نقل المريض إلى قائمة الغائبين لإعادة النداء لاحقاً.");
            });
    }

    [RelayCommand]
    private void CompleteVisit()
    {
        if (CurrentPatient == null)
        {
            ShowError("لا يوجد مريض حالي لإنهاء زيارته.");
            return;
        }

        if (!ValidateVisitData(out var validationError))
        {
            ShowError(validationError);
            return;
        }

        if (!IsAwaitingCompletionConfirmation)
        {
            IsAwaitingCompletionConfirmation = true;
            ShowDialog(
                title: "تأكيد",
                message: "هل أنت متأكد من إنهاء الزيارة وحفظ البيانات؟",
                isError: false,
                isConfirm: true,
                primaryText: "نعم، إنهاء وحفظ",
                secondaryText: "إلغاء",
                primaryAction: () =>
                {
                    // Re-run validation right before saving
                    if (!ValidateVisitData(out var validationError2))
                    {
                        ShowDialog("تنبيه", validationError2, isError: true);
                        return;
                    }
                    SaveVisitInternal();
                },
                secondaryAction: () => { IsAwaitingCompletionConfirmation = false; });
            return;
        }

        SaveVisitInternal();
    }

    private void SaveVisitInternal()
    {
        try
        {
            if (CurrentPatient == null) return;

            var doctor = _db.Users.FirstOrDefault(u => u.Role == UserRole.Doctor || u.Role == UserRole.SuperAdmin)
                         ?? _db.Users.FirstOrDefault();
            var doctorId = doctor?.Id ?? Guid.Empty;

            var visit = new Visit
            {
                PatientId = CurrentPatient.PatientId,
                DoctorId = doctorId,
                Diagnosis = string.IsNullOrWhiteSpace(Diagnosis) ? null : Diagnosis.Trim(),
                Prescriptions = string.IsNullOrWhiteSpace(Prescriptions) ? null : Prescriptions.Trim(),
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                BloodPressure = string.IsNullOrWhiteSpace(BloodPressure) ? null : BloodPressure.Trim(),
                Temperature = ParseDecimal(TemperatureInput),
                HeartRate = ParseInt(HeartRateInput),
                Weight = ParseDecimal(WeightInput),
                VisitDate = DateTime.Now,
                CreatedAt = DateTime.UtcNow
            };

            _db.Visits.Add(visit);
            _db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(LabTestName))
            {
                _db.LabTests.Add(new LabTest
                {
                    VisitId = visit.Id,
                    PatientId = CurrentPatient.PatientId,
                    TestName = LabTestName.Trim(),
                    RequestedById = doctorId,
                    RequestedAt = DateTime.UtcNow,
                    Status = LabTestStatus.Requested
                });
                _db.SaveChanges();
            }

            // Task 5: Generate Invoice for Cashier
            var invoice = new Invoice
            {
                VisitId = visit.Id,
                PatientId = CurrentPatient.PatientId,
                Amount = 150.00m,
                TaxAmount = 22.50m,
                Status = InvoiceStatus.Pending,
                CreatedById = doctorId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Invoices.Add(invoice);
            _db.SaveChanges();

            CurrentPatient.Status = TicketStatus.Completed;
            CurrentPatient.CompletedAt = DateTime.Now;
            _db.SaveChanges();

            ClearDiagnosisForm();
            LoadQueue();
            LoadStatistics();
            ShowSuccess("تمت زيارة المريض بنجاح وتم الحفظ.");
        }
        catch (Exception)
        {
            ShowDialog("خطأ", "حدث خطأ أثناء الحفظ. يرجى المحاولة مجدداً.", isError: true);
        }
        finally
        {
            IsAwaitingCompletionConfirmation = false;
            OnPropertyChanged(nameof(IsAttendanceActionsVisible));
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadQueue();
        LoadStatistics();
    }

    partial void OnStatusMessageChanged(string value) => OnPropertyChanged(nameof(IsSuccessMessage));
    partial void OnIsErrorChanged(bool value) => OnPropertyChanged(nameof(IsSuccessMessage));

    partial void OnDiagnosisChanged(string value) => UpdateCanCompleteVisit();
    partial void OnNotesChanged(string value) => UpdateCanCompleteVisit();
    partial void OnPrescriptionsChanged(string value) => UpdateCanCompleteVisit();

    partial void OnBloodPressureChanged(string value) => OnPropertyChanged(nameof(IsVitalSignsEmpty));
    partial void OnTemperatureInputChanged(string value) => OnPropertyChanged(nameof(IsVitalSignsEmpty));
    partial void OnHeartRateInputChanged(string value) => OnPropertyChanged(nameof(IsVitalSignsEmpty));
    partial void OnWeightInputChanged(string value) => OnPropertyChanged(nameof(IsVitalSignsEmpty));

    partial void OnCurrentPatientChanged(QueueTicket? value)
    {
        UpdateCanCompleteVisit();
        OnPropertyChanged(nameof(IsAttendanceActionsVisible));
    }

    // Task 2: Add selected diagnosis to diagnosis field
    partial void OnSelectedDiagnosisChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && value != "أخرى...")
            Diagnosis = value;
    }

    partial void OnSelectedMedicationChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (!string.IsNullOrWhiteSpace(Prescriptions))
                Prescriptions += "\n";
            Prescriptions += value;
        }
    }

    // Task 4: Add selected lab test to lab test name field
    partial void OnSelectedLabTestChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
            LabTestName = value;
    }

    // Task 4: Send Lab Test Request
    [RelayCommand]
    private void SendToLab()
    {
        if (CurrentPatient == null)
        {
            ShowError("لا يوجد مريض حالي لإرسال طلب فحص له.");
            return;
        }

        if (string.IsNullOrWhiteSpace(LabTestName))
        {
            ShowError("الرجاء تحديد اسم الفحص المطلوب.");
            return;
        }

        try
        {
            var doctorId = _db.Users.FirstOrDefault(u => u.Role == UserRole.Doctor || u.Role == UserRole.SuperAdmin)?.Id ?? Guid.Empty;

            // Create a temporary visit if one doesn't exist yet for today
            var today = DateTime.Today;
            var visit = _db.Visits
                .FirstOrDefault(v => v.PatientId == CurrentPatient.PatientId && v.VisitDate.Date == today);

            Guid visitId;
            if (visit == null)
            {
                // Create a placeholder visit that will be updated when CompleteVisit is called
                visit = new Visit
                {
                    PatientId = CurrentPatient.PatientId,
                    DoctorId = doctorId,
                    Diagnosis = "جاري الفحص...",
                    VisitDate = DateTime.Now,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Visits.Add(visit);
                _db.SaveChanges();
                visitId = visit.Id;
            }
            else
            {
                visitId = visit.Id;
            }

            var labTest = new LabTest
            {
                PatientId = CurrentPatient.PatientId,
                VisitId = visitId,
                TestName = LabTestName.Trim(),
                Status = LabTestStatus.Requested,
                RequestedById = doctorId,
                RequestedAt = DateTime.UtcNow
            };

            _db.LabTests.Add(labTest);
            _db.SaveChanges();

            ShowSuccess($"تم إرسال طلب فحص: {LabTestName} للمختبر بنجاح.");
            LabTestName = string.Empty;
            SelectedLabTest = null;
        }
        catch (Exception ex)
        {
            ShowError($"حدث خطأ أثناء إرسال طلب الفحص: {ex.Message}");
        }
    }
}
