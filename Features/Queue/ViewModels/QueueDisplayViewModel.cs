using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Features.Reception.Models;
using HealthCenter.Desktop.Services;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace HealthCenter.Desktop.Features.Queue.ViewModels;

public class DisplayPatient
{
    public string TicketNumber { get; set; } = "";
    public string MaskedName { get; set; } = "";
    public Patient? OriginalPatient { get; set; }
}

public partial class QueueDisplayViewModel : ObservableObject
{
    [ObservableProperty] private string _currentTicketDisplay = "--";
    [ObservableProperty] private string _currentNameDisplay = "بانتظار استقبال المرضى";

    [ObservableProperty] private bool _showPopup = false;
    [ObservableProperty] private string _popupTicketDisplay = "";
    [ObservableProperty] private string _popupNameDisplay = "";

    [ObservableProperty] private ObservableCollection<DisplayPatient> _waitingListDisplay = new();
    [ObservableProperty] private int _patientsAheadCount;
    [ObservableProperty] private int _estimatedWaitTimeMinutes;

    [ObservableProperty] private string _currentDateHijri = string.Empty;
    [ObservableProperty] private string _currentDateGregorian = string.Empty;
    [ObservableProperty] private string _currentTime = string.Empty;

    [ObservableProperty] private bool _isMuted = false;
    [ObservableProperty] private string _muteIcon = "🔊";

    [ObservableProperty] private string _currentHealthTip = string.Empty;
    private readonly List<string> _healthTips = new()
    {
        "احرص على تناول الخضروات والفواكه الطازجة يومياً.",
        "شرب 8 أكواب من الماء يومياً يحافظ على نشاطك وصحتك.",
        "غسل اليدين بانتظام يقي من معظم الأمراض الفيروسية.",
        "تجنب السهر الطويل، فالنوم الكافي يعزز مناعة الجسم."
    };
    private int _currentTipIndex = 0;

    public QueueDisplayViewModel()
    {
        StartTimers();
        RefreshQueue();
        ClinicStore.Instance.QueueChanged += () => Dispatcher.UIThread.Post(RefreshQueue);
        ClinicStore.Instance.OnPatientCalled += HandlePatientCalled;
    }

    private string MaskName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "";
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var maskedParts = parts.Select(p => 
        {
            if (p.Length <= 1) return p + "***"; 
            return p.Substring(0, 2) + "***";
        });
        return string.Join(" ", maskedParts);
    }

    private void RefreshQueue()
    {
        WaitingListDisplay.Clear();
        foreach (var p in ClinicStore.Instance.TodayActiveQueue.Take(7))
        {
            WaitingListDisplay.Add(new DisplayPatient
            {
                TicketNumber = p.TicketNumber.ToString("D3"),
                MaskedName = MaskName(p.FullName),
                OriginalPatient = p
            });
        }
        PatientsAheadCount = ClinicStore.Instance.TodayActiveQueue.Count;
        EstimatedWaitTimeMinutes = PatientsAheadCount * 10; 
    }

    private async void HandlePatientCalled(Patient patient)
    {
        var ticket = patient.TicketNumber.ToString("D3");
        var maskedName = MaskName(patient.FullName);

        CurrentTicketDisplay = ticket;
        CurrentNameDisplay = maskedName;
        PopupTicketDisplay = ticket;
        PopupNameDisplay = maskedName;
        ShowPopup = true;

        // 🔊 النظام الصوتي المحدث (بدون أي تحذيرات صفراء)
        if (!IsMuted)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    // التحقق من أن النظام ويندوز لتجنب أعطال المنصات الأخرى
                    if (OperatingSystem.IsWindows())
                    {
                        using var synthesizer = new SpeechSynthesizer();
                        synthesizer.SetOutputToDefaultAudioDevice();
                        synthesizer.Speak($"الرقم {patient.TicketNumber}، تَفَضَّلْ إِلَى عِيَادَةِ الطَّبِيب.");
                    }
                }
                catch { /* تجاهل الخطأ */ }
            });
        }

        await Task.Delay(8000); 
        Dispatcher.UIThread.Post(() => ShowPopup = false);
    }

    [RelayCommand]
    private void ToggleMute()
    {
        IsMuted = !IsMuted;
        MuteIcon = IsMuted ? "🔇" : "🔊";
    }

    private void StartTimers()
    {
        var clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        clockTimer.Tick += (s, e) =>
        {
            var now = DateTime.Now;
            CurrentTime = now.ToString("hh:mm tt", new CultureInfo("ar-SA"));
            CurrentDateGregorian = now.ToString("dd MMMM yyyy", new CultureInfo("ar-EG")) + " م";
            var hijriCalendar = new HijriCalendar();
            CurrentDateHijri = $"{hijriCalendar.GetDayOfMonth(now)} {now.ToString("MMMM", new CultureInfo("ar-SA"))} {hijriCalendar.GetYear(now)} هـ";
        };
        clockTimer.Start();

        CurrentHealthTip = _healthTips[0];
        var tipsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
        tipsTimer.Tick += (s, e) =>
        {
            _currentTipIndex = (_currentTipIndex + 1) % _healthTips.Count;
            CurrentHealthTip = _healthTips[_currentTipIndex];
        };
        tipsTimer.Start();
    }
}