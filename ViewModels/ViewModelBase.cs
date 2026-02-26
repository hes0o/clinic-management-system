using CommunityToolkit.Mvvm.ComponentModel;

namespace HealthCenter.Desktop.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isError;

    public void ShowError(string message)
    {
        StatusMessage = message;
        IsError = true;
    }

    public void ShowSuccess(string message)
    {
        StatusMessage = message;
        IsError = false;
    }

    public void ClearStatus()
    {
        StatusMessage = string.Empty;
        IsError = false;
    }
}
