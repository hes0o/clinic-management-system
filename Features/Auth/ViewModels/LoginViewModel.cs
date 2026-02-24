using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Services;
using System.Linq;
using System;

namespace HealthCenter.Desktop.Features.Auth.ViewModels;

public partial class LoginViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel()
    {
        _db = new HealthCenterDbContext();
    }

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "الرجاء إدخال اسم المستخدم وكلمة المرور.";
            return;
        }

        try
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == Username && u.IsActive);

            if (user == null || user.PasswordHash != Password) // In production, hash the input!
            {
                ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                return;
            }

            // Success
            AuthService.Instance.Login(user);
        }
        catch (Exception ex)
        {
            ErrorMessage = "حدث خطأ أثناء تسجيل الدخول: " + ex.Message;
        }
    }
}
