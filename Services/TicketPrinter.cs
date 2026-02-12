using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning; // 👈 مهم جداً لإضافة خاصية الويندوز
using HealthCenter.Desktop.Features.Reception.Models;

namespace HealthCenter.Desktop.Services;

// ✅ هذا السطر يخبر المترجم: "لا تقلق، هذا الكلاس مخصص للعمل على Windows فقط"
[SupportedOSPlatform("windows")]
public class TicketPrinter
{
    private readonly Patient _patient;
    private readonly int _waitingCount;

    public TicketPrinter(Patient patient, int waitingCount)
    {
        _patient = patient;
        _waitingCount = waitingCount;
    }

    public void Print()
    {
        try
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(DrawTicket);
            pd.Print();
        }
        catch (Exception ex)
        {
            Console.WriteLine("خطأ في الطباعة: " + ex.Message);
        }
    }

    private void DrawTicket(object sender, PrintPageEventArgs e)
    {
        // ✅ استخدام (!) لنؤكد للمترجم أن الجرافيكس موجود ولن يكون فارغاً
        Graphics g = e.Graphics!; 
        
        // إعدادات الخطوط
        Font fontTitle = new Font("Arial", 14, FontStyle.Bold);
        Font fontHeader = new Font("Arial", 10, FontStyle.Regular);
        Font fontBigNumber = new Font("Arial", 35, FontStyle.Bold);
        Font fontNormal = new Font("Arial", 9, FontStyle.Regular);
        Font fontBold = new Font("Arial", 9, FontStyle.Bold);
        Font fontFooter = new Font("Arial", 8, FontStyle.Italic);

        Brush brush = Brushes.Black;
        float y = 10; 
        float centerX = e.PageBounds.Width / 2;
        float leftMargin = 5;
        
        // دالة مساعدة
        void DrawCentered(string text, Font font, float yPos)
        {
            if (g == null) return;
            SizeF size = g.MeasureString(text, font);
            g.DrawString(text, font, brush, centerX - (size.Width / 2), yPos);
        }

        // --- الرسم ---
        DrawCentered("المركز الصحي", fontTitle, y);
        y += 25;
        DrawCentered("Health Center System", fontHeader, y);
        y += 20;
        
        g.DrawLine(Pens.Black, leftMargin, y, e.PageBounds.Width - 5, y);
        y += 10;

        DrawCentered("رقم التذكرة / Ticket No", fontNormal, y);
        y += 20;
        DrawCentered(_patient.TicketNumber.ToString(), fontBigNumber, y);
        y += 60;

        string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        g.DrawString("التاريخ: " + dateStr, fontNormal, brush, leftMargin, y);
        y += 20;
        
        g.DrawString("المريض: " + _patient.FullName, fontBold, brush, leftMargin, y);
        y += 20;
        
        g.DrawString("العيادة: الطب العام", fontNormal, brush, leftMargin, y);
        y += 30;

        Rectangle rect = new Rectangle((int)leftMargin, (int)y, (int)e.PageBounds.Width - 10, 30);
        g.FillRectangle(Brushes.Black, rect);
        
        string waitText = $"أمامك في الانتظار: {_waitingCount}";
        SizeF waitSize = g.MeasureString(waitText, fontBold);
        g.DrawString(waitText, fontBold, Brushes.White, centerX - (waitSize.Width / 2), y + 7);
        
        y += 45;

        DrawCentered("نتمنى لكم الشفاء العاجل", fontFooter, y);
        y += 15;
        DrawCentered("www.health-center.com", fontFooter, y);
    }
}