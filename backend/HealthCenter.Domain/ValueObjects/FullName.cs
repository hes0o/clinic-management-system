namespace HealthCenter.Domain.ValueObjects;

public sealed class FullName : IEquatable<FullName>
{
    public string Value { get; }

    private FullName(string value)
    {
        Value = value;
    }

    public static FullName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        var trimmed = name.Trim();
        if (trimmed.Length < 2)
            throw new ArgumentException("Name must be at least 2 characters", nameof(name));

        return new FullName(trimmed);
    }

    public bool Equals(FullName? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as FullName);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
