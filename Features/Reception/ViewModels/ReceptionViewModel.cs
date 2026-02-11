using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using HealthCenter.Desktop.Features.Reception.Models;
using Avalonia.Media; 
using System.Linq;
using System.Collections.Generic;
using System;
using HealthCenter.Desktop.Services;

namespace HealthCenter.Desktop.Features.Reception.ViewModels;

public partial class ReceptionViewModel : ObservableObject
{
    public event Action<Patient>? OnPrintRequest;

    // --- إعدادات اللغة ---
    [ObservableProperty] private bool _isArabic = true;
    [ObservableProperty] private FlowDirection _currentFlowDirection = FlowDirection.RightToLeft;
    
    // إعدادات الطابعة الشبكية
    [ObservableProperty] private string _printerIp = "192.168.1.200"; 
    [ObservableProperty] private int _printerPort = 9100;

    // العناوين والنصوص
    [ObservableProperty] private string _tabRegisterHeader = "تسجيل جديد";
    [ObservableProperty] private string _tabSearchHeader = "بحث وتذاكر";
    [ObservableProperty] private string _titleText = "تسجيل مريض جديد";
    [ObservableProperty] private string _nameLabel = "الاسم الكامل";
    [ObservableProperty] private string _phoneLabel = "رقم الهاتف";
    [ObservableProperty] private string _nationalIdLabel = "الرقم الوطني";
    [ObservableProperty] private string _genderLabel = "الجنس";
    [ObservableProperty] private string _bloodTypeLabel = "زمرة الدم (اختياري)";
    [ObservableProperty] private string _birthDateLabel = "تاريخ الميلاد";
    [ObservableProperty] private string _saveButtonText = "حفظ البيانات";
    [ObservableProperty] private string _searchPlaceholder = "ابحث بالاسم أو الرقم...";
    [ObservableProperty] private string _searchBtnText = "بحث";
    [ObservableProperty] private string _langButtonText = "English";

    // --- بيانات الإدخال ---
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddPatientCommand))]
    private string _patientName = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddPatientCommand))]
    private string _phoneNumber = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddPatientCommand))]
    private string _nationalId = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddPatientCommand))] 
    private GenderOption? _selectedGender;
    
    [ObservableProperty] private string? _selectedBloodType;
    
    // ✅ التعديل الأول: تغيير النوع إلى DateTime? ليتوافق مع CalendarDatePicker
    [ObservableProperty] private DateTime? _selectedBirthDate;

    // --- وضع التعديل ---
    private string? _editingPatientId = null; 
    [ObservableProperty] private bool _isEditMode = false;

    // القوائم
    public ObservableCollection<GenderOption> GenderOptions { get; } = new();
    public ObservableCollection<string> BloodTypes { get; } = new()
    {
        "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
    };

    // --- بيانات البحث ---
    [ObservableProperty] private string _searchQuery = string.Empty;
    public ObservableCollection<Patient> SearchResults { get; } = new();
    
    // رسائل التنبيه
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    
    [ObservableProperty] private int _selectedTabIndex = 0;

    public ReceptionViewModel()
    {
        UpdateUiLanguage();
        RefreshSearchResults();
    }

    partial void OnSearchQueryChanged(string value) => RefreshSearchResults();

    [RelayCommand]
    private void Search() => RefreshSearchResults();

    [RelayCommand]
    private void SwitchLanguage()
    {
        IsArabic = !IsArabic;
        UpdateUiLanguage();
    }

    private void UpdateUiLanguage()
    {
        if (IsArabic)
        {
            CurrentFlowDirection = FlowDirection.RightToLeft;
            NationalIdLabel = "الرقم الوطني";
            SaveButtonText = IsEditMode ? "تعديل البيانات" : "حفظ البيانات";
            SearchPlaceholder = "ابحث بالاسم أو الرقم...";
            SearchBtnText = "بحث";
            GenderOptions.Clear(); 
            GenderOptions.Add(new GenderOption("ذكر", "M")); 
            GenderOptions.Add(new GenderOption("أنثى", "F"));
        }
        else
        {
            CurrentFlowDirection = FlowDirection.LeftToRight;
            NationalIdLabel = "National ID";
            SaveButtonText = IsEditMode ? "Update Data" : "Save Data";
            SearchPlaceholder = "Search Name or ID...";
            SearchBtnText = "Search";
            GenderOptions.Clear(); 
            GenderOptions.Add(new GenderOption("Male", "M")); 
            GenderOptions.Add(new GenderOption("Female", "F"));
        }
    }

    private bool CanAddPatient() => 
        !string.IsNullOrWhiteSpace(PatientName) && PatientName.Length >= 3 &&
        !string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber.Length == 10 &&
        !string.IsNullOrWhiteSpace(NationalId) && NationalId.Length >= 10 && 
        SelectedGender != null;

    [RelayCommand(CanExecute = nameof(CanAddPatient))]
    private void AddPatient()
    {
        if (IsEditMode && _editingPatientId != null)
        {
            // --- تعديل ---
            var existing = ClinicStore.Instance.AllPatients.FirstOrDefault(p => p.Id == _editingPatientId);
            if (existing != null)
            {
                existing.FullName = PatientName;
                existing.PhoneNumber = PhoneNumber;
                existing.NationalId = NationalId;
                existing.Gender = SelectedGender?.Value ?? "M";
                existing.BloodType = SelectedBloodType ?? "";
                
                // ✅ التعديل الثاني: تعيين التاريخ مباشرة بدون .DateTimeOffset
                existing.BirthDate = SelectedBirthDate; 
                
                SuccessMessage = IsArabic ? "تم تعديل البيانات بنجاح!" : "Data Updated!";
            }
            IsEditMode = false;
            _editingPatientId = null;
            SaveButtonText = IsArabic ? "حفظ البيانات" : "Save Data";
        }
        else
        {
            // --- إضافة جديدة ---
            int nextTicketNumber = ClinicStore.Instance.TodayCount + 1;

            var newPatient = new Patient 
            { 
                FullName = PatientName, 
                PhoneNumber = PhoneNumber, 
                NationalId = NationalId,
                Gender = SelectedGender?.Value ?? "M",
                BloodType = SelectedBloodType ?? "",
                
                // ✅ التعديل الثالث: تعيين التاريخ مباشرة
                BirthDate = SelectedBirthDate,
                
                TicketNumber = nextTicketNumber,
                RegistrationDate = DateTime.Now
            };
            
            ClinicStore.Instance.AddPatientToQueue(newPatient);
            SuccessMessage = IsArabic ? $"تم الحفظ! رقم التذكرة: {nextTicketNumber}" : $"Saved! Ticket #{nextTicketNumber}";
        }

        PatientName = ""; PhoneNumber = ""; NationalId = ""; SelectedGender = null; 
        SelectedBloodType = null; SelectedBirthDate = null;
        RefreshSearchResults();
    }

    [RelayCommand]
    private void EditPatient(Patient patient)
    {
        PatientName = patient.FullName;
        PhoneNumber = patient.PhoneNumber;
        NationalId = patient.NationalId;
        SelectedGender = GenderOptions.FirstOrDefault(g => g.Value == patient.Gender);
        SelectedBloodType = patient.BloodType;
        
        // ✅ التعديل الرابع: قراءة التاريخ مباشرة
        SelectedBirthDate = patient.BirthDate;

        IsEditMode = true;
        _editingPatientId = patient.Id;
        SaveButtonText = IsArabic ? "تعديل البيانات" : "Update Data";
        SelectedTabIndex = 0; 
    }

    [RelayCommand]
    private void DeletePatient(Patient patient)
    {
        ClinicStore.Instance.RemovePatient(patient);
        RefreshSearchResults();
    }

    private void RefreshSearchResults()
    {
        SearchResults.Clear();
        var q = SearchQuery.ToLower().Trim();
        var source = ClinicStore.Instance.AllPatients;

        var query = string.IsNullOrWhiteSpace(q) 
            ? source 
            : source.Where(p => 
                p.FullName.ToLower().Contains(q) || 
                p.PhoneNumber.Contains(q) || 
                p.NationalId.Contains(q));
        
        foreach(var p in query.OrderBy(p => p.FullName)) 
        {
            SearchResults.Add(p);
        }
    }

    [RelayCommand]
    private void PrintTicket(Patient patient)
    {
        OnPrintRequest?.Invoke(patient);
    }
}