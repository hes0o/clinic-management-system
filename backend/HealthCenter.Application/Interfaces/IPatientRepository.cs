using HealthCenter.Domain.Common;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Interfaces;

/// <summary>
/// Patient repository interface - DIP compliant, inherits from generic repository
/// Allows for specialized patient queries while maintaining LSP
/// </summary>
public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByPhoneAsync(string phone);
    Task<bool> ExistsAsync(Guid id);
}
