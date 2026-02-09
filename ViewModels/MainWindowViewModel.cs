using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReceptionVisible))]
    [NotifyPropertyChangedFor(nameof(IsQueueVisible))]
    [NotifyPropertyChangedFor(nameof(IsDoctorVisible))]
    private string _currentView = "Reception";

    partial void OnCurrentViewChanged(string value)
    {
        if (value == "Queue")
        {
            IsSidebarToggleVisible = true;
        }
        else
        {
            IsSidebarToggleVisible = false;
            IsSidebarVisible = true; // Always show sidebar in other views
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SidebarToggleIcon))]
    private bool _isSidebarVisible = true;

    [ObservableProperty]
    private bool _isSidebarToggleVisible;

    public string SidebarToggleIcon => IsSidebarVisible ? "▶" : "◀"; // RTL: Right (Start) -> Hide with Right Arrow? Trial.

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarVisible = !IsSidebarVisible;
    }

    public bool IsReceptionVisible => CurrentView == "Reception";
    public bool IsQueueVisible => CurrentView == "Queue";
    public bool IsDoctorVisible => CurrentView == "Doctor";

    public Features.Queue.ViewModels.QueueDisplayViewModel QueueViewModel { get; } = new();

    [ObservableProperty]
    private ObservableCollection<Patient> _patients = new();


    [ObservableProperty]
    private ObservableCollection<QueueTicket> _todayQueue = new();

    // New Patient Form Fields
    [ObservableProperty]
    private string _newPatientName = string.Empty;

    [ObservableProperty]
    private string _newPatientPhone = string.Empty;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private Patient? _selectedPatient;

    [ObservableProperty]
    private int _todayPatientCount;

    [ObservableProperty]
    private int _waitingCount;

    public MainWindowViewModel()
    {
        _db = new HealthCenterDbContext();
        _db.Database.EnsureCreated();
        LoadData();
    }

    private void LoadData()
    {
        // Load all patients
        var patients = _db.Patients.OrderByDescending(p => p.CreatedAt).ToList();
        Patients = new ObservableCollection<Patient>(patients);

        // Load today's queue
        var today = DateTime.Today;
        var queue = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today)
            .OrderBy(q => q.TicketNumber)
            .ToList();
        TodayQueue = new ObservableCollection<QueueTicket>(queue);

        // Stats
        TodayPatientCount = queue.Count;
        WaitingCount = queue.Count(q => q.Status == TicketStatus.Waiting || q.Status == TicketStatus.AwaitingRecall);
    }

    [RelayCommand]
    private void AddPatient()
    {
        if (string.IsNullOrWhiteSpace(NewPatientName) || string.IsNullOrWhiteSpace(NewPatientPhone))
            return;

        var patient = new Patient
        {
            FullName = NewPatientName.Trim(),
            PhoneNumber = NewPatientPhone.Trim()
        };

        _db.Patients.Add(patient);
        _db.SaveChanges();

        Patients.Insert(0, patient);
        NewPatientName = string.Empty;
        NewPatientPhone = string.Empty;
    }

    [RelayCommand]
    private void GenerateTicket()
    {
        if (SelectedPatient == null)
            return;

        // Get next ticket number for today
        var today = DateTime.Today;
        var lastTicket = _db.QueueTickets
            .Where(q => q.CreatedAt.Date == today)
            .OrderByDescending(q => q.TicketNumber)
            .FirstOrDefault();

        var nextNumber = (lastTicket?.TicketNumber ?? 0) + 1;

        var ticket = new QueueTicket
        {
            PatientId = SelectedPatient.Id,
            TicketNumber = nextNumber,
            Status = TicketStatus.Waiting
        };

        _db.QueueTickets.Add(ticket);
        _db.SaveChanges();

        // Reload the ticket with patient info
        ticket.Patient = SelectedPatient;
        TodayQueue.Add(ticket);
        
        TodayPatientCount++;
        WaitingCount++;
    }

    [RelayCommand]
    private void SearchPatients()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            LoadData();
            return;
        }

        var query = SearchQuery.Trim().ToLower();
        var results = _db.Patients
            .Where(p => p.FullName.ToLower().Contains(query) || p.PhoneNumber.Contains(query))
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
        
        Patients = new ObservableCollection<Patient>(results);
    }

    [RelayCommand]
    private void NavigateTo(string view)
    {
        CurrentView = view;
        LoadData();
    }
}
