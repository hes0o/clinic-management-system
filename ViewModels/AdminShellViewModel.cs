using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using HealthCenter.Desktop.Features.Doctor.ViewModels;
using HealthCenter.Desktop.Features.Reception.ViewModels;
using HealthCenter.Desktop.Features.Nurse.ViewModels;
using HealthCenter.Desktop.Features.Cashier.ViewModels;
using HealthCenter.Desktop.Features.Lab.ViewModels;
using HealthCenter.Desktop.Services;

namespace HealthCenter.Desktop.ViewModels;

/// <summary>
/// The Admin Shell contains tab pages for all available dashboards.
/// Used by SuperAdmin and ClinicManager roles.
/// </summary>
public partial class AdminShellViewModel : ViewModelBase
{
    public DoctorPanelViewModel DoctorPanel { get; } = new DoctorPanelViewModel();
    public ReceptionViewModel ReceptionPanel { get; } = new ReceptionViewModel();
    public NursePanelViewModel NursePanel { get; } = new NursePanelViewModel();
    public CashierPanelViewModel CashierPanel { get; } = new CashierPanelViewModel();
    public LabPanelViewModel LabPanel { get; } = new LabPanelViewModel();
}
