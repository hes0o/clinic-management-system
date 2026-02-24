using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using HealthCenter.Desktop.Services;
using HealthCenter.Desktop.Features.Auth.ViewModels;
using HealthCenter.Desktop.Features.Doctor.ViewModels;
using HealthCenter.Desktop.Features.Reception.ViewModels;
using HealthCenter.Desktop.Features.Nurse.ViewModels;
using HealthCenter.Desktop.Features.Cashier.ViewModels;
using HealthCenter.Desktop.Features.Lab.ViewModels;

namespace HealthCenter.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    // Auth
    [ObservableProperty] private bool _isAuthenticated;
    [ObservableProperty] private string _loggedInUserName = string.Empty;
    [ObservableProperty] private string _loggedInUserRole = string.Empty;

    // The current panel the authenticated user sees
    [ObservableProperty] private object? _activeDashboard;

    // Login form (always available)
    public LoginViewModel LoginViewModel { get; } = new LoginViewModel();

    public MainWindowViewModel()
    {
        _db = new HealthCenterDbContext();
        _db.Database.EnsureCreated();

        // React to login / logout
        AuthService.Instance.OnAuthStateChanged += (s, e) =>
        {
            IsAuthenticated = AuthService.Instance.IsAuthenticated;
            if (IsAuthenticated)
                LoadDashboardForCurrentUser();
            else
            {
                ActiveDashboard = null;
                LoggedInUserName = string.Empty;
                LoggedInUserRole = string.Empty;
            }
        };

        // Already logged in?
        IsAuthenticated = AuthService.Instance.IsAuthenticated;
        if (IsAuthenticated)
            LoadDashboardForCurrentUser();
    }

    /// <summary>
    /// Resolves which ViewModel to show based on the authenticated user's Role.
    /// SuperAdmin/ClinicManager get a multi-panel admin shell; others get their own panel.
    /// </summary>
    private void LoadDashboardForCurrentUser()
    {
        var user = AuthService.Instance.CurrentUser!;
        LoggedInUserName = user.FullName;
        LoggedInUserRole = GetRoleDisplayName(user.Role);

        ActiveDashboard = user.Role switch
        {
            UserRole.Doctor => new DoctorPanelViewModel(),
            UserRole.Nurse => new NursePanelViewModel(),
            UserRole.Receptionist => new ReceptionViewModel(),
            UserRole.Cashier => new CashierPanelViewModel(),
            UserRole.LabTechnician => new LabPanelViewModel(),
            // SuperAdmin and ClinicManager get the full admin shell
            _ => new AdminShellViewModel(),
        };
    }

    private static string GetRoleDisplayName(UserRole role) => role switch
    {
        UserRole.SuperAdmin => "مدير النظام",
        UserRole.ClinicManager => "مدير العيادة",
        UserRole.Doctor => "طبيب",
        UserRole.Nurse => "ممرض / ممرضة",
        UserRole.Receptionist => "موظف الاستقبال",
        UserRole.Cashier => "أمين الصندوق",
        UserRole.LabTechnician => "فني المختبر",
        _ => role.ToString()
    };

    [RelayCommand]
    private void Logout()
    {
        AuthService.Instance.Logout();
    }
}