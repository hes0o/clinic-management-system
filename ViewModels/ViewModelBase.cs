using CommunityToolkit.Mvvm.ComponentModel;

namespace HealthCenter.Desktop.ViewModels;

public class ViewModelBase : ObservableObject
{
    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        protected set
        {
            if (SetProperty(ref _statusMessage, value))
                OnPropertyChanged(nameof(IsSuccess));
        }
    }

    private bool _isError;
    public bool IsError
    {
        get => _isError;
        protected set
        {
            if (SetProperty(ref _isError, value))
                OnPropertyChanged(nameof(IsSuccess));
        }
    }

    /// <summary>True only when there is a non-empty message AND it is a success (not an error).
    /// Bind the green success border's IsVisible to this — avoids empty banner glitch.</summary>
    public bool IsSuccess => !IsError && !string.IsNullOrEmpty(StatusMessage);

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
