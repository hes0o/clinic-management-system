using System;

namespace HealthCenter.Desktop.Database.Entities;

public static class TicketStatusExtensions
{
    public static string ToArabic(this TicketStatus status) => status switch
    {
        TicketStatus.Waiting => "في الانتظار",
        TicketStatus.Called => "تم النداء",
        TicketStatus.InProgress => "قيد الفحص",
        TicketStatus.AwaitingRecall => "بانتظار إعادة النداء",
        TicketStatus.Completed => "منتهي",
        TicketStatus.Present => "حاضر",
        TicketStatus.Absent => "غائب",
        _ => status.ToString()
    };
}
