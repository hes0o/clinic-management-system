using CommunityToolkit.Mvvm.ComponentModel;

namespace HealthCenter.Desktop.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSuccess))]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSuccess))]
    private bool _isError;

    public bool IsSuccess => !IsError && !string.IsNullOrEmpty(StatusMessage);

    protected void ShowError(string msg)
    {
        StatusMessage = msg;
        IsError = true;
    }

    protected void ShowSuccess(string msg)
    {
        StatusMessage = msg;
        IsError = false;
    }

    protected void ClearStatus()
    {
        StatusMessage = string.Empty;
        IsError = false;
    }
}
