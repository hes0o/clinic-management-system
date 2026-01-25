namespace HealthCenter.Domain.Entities;
//this code has been written by hassan so please don't play with it wihtout telling him :)
//Updated by Bassam to add BirthDate and Gender for Phase 1 MVP
public class Patient
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
}