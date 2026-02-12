using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace HealthCenter.Desktop.Views.Components;

public partial class LoadingSpinner : UserControl
{
    private DispatcherTimer? _timer;
    private RotateTransform? _rotate;
    private double _angle;

    public LoadingSpinner()
    {
        InitializeComponent();

        // نجيب الـ Path وبعدين نطلع الـ RotateTransform منه
        var path = this.FindControl<Avalonia.Controls.Shapes.Path>("SpinnerPath");
        _rotate = path?.RenderTransform as RotateTransform;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        _timer.Tick += (_, _) =>
        {
            _angle = (_angle + 6) % 360;
            if (_rotate != null)
                _rotate.Angle = _angle;
        };

        AttachedToVisualTree += (_, _) => _timer.Start();
        DetachedFromVisualTree += (_, _) => _timer.Stop();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
