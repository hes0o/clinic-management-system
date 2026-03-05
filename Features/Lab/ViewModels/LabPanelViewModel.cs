using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using HealthCenter.Desktop.Services;

namespace HealthCenter.Desktop.Features.Lab.ViewModels;

public partial class LabPanelViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private readonly HealthCenterDbContext _db;
    private readonly DispatcherTimer _refreshTimer;

    [ObservableProperty] private ObservableCollection<LabTest> _requestedTests = new();
    [ObservableProperty] private LabTest? _selectedTest;
    [ObservableProperty] private string _resultNotes = string.Empty;
    [ObservableProperty] private bool _hasNoTests;

    public LabPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadTests();

        // Setup background polling every 5 seconds
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _refreshTimer.Tick += (s, e) => LoadTestsSilent();
        _refreshTimer.Start();
    }

    private void LoadTests()
    {
        try
        {
            var tests = _db.LabTests
                .Include(t => t.Patient)
                .Include(t => t.RequestedBy)
                .Where(t => t.Status != LabTestStatus.Completed)
                .OrderByDescending(t => t.RequestedAt)
                .ToList();

            RequestedTests = new ObservableCollection<LabTest>(tests);
            HasNoTests = RequestedTests.Count == 0;
        }
        catch (Exception ex)
        {
            ShowError($"خطأ في تحميل الفحوصات: {ex.Message}");
            RequestedTests = new ObservableCollection<LabTest>();
            HasNoTests = true;
        }
    }

    private void LoadTestsSilent()
    {
        try
        {
            var tests = _db.LabTests
                .Include(t => t.Patient)
                .Include(t => t.RequestedBy)
                .Where(t => t.Status != LabTestStatus.Completed)
                .OrderByDescending(t => t.RequestedAt)
                .ToList();

            if (RequestedTests.Count != tests.Count)
            {
                RequestedTests = new ObservableCollection<LabTest>(tests);
                HasNoTests = RequestedTests.Count == 0;
            }
        }
        catch (Exception) { /* Fail silently */ }
    }

    [RelayCommand]
    private void MarkCompleted()
    {
        if (SelectedTest == null)
        {
            StatusMessage = "الرجاء تحديد فحص أولاً.";
            return;
        }

        try
        {
            var tech = AuthService.Instance.CurrentUser;

            SelectedTest.Status = LabTestStatus.Completed;
            SelectedTest.ResultNotes = ResultNotes;
            SelectedTest.PerformedById = tech?.Id;
            SelectedTest.CompletedAt = DateTime.UtcNow;

            _db.SaveChanges();
            ShowSuccess($"تم تحديث نتيجة فحص: {SelectedTest.TestName}");

            ResultNotes = string.Empty;
            LoadTests();
        }
        catch (Exception ex)
        {
            ShowError($"خطأ في حفظ النتيجة: {ex.Message}");
        }
    }

}
