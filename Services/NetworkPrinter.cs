using System;
using System.Drawing; // سبب المشكلة (خاص بويندوز)
using System.Drawing.Printing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Versioning; // 👈 مهم جداً للإصلاح
using HealthCenter.Desktop.Features.Reception.Models;

namespace HealthCenter.Desktop.Services;

// ✅ هذا السطر هو الحل السحري لكل الأخطاء
// يخبر المترجم: "هذا الكلاس سيعمل فقط على ويندوز، فلا تنبهني لباقي الأنظمة"
[SupportedOSPlatform("windows")]
public class NetworkPrinter
{
    private readonly string _ipAddress;
    private readonly int _port;

    public NetworkPrinter(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public void PrintTicket(Patient patient, int waitingCount)
    {
        try
        {
            // 1. إنشاء صورة التذكرة
            using Bitmap ticketImage = CreateTicketImage(patient, waitingCount);

            // 2. تحويل الصورة إلى أوامر
            byte[] printData = GetImagePrintData(ticketImage);

            // 3. الإرسال
            using TcpClient client = new TcpClient();
            if (!client.ConnectAsync(_ipAddress, _port).Wait(3000))
            {
                throw new Exception("فشل الاتصال بالطابعة.");
            }

            using NetworkStream stream = client.GetStream();

            stream.Write(new byte[] { 0x1B, 0x40 }, 0, 2); // تهيئة
            stream.Write(printData, 0, printData.Length);  // الصورة
            stream.Write(new byte[] { 0x1D, 0x56, 0x42, 0x00 }, 0, 4); // قص
        }
        catch (Exception ex)
        {
            Console.WriteLine("Print Error: " + ex.Message);
        }
    }

    private Bitmap CreateTicketImage(Patient patient, int waitingCount)
    {
        int width = 570;
        int height = 650; // زدنا الارتفاع قليلاً
        Bitmap bmp = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(bmp);

        g.Clear(Color.White);

        // تحسين الخطوط لتظهر واضحة
        Font fontTitle = new Font("Arial", 22, FontStyle.Bold);
        Font fontNormal = new Font("Arial", 18, FontStyle.Bold); // جعلناه Bold ليكون أوضح
        Font fontBig = new Font("Arial", 60, FontStyle.Bold);

        StringFormat center = new StringFormat { Alignment = StringAlignment.Center };
        StringFormat right = new StringFormat { Alignment = StringAlignment.Far };

        int y = 20;

        // الرأس
        g.DrawString("المركز الصحي", fontTitle, Brushes.Black, width / 2, y, center);
        y += 45;
        g.DrawString("Health Center", new Font("Arial", 14), Brushes.Black, width / 2, y, center);
        y += 40;

        // خط فاصل سميك
        g.FillRectangle(Brushes.Black, 20, y, width - 40, 3);
        y += 20;

        // الرقم
        g.DrawString("رقم التذكرة", fontNormal, Brushes.Black, width / 2, y, center);
        y += 35;
        g.DrawString($"T-{patient.TicketNumber}", fontBig, Brushes.Black, width / 2, y, center);
        y += 90;

        // التفاصيل
        int rightMargin = width - 30;
        g.DrawString($"الاسم: {patient.FullName}", fontNormal, Brushes.Black, rightMargin, y, right);
        y += 40;
        g.DrawString($"التاريخ: {DateTime.Now:yyyy/MM/dd}", fontNormal, Brushes.Black, rightMargin, y, right);
        y += 40;
        g.DrawString($"الوقت: {DateTime.Now:hh:mm tt}", fontNormal, Brushes.Black, rightMargin, y, right);
        y += 60;

        // مربع الانتظار (أسود)
        g.FillRectangle(Brushes.Black, 20, y, width - 40, 60);
        g.DrawString($"أمامك: {waitingCount}", new Font("Arial", 24, FontStyle.Bold), Brushes.White, width / 2, y + 10, center);

        return bmp;
    }

    // خوارزمية تحويل الصورة (لا تقلق بشأن تعقيدها، هي قياسية)
    private byte[] GetImagePrintData(Bitmap bitmap)
    {
        var list = new System.Collections.Generic.List<byte>();
        list.AddRange(new byte[] { 0x1B, 0x61, 0x01 }); // توسيط
        list.AddRange(new byte[] { 0x1D, 0x76, 0x30, 0x00 }); // أمر طباعة Raster

        int width = bitmap.Width;
        int height = bitmap.Height;
        int bytesWidth = (width + 7) / 8;

        list.Add((byte)(bytesWidth % 256));
        list.Add((byte)(bytesWidth / 256));
        list.Add((byte)(height % 256));
        list.Add((byte)(height / 256));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < bytesWidth; x++)
            {
                byte b = 0;
                for (int k = 0; k < 8; k++)
                {
                    int px = x * 8 + k;
                    if (px < width)
                    {
                        Color c = bitmap.GetPixel(px, y);
                        // أي لون غير الأبيض نعتبره أسود
                        if (c.R < 200 || c.G < 200 || c.B < 200)
                            b |= (byte)(1 << (7 - k));
                    }
                }
                list.Add(b);
            }
        }
        return list.ToArray();
    }
}