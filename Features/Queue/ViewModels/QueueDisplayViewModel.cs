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

    // Sidebar Properties (Mocked for UI consistency)
    [ObservableProperty]
    private int _todayPatientCount = 12; // Mock value

    [ObservableProperty]
    private int _waitingCount;

    [RelayCommand]
    private void NavigateTo(string view)
    {
        // No-op for visual demo
    }

    [ObservableProperty]
    private string _currentPatientName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _waitingQueue = new();

    [ObservableProperty]
    private ObservableCollection<QueueTicket> _recentCalled = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentDateGregorian))]
    private string _currentTime = DateTime.Now.ToString("HH:mm");

    [ObservableProperty]
    private string _currentDate = DateTime.Now.ToString("yyyy/MM/dd");
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentDateHijri))]
    private string _hijriDate = string.Empty;
    
    [ObservableProperty]
    private string _clinicHours = "8:00 ص - 4:00 م";
    
    [ObservableProperty]
    private string _currentHealthTip = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PatientsAheadCount))]
    private int _patientsAhead = 0;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstimatedWaitTimeMinutes))]
    private int _estimatedWaitMinutes = 0;
    
    [ObservableProperty]
    private double _averageServiceTimeMinutes = 10.0;
    
    [ObservableProperty]
    private QueueTicket? _currentTicket;

    // View Aliases for compatibility with new UI Key
    public ObservableCollection<QueueTicket> WaitingList => WaitingQueue;
    public string CurrentDateHijri => HijriDate;
    public DateTime CurrentDateGregorian => DateTime.Now;
    public int PatientsAheadCount => PatientsAhead;
    public int EstimatedWaitTimeMinutes => EstimatedWaitMinutes;

    [ObservableProperty]
    private bool _isMuted = false;

    private static readonly List<string> HealthTips = new()
    {
        "اشرب 8 أكواب من الماء يومياً للحفاظ على صحتك",
        "المشي 30 دقيقة يومياً يحسن صحة القلب",
        "النوم 7-8 ساعات ضروري لصحة جيدة",
        "تناول الخضروات والفواكه الطازجة يومياً",
        "اغسل يديك بانتظام للوقاية من الأمراض",
        "ممارسة الرياضة تقوي المناعة وتحسن المزاج",
        "تفاحة في اليوم تغنيك عن الطبيب",
        "قلل من تناول المشروبات الغازية والسكريات",
        "التعرض لأشعة الشمس مصدر فيتامين د",
        "قلل من وقت الشاشات قبل النوم لنوم أفضل"
    };

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
            CurrentTicket = currentlyBeingServed;
            CurrentTicketNumber = currentlyBeingServed.TicketNumber;
            CurrentPatientName = currentlyBeingServed.Patient?.FullName ?? "";
        }
        else
        {
            CurrentTicket = null;
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
