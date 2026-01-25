using HealthCenter.Application.Common;
using HealthCenter.Application.Dtos;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Mappers;

/// <summary>
/// Patient mapper - LSP compliant, can be substituted with any IMapper implementation
/// </summary>
public class PatientMapper : IMapper<Patient, PatientDto>
{
    public PatientDto Map(Patient source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new PatientDto(
            source.Id,
            source.FullName.Value,
            source.Phone.Value
        );
    }

    public IEnumerable<PatientDto> Map(IEnumerable<Patient> sources)
    {
        if (sources == null)
            throw new ArgumentNullException(nameof(sources));

        return sources.Select(Map);
    }
}
