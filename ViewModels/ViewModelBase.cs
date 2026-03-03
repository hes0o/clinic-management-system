using CommunityToolkit.Mvvm.ComponentModel;

namespace HealthCenter.Desktop.ViewModels;

public class ViewModelBase : ObservableObject
{
    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        protected set => SetProperty(ref _statusMessage, value);
    }

    private bool _isError;
    public bool IsError
    {
        get => _isError;
        protected set => SetProperty(ref _isError, value);
    }

    protected void ShowError(string message)
    {
        StatusMessage = message;
        IsError = true;
    }

    protected void ShowSuccess(string message)
    {
        StatusMessage = message;
        IsError = false;
    }
}
