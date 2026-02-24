using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthCenter.Desktop.Database;
using HealthCenter.Desktop.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Desktop.Features.Cashier.ViewModels;

public partial class CashierPanelViewModel : HealthCenter.Desktop.ViewModels.ViewModelBase
{
    private readonly HealthCenterDbContext _db;

    [ObservableProperty] private ObservableCollection<Invoice> _pendingInvoices = new();
    [ObservableProperty] private Invoice? _selectedInvoice;
    [ObservableProperty] private string _selectedPaymentMethod = "Cash";
    [ObservableProperty] private string _statusMessage = string.Empty;

    public ObservableCollection<string> PaymentMethods { get; } = new()
        { "Cash", "Card", "Insurance" };

    public CashierPanelViewModel()
    {
        _db = new HealthCenterDbContext();
        LoadInvoices();
    }

    private void LoadInvoices()
    {
        var invoices = _db.Invoices
            .Include(i => i.Patient)
            .Where(i => i.Status == InvoiceStatus.Pending)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

        PendingInvoices = new ObservableCollection<Invoice>(invoices);
    }

    [RelayCommand]
    private void MarkAsPaid()
    {
        if (SelectedInvoice == null)
        {
            StatusMessage = "الرجاء تحديد فاتورة أولاً.";
            return;
        }

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

        StatusMessage = $"تم تحديد فاتورة المريض {SelectedInvoice.Patient?.FullName} كمدفوعة.";
        LoadInvoices();
    }

    [RelayCommand]
    private void RefreshInvoices()
    {
        LoadInvoices();
        StatusMessage = "تم تحديث قائمة الفواتير.";
    }
}
