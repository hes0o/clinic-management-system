using System;
using HealthCenter.Desktop.Database.Entities;

namespace HealthCenter.Desktop.Services;

/// <summary>
/// Singleton service to hold the currently authenticated User state
/// </summary>
public class AuthService
{
    private static AuthService? _instance;
    public static AuthService Instance => _instance ??= new AuthService();

    public User? CurrentUser { get; private set; }
    
    // Event that UI or ViewModels can listen to when user logs in/out
    public event EventHandler? OnAuthStateChanged;

    private AuthService() { }

    public void Login(User user)
    {
        CurrentUser = user;
        OnAuthStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Logout()
    {
        CurrentUser = null;
        OnAuthStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsAuthenticated => CurrentUser != null;
}
