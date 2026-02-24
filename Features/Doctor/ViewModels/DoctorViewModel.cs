using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Features.Reception.Models;
using HealthCenter.Desktop.Services;
using Avalonia.Threading;

namespace HealthCenter.Desktop.Features.Doctor.ViewModels;

public partial class DoctorViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Patient> _waitingPatients = new();

    // 🌟 نظام تحديد المريض من الطابور
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedWaitingPatient))]
    private Patient? _selectedWaitingPatient;

    public bool HasSelectedWaitingPatient => SelectedWaitingPatient != null;

    // 🌟 المريض الحالي في العيادة
    private Patient? _currentPatient;
    public Patient? CurrentPatient
    {
        get => _currentPatient;
        set
        {
            SetProperty(ref _currentPatient, value);
            OnPropertyChanged(nameof(HasCurrentPatient));
            OnPropertyChanged(nameof(IsClinicEmpty));
        }
    }

    public bool HasCurrentPatient => CurrentPatient != null;
    public bool IsClinicEmpty => CurrentPatient == null;

    [ObservableProperty] private string _diagnosis = string.Empty;
    [ObservableProperty] private string _prescriptions = string.Empty;
    [ObservableProperty] private string _notes = string.Empty;

    [ObservableProperty] private int _waitingCount;
    [ObservableProperty] private int _completedToday;

    [ObservableProperty] private ObservableCollection<VisitRecord> _patientHistory = new();
    [ObservableProperty] private bool _isHistoryExpanded = false;

    public ObservableCollection<string> CommonDiagnoses { get; } = new()
    {
        "نزلة برد", "إنفلونزا", "صداع", "ألم المعدة", "التهاب الحلق",
        "ارتفاع ضغط الدم", "السكري", "حساسية", "أخرى..."
    };
    [ObservableProperty] private string? _selectedDiagnosis;

    public ObservableCollection<string> CommonMedications { get; } = new()
    {
        "باراسيتامول 500mg - مرتين يومياً", "أموكسيسيلين 500mg - ثلاث مرات يومياً",
        "إيبوبروفين 400mg - عند الحاجة", "أوميبرازول 20mg - قبل الفطور",
        "أسبرين 100mg - مرة يومياً", "فيتامين د 1000 وحدة - يومياً"
    };
    [ObservableProperty] private string? _selectedMedication;

    [ObservableProperty] private string _bloodPressure = string.Empty;
    [ObservableProperty] private decimal? _temperature;
    [ObservableProperty] private int? _heartRate;
    [ObservableProperty] private decimal? _weight;

    [ObservableProperty] private int _todayPatients;
    [ObservableProperty] private int _weekPatients;
    [ObservableProperty] private int _monthPatients;

    public DoctorViewModel()
    {
        // 🌟 إجبار الواجهة على قراءة الحالة الصحيحة لمنع التداخل
        CurrentPatient = null; 

        LoadQueue();
        LoadStatistics();
        
        // 🌟 تحديث الطابور فوراً وبشكل قوي عند إرسال مريض من الاستقبال
        ClinicStore.Instance.QueueChanged += () => 
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadQueue();
            });
        };
    }

    public void LoadQueue()
    {
        var list = ClinicStore.Instance.TodayActiveQueue.ToList();
        WaitingPatients.Clear();
        foreach (var p in list)
        {
            WaitingPatients.Add(p);
        }
        WaitingCount = WaitingPatients.Count;
    }

    private void LoadStatistics()
    {
        var today = DateTime.Today;
        
        // حساب بداية الأسبوع (بافتراض الأحد هو أول أيام الأسبوع)
        var weekStart = today.AddDays(-(int)today.DayOfWeek); 
        
        // حساب بداية الشهر
        var monthStart = new DateTime(today.Year, today.Month, 1);

        // جلب كل المرضى من المخزن المركزي
        var allPatients = ClinicStore.Instance.AllPatients;

        // حساب الأعداد الحقيقية بناءً على تاريخ تسجيل المريض
        TodayPatients = allPatients.Count(p => p.RegistrationDate.Date == today);
        WeekPatients = allPatients.Count(p => p.RegistrationDate.Date >= weekStart);
        MonthPatients = allPatients.Count(p => p.RegistrationDate.Date >= monthStart);
    }

    private void LoadPatientHistory(string patientId)
    {
        PatientHistory.Clear();
        PatientHistory.Add(new VisitRecord 
        { 
            VisitDate = DateTime.Now.AddDays(-14),
            Diagnosis = "مراجعة دورية - حالة مستقرة",
            Prescriptions = "فيتامين د 1000 وحدة",
            Notes = "نصح المريض بممارسة الرياضة"
        });
    }

    // 🌟 الزر الجديد: نداء المريض الذي تم تحديده بالماوس
    [RelayCommand]
    private void CallSelected()
    {
        if (SelectedWaitingPatient == null) return;

        CurrentPatient = SelectedWaitingPatient;
        ClinicStore.Instance.CallPatient(SelectedWaitingPatient); 
        
        SelectedWaitingPatient = null; // تفريغ التحديد بعد النداء
        LoadQueue();
        LoadPatientHistory(CurrentPatient.Id);
    }

    [RelayCommand]
    private void MarkAbsent()
    {
        if (CurrentPatient == null) return;
        ClinicStore.Instance.SendToDoctorQueue(CurrentPatient);
        CurrentPatient = null;
        LoadQueue();
    }

    [RelayCommand]
    private void CompleteVisit()
    {
        if (CurrentPatient == null) return;
        CompletedToday++;
        CurrentPatient = null;
        Diagnosis = string.Empty; Prescriptions = string.Empty; Notes = string.Empty;
        BloodPressure = string.Empty; Temperature = null; HeartRate = null; Weight = null;
        SelectedDiagnosis = null; SelectedMedication = null;
        LoadQueue();
        LoadStatistics();
    }

    partial void OnSelectedDiagnosisChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && value != "أخرى...") Diagnosis = value;
    }

    partial void OnSelectedMedicationChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (!string.IsNullOrWhiteSpace(Prescriptions)) Prescriptions += "\n";
            Prescriptions += value;
        }
    }
}

public class VisitRecord
{
    public DateTime VisitDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescriptions { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}