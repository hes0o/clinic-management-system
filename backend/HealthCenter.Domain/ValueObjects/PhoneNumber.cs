namespace HealthCenter.Domain.ValueObjects;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone number is required", nameof(phone));

        var cleaned = phone.Trim();
        if (cleaned.Length < 10)
            throw new ArgumentException("Phone number must be at least 10 digits", nameof(phone));

        return new PhoneNumber(cleaned);
    }

    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
