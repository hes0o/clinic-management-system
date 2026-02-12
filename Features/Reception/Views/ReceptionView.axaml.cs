using Avalonia.Controls;
using HealthCenter.Desktop.Features.Reception.ViewModels;
using HealthCenter.Desktop.Features.Reception.Models;

namespace HealthCenter.Desktop.Features.Reception.Views;

public partial class ReceptionView : UserControl
{
    public ReceptionView()
    {
        InitializeComponent();

        // 1. إنشاء الـ ViewModel
        var vm = new ReceptionViewModel();
        
        // 2. الاستماع لحدث الطباعة
        vm.OnPrintRequest += (patient) => 
        {
            // عند طلب الطباعة، نفتح النافذة الجديدة
            var dialog = new TicketDialog(patient, vm); 
    
            var topLevel = TopLevel.GetTopLevel(this) as Window;
            if (topLevel != null)
            {
                dialog.ShowDialog(topLevel);
            }
        };

        // 3. تعيين الـ DataContext
        DataContext = vm;
    }
}