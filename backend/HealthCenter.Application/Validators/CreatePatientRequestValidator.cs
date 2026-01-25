using HealthCenter.Application.Common;
using HealthCenter.Application.Dtos;
using HealthCenter.Domain.Common;

namespace HealthCenter.Application.Validators;

/// <summary>
/// Request validator - OCP compliant, extensible for new validation rules
/// </summary>
public class CreatePatientRequestValidator : IValidator<CreatePatientRequest>
{
    public Result Validate(CreatePatientRequest item)
    {
        if (item == null)
            return Result.Failure("Request cannot be null");

        if (string.IsNullOrWhiteSpace(item.FullName))
            return Result.Failure("FullName is required");

        if (string.IsNullOrWhiteSpace(item.Phone))
            return Result.Failure("Phone is required");

        if (item.FullName.Trim().Length < 2)
            return Result.Failure("FullName must be at least 2 characters");

        if (item.Phone.Trim().Length < 10)
            return Result.Failure("Phone must be at least 10 digits");

        return Result.Success();
    }
}
