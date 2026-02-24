using Avalonia.Controls;
using Avalonia.Interactivity;
using HealthCenter.Desktop.Features.Queue.Views; // استدعاء شاشة الانتظار

namespace HealthCenter.Desktop.Features.Doctor.Views;

public partial class DoctorView : UserControl
{
    public DoctorView()
    {
        InitializeComponent();
    }

    // هذه الدالة تفتح شاشة الانتظار كنافذة مستقلة عند الضغط على الزر
    private void OpenDisplay_Click(object? sender, RoutedEventArgs e)
    {
        var displayWindow = new QueueDisplayWindow();
        displayWindow.Show(); // تفتح النافذة بحيث يمكنك سحبها لشاشة التلفاز
    }
}