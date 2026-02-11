using System;

namespace HealthCenter.Desktop.Features.Reception.Models;

public class Patient
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; } 

    // ✅ الحقل الجديد: رقم التذكرة المتسلسل
    public int TicketNumber { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public string FormattedDate => RegistrationDate.ToString("yyyy-MM-dd");
}