using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Desktop.Features.Cashier.ViewModels;

public partial class CashierPanelViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase, IDisposable
{
    private readonly HealthCenterDbContext _db;
    private readonly DispatcherTimer _refreshTimer;

    [ObservableProperty] private ObservableCollection<Invoice> _pendingInvoices = new();
    [ObservableProperty] private Invoice? _selectedInvoice;
    [ObservableProperty] private string _selectedPaymentMethod = "Cash";
    [ObservableProperty] private bool _hasNoInvoices;

    public ObservableCollection<string> PaymentMethods { get; } = new()
        { "Cash", "Card", "Insurance" };

    public CashierPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadInvoices();

        // Setup background polling every 5 seconds
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _refreshTimer.Tick += (s, e) => LoadInvoicesSilent();
        _refreshTimer.Start();
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Tick -= (s, e) => LoadInvoicesSilent();
        _db?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void LoadInvoices()
    {
        try
        {
            var invoices = _db.Invoices
                .Include(i => i.Patient)
                .Where(i => i.Status == InvoiceStatus.Pending)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            PendingInvoices = new ObservableCollection<Invoice>(invoices);
            HasNoInvoices = PendingInvoices.Count == 0;
        }
        catch (Exception ex)
        {
            ShowError($"خطأ في تحميل الفواتير: {ex.Message}");
            PendingInvoices = new ObservableCollection<Invoice>();
            HasNoInvoices = true;
        }
    }

    private void LoadInvoicesSilent()
    {
        try
        {
            var invoices = _db.Invoices
                .Include(i => i.Patient)
                .Where(i => i.Status == InvoiceStatus.Pending)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            bool hasChanged = PendingInvoices.Count != invoices.Count;
            if (!hasChanged)
            {
                for (int i = 0; i < invoices.Count; i++)
                {
                    if (PendingInvoices[i].Id != invoices[i].Id ||
                        PendingInvoices[i].CreatedAt != invoices[i].CreatedAt)
                    {
                        hasChanged = true;
                        break;
                    }
                }
            }

            if (hasChanged)
            {
                PendingInvoices = new ObservableCollection<Invoice>(invoices);
                HasNoInvoices = PendingInvoices.Count == 0;
            }
        }
        catch (Exception) { /* Fail silently */ }
    }

    [RelayCommand]
    private void MarkAsPaid()
    {
        if (SelectedInvoice == null)
        {
            ShowError("الرجاء تحديد فاتورة أولاً.");
            return;
        }

        try
        {
            var method = SelectedPaymentMethod switch
            {
                "Card" => PaymentMethod.Card,
                "Insurance" => PaymentMethod.Insurance,
                _ => PaymentMethod.Cash
            };

            SelectedInvoice.Status = InvoiceStatus.Paid;
            SelectedInvoice.PaymentMethod = method;
            SelectedInvoice.PaidAt = DateTime.UtcNow;

            _db.SaveChanges();

            ShowSuccess($"تم تحديد فاتورة المريض {SelectedInvoice.Patient?.FullName} كمدفوعة.");
            LoadInvoices();
        }
        catch (Exception ex)
        {
            ShowError($"خطأ في تحديث الفاتورة: {ex.Message}");
        }
    }

}
