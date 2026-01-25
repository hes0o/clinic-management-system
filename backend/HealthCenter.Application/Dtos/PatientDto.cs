namespace HealthCenter.Application.Dtos;

public record CreatePatientRequestDto(
    string FullName,
    string PhoneNumber,
    DateTime BirthDate,
    string Gender
);

public record PatientResponseDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    DateTime BirthDate,
    string Gender
);
