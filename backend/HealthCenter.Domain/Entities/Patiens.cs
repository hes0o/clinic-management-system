using HealthCenter.Domain.Common;
using HealthCenter.Domain.Events;
using HealthCenter.Domain.ValueObjects;

namespace HealthCenter.Domain.Entities;

public sealed class Patient : Entity
{
    public FullName FullName { get; private set; } = null!;
    public PhoneNumber Phone { get; private set; } = null!;

    private Patient() { }

    private Patient(FullName fullName, PhoneNumber phone)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Phone = phone;
    }

    public static Patient Create(string fullName, string phone)
    {
        var nameVO = FullName.Create(fullName);
        var phoneVO = PhoneNumber.Create(phone);

        var patient = new Patient(nameVO, phoneVO);
        patient.RaiseDomainEvent(new PatientCreatedEvent(patient.Id, nameVO.Value));

        return patient;
    }

    public void UpdateContactInfo(string phone)
    {
        Phone = PhoneNumber.Create(phone);
    }
}