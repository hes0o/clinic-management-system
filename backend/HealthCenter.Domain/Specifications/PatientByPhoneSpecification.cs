using HealthCenter.Domain.Entities;

namespace HealthCenter.Domain.Specifications;

/// <summary>
/// Concrete specification for finding patients by phone - OCP compliant
/// New specifications can be added without modifying existing code
/// </summary>
public class PatientByPhoneSpecification : BaseSpecification<Patient>
{
    public PatientByPhoneSpecification(string phone)
        : base(p => p.Phone.Value == phone)
    {
    }
}
