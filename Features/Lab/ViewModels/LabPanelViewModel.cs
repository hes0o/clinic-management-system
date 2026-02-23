using System;
using System.Collections.ObjectModel;
using System.Linq;
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

    [ObservableProperty] private ObservableCollection<LabTest> _requestedTests = new();
    [ObservableProperty] private LabTest? _selectedTest;
    [ObservableProperty] private string _resultNotes = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public LabPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadTests();
    }

    private void LoadTests()
    {
        var tests = _db.LabTests
            .Include(t => t.Patient)
            .Include(t => t.RequestedBy)
            .Where(t => t.Status != LabTestStatus.Completed)
            .OrderByDescending(t => t.RequestedAt)
            .ToList();

        RequestedTests = new ObservableCollection<LabTest>(tests);
    }

    [RelayCommand]
    private void MarkCompleted()
    {
        if (SelectedTest == null)
        {
            StatusMessage = "الرجاء تحديد فحص أولاً.";
            return;
        }

        var tech = AuthService.Instance.CurrentUser;

        SelectedTest.Status = LabTestStatus.Completed;
        SelectedTest.ResultNotes = ResultNotes;
        SelectedTest.PerformedById = tech?.Id;
        SelectedTest.CompletedAt = DateTime.UtcNow;

        _db.SaveChanges();
        StatusMessage = $"تم تحديث نتيجة فحص: {SelectedTest.TestName}";

        ResultNotes = string.Empty;
        LoadTests();
    }

    [RelayCommand]
    private void RefreshTests()
    {
        LoadTests();
        StatusMessage = "تم تحديث قائمة الفحوصات.";
    }
}
