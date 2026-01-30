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

    public DoctorPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadQueue();
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
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadQueue();
    }
}
