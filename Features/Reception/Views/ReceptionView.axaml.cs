using Avalonia.Controls;
using HealthCenter.Desktop.Features.Reception.ViewModels;

namespace HealthCenter.Desktop.Features.Reception.Views;

public partial class ReceptionView : UserControl
{
    public ReceptionView()
    {
        InitializeComponent();
        DataContext = new ReceptionViewModel();
    }
}