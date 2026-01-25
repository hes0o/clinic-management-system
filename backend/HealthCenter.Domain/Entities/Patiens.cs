namespace HealthCenter.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public string Gender { get; private set; } = string.Empty;

    private Patient() { }

    public static Patient Create(string fullName, string phoneNumber, DateTime birthDate, string gender)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required.", nameof(fullName));
        
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("PhoneNumber is required.", nameof(phoneNumber));
        
        if (string.IsNullOrWhiteSpace(gender))
            throw new ArgumentException("Gender is required.", nameof(gender));

        return new Patient
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            PhoneNumber = phoneNumber,
            BirthDate = birthDate,
            Gender = gender
        };
    }
}