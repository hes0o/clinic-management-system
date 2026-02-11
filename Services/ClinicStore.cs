using System;
using System.Collections.ObjectModel;
using System.Linq;
using HealthCenter.Desktop.Features.Reception.Models;

namespace HealthCenter.Desktop.Services;

public class ClinicStore
{
    // نمط Singleton: لضمان وجود نسخة واحدة فقط من البيانات في كامل التطبيق
    private static ClinicStore? _instance;
    public static ClinicStore Instance => _instance ??= new ClinicStore();

    // ✅ هذه هي القائمة المشتركة التي سيراها الجميع (الاستقبال، الانتظار، الإحصائيات)
    public ObservableCollection<Patient> AllPatients { get; } = new();

    // خاصية لحساب عدد المنتظرين (أي مريض مسجل اليوم)
    public int WaitingCount => AllPatients.Count(p => p.RegistrationDate.Date == DateTime.Today);

    // خاصية لحساب إجمالي مرضى اليوم
    public int TodayCount => AllPatients.Count(p => p.RegistrationDate.Date == DateTime.Today);

    // حدث (Event) لإبلاغ القوائم الأخرى عند حدوث تغيير (لتحديث العدادات فوراً)
    public event Action? QueueChanged;

    private ClinicStore()
    {
        // إضافة مريض تجريبي عند التشغيل
        AddPatientToQueue(new Patient 
        { 
            FullName = "تجربة ربط النظام", 
            PhoneNumber = "0500000000", 
            NationalId = "1000000000",
            TicketNumber = 1, 
            RegistrationDate = DateTime.Now 
        });
    }

    // دالة لإضافة مريض جديد
    public void AddPatientToQueue(Patient patient)
    {
        AllPatients.Add(patient);
        QueueChanged?.Invoke(); // تحديث العدادات
    }

    // دالة لحذف مريض
    public void RemovePatient(Patient patient)
    {
        if (AllPatients.Contains(patient))
        {
            AllPatients.Remove(patient);
            QueueChanged?.Invoke(); // تحديث العدادات
        }
    }
}