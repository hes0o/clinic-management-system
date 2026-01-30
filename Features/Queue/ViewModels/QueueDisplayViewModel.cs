using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Desktop.Features.Queue.ViewModels;

public partial class QueueDisplayViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    [ObservableProperty]
    private int _currentTicketNumber;

    [ObservableProperty]
    private string _currentPatientName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _waitingQueue = new();

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _recentCalled = new();

    [ObservableProperty]
    private string _currentTime = DateTime.Now.ToString("HH:mm");

    [ObservableProperty]
    private string _currentDate = DateTime.Now.ToString("yyyy/MM/dd");

    public QueueDisplayViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadQueue();
        
        // Update time every minute
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (s, e) => 
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            CurrentDate = DateTime.Now.ToString("yyyy/MM/dd");
        };
        timer.Start();
    }

    public void LoadQueue()
    {
        var today = DateTime.Today;
        
        // Get currently called patient
        var currentlyBeingServed = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today && 
                       (q.Status == TicketStatus.Called || q.Status == TicketStatus.InProgress))
            .OrderByDescending(q => q.CalledAt)
            .FirstOrDefault();

        if (currentlyBeingServed != null)
        {
            CurrentTicketNumber = currentlyBeingServed.TicketNumber;
            CurrentPatientName = currentlyBeingServed.Patient?.FullName ?? "";
        }
        else
        {
            CurrentTicketNumber = 0;
            CurrentPatientName = "لا يوجد";
        }

        // Get waiting queue
        var waiting = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today && 
                       (q.Status == TicketStatus.Waiting || q.Status == TicketStatus.AwaitingRecall))
            .OrderBy(q => q.TicketNumber)
            .Take(5)
            .ToList();
        WaitingQueue = new ObservableCollection<QueueTicket>(waiting);

        // Get recently called (last 3)
        var recent = _db.QueueTickets
            .Include(q => q.Patient)
            .Where(q => q.CreatedAt.Date == today && q.Status == TicketStatus.Completed)
            .OrderByDescending(q => q.CompletedAt)
            .Take(3)
            .ToList();
        RecentCalled = new ObservableCollection<QueueTicket>(recent);
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadQueue();
    }
}
