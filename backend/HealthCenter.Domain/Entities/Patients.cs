namespace HealthCenter.Domain.Entities;
//this code has been written by hassan so please don't play with it wihtout telling him :)
public class Patient
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }

    private Patient() 
    {
        FullName = null!;
        PhoneNumber = null!;
    } // For future persistence tools

    public Patient(string fullName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Patient name cannot be empty");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty");

        Id = Guid.NewGuid();
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }
}