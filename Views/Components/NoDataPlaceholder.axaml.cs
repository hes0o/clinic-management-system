using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HealthCenter.Desktop.Views.Components;

public partial class NoDataPlaceholder : UserControl
{
    public NoDataPlaceholder()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
