using Avalonia.Controls;
using Avalonia.Interactivity;
using HealthCenter.Desktop.Features.Reception.Models;
using HealthCenter.Desktop.Features.Reception.ViewModels; 
using HealthCenter.Desktop.Services;
using System;

namespace HealthCenter.Desktop.Features.Reception.Views;

public partial class TicketDialog : Window
{
    private readonly Patient _patient;
    private readonly ReceptionViewModel _viewModel; 

    public TicketDialog()
    {
        InitializeComponent();
        _patient = new Patient();
        _viewModel = new ReceptionViewModel();
    }

    public TicketDialog(Patient patient, ReceptionViewModel vm) : this()
    {
        _patient = patient;
        _viewModel = vm;
        this.DataContext = _viewModel; 

        var ticketNumTxt = this.FindControl<TextBlock>("TicketNumberTxt");
        var patientNameTxt = this.FindControl<TextBlock>("PatientNameTxt");
        var dateTxt = this.FindControl<TextBlock>("DateTxt");
        var timeTxt = this.FindControl<TextBlock>("TimeTxt");
        var waitingCountTxt = this.FindControl<TextBlock>("WaitingCountTxt");

        if (ticketNumTxt != null) ticketNumTxt.Text = patient.TicketNumber.ToString();
        if (patientNameTxt != null) patientNameTxt.Text = patient.FullName;
        if (dateTxt != null) dateTxt.Text = DateTime.Now.ToString("yyyy/MM/dd");
        if (timeTxt != null) timeTxt.Text = DateTime.Now.ToString("hh:mm tt");
        
        if (waitingCountTxt != null)
        {
            int peopleAhead = ClinicStore.Instance.WaitingCount - 1;
            if (peopleAhead < 0) peopleAhead = 0;
            waitingCountTxt.Text = $"أمامك: {peopleAhead} مرضى";
        }
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        // ✅ التحقق من النظام قبل الطباعة
        if (OperatingSystem.IsWindows())
        {
            int waiting = ClinicStore.Instance.WaitingCount - 1;
            if (waiting < 0) waiting = 0;

            string ip = _viewModel.PrinterIp;
            int port = _viewModel.PrinterPort;

            var netPrinter = new NetworkPrinter(ip, port);
            netPrinter.PrintTicket(_patient, waiting);
        }
        else
        {
            // طباعة رسالة في التيرمينال إذا لم يكن ويندوز
            Console.WriteLine("Printing is currently supported on Windows only.");
        }
        
        this.Close(); 
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}