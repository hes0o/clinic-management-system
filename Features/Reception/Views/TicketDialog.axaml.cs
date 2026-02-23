using Avalonia.Controls;
using Avalonia.Interactivity;
using HealthCenter.Desktop.Database.Entities;
using HealthCenter.Desktop.Features.Reception.ViewModels;
using System;

namespace HealthCenter.Desktop.Features.Reception.Views;

public partial class TicketDialog : Window
{
    public TicketDialog()
    {
        InitializeComponent();
    }

    public TicketDialog(Patient patient, int ticketNumber, int waitingCount) : this()
    {
        var ticketNumTxt = this.FindControl<TextBlock>("TicketNumberTxt");
        var patientNameTxt = this.FindControl<TextBlock>("PatientNameTxt");
        var dateTxt = this.FindControl<TextBlock>("DateTxt");
        var timeTxt = this.FindControl<TextBlock>("TimeTxt");
        var waitingCountTxt = this.FindControl<TextBlock>("WaitingCountTxt");

        if (ticketNumTxt != null) ticketNumTxt.Text = ticketNumber.ToString();
        if (patientNameTxt != null) patientNameTxt.Text = patient.FullName;
        if (dateTxt != null) dateTxt.Text = DateTime.Now.ToString("yyyy/MM/dd");
        if (timeTxt != null) timeTxt.Text = DateTime.Now.ToString("hh:mm tt");
        if (waitingCountTxt != null) waitingCountTxt.Text = $"أمامك: {Math.Max(0, waitingCount - 1)} مرضى";
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}