using System;
using System.Collections.ObjectModel;
using System.Linq;
using HealthCenter.Desktop.Features.Reception.Models;

namespace HealthCenter.Desktop.Services;

public class ClinicStore
{
    private static ClinicStore? _instance;
    public static ClinicStore Instance => _instance ??= new ClinicStore();

    public ObservableCollection<Patient> AllPatients { get; } = new();
    public ObservableCollection<Patient> TodayActiveQueue { get; } = new();

    public int WaitingCount => TodayActiveQueue.Count;
    public int TodayCount => AllPatients.Count(p => p.RegistrationDate.Date == DateTime.Today);

    public event Action? QueueChanged;
    public event Action<Patient>? OnPatientCalled; 

    private ClinicStore() { }

    public void RegisterNewPatient(Patient patient)
    {
        if (!AllPatients.Any(p => p.NationalId == patient.NationalId))
        {
            AllPatients.Add(patient);
            QueueChanged?.Invoke();
        }
    }

    public void SendToDoctorQueue(Patient patient)
    {
        if (TodayActiveQueue.Any(p => p.Id == patient.Id)) return;

        int age = 0;
        if (patient.BirthDate.HasValue)
        {
            age = DateTime.Now.Year - patient.BirthDate.Value.Year;
            if (patient.BirthDate.Value.Date > DateTime.Now.AddYears(-age)) age--;
        }

        patient.IsPriority = age >= 65; 

        if (patient.IsPriority)
        {
            int insertIndex = TodayActiveQueue.Count(p => p.IsPriority);
            TodayActiveQueue.Insert(insertIndex, patient);
        }
        else
        {
            TodayActiveQueue.Add(patient);
        }

        QueueChanged?.Invoke();
    }

    public void CallPatient(Patient patient)
    {
        OnPatientCalled?.Invoke(patient);
        TodayActiveQueue.Remove(patient);
        QueueChanged?.Invoke();
    }

    public void RemovePatient(Patient patient)
    {
        if (AllPatients.Contains(patient))
            AllPatients.Remove(patient);
            
        if (TodayActiveQueue.Contains(patient))
            TodayActiveQueue.Remove(patient);
            
        QueueChanged?.Invoke();
    }
}