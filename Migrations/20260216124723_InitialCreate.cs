using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthCenter.Desktop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Gender = table.Column<int>(type: "INTEGER", nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BloodType = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    EmergencyContact = table.Column<string>(type: "TEXT", nullable: true),
                    MedicalHistory = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DoctorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ScheduledTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QueueTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TicketNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DoctorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CalledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CallCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueTickets_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueTickets_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DoctorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Diagnosis = table.Column<string>(type: "TEXT", nullable: true),
                    Prescriptions = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Attachments = table.Column<string>(type: "TEXT", nullable: true),
                    InvoiceAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    VisitDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "BloodType", "CreatedAt", "DateOfBirth", "EmergencyContact", "FullName", "Gender", "MedicalHistory", "Notes", "PhoneNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "الرياض، حي الملز", "A+", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "0559876543", "أحمد محمد علي", 0, null, null, "0501234567", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "جدة، حي الروضة", "O-", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "0501234567", "فاطمة خالد العمري", 1, null, null, "0559876543", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "الدمام، حي الفيصلية", "B+", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1978, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "0554445566", "عبدالله سعد الغامدي", 0, null, null, "0551112233", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "مكة المكرمة، العزيزية", "AB+", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "0551112233", "نورة عبدالرحمن القحطاني", 1, null, null, "0544445566", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "أبها، حي المنهل", "O+", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1982, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "0561234567", "محمد عبدالعزيز الشهري", 0, null, null, "0567778899", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "الرياض، حي النخيل", "A-", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "0522223333", "سارة أحمد الدوسري", 1, null, null, "0533334444", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "الطائف، حي الشهداء", "B-", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "0533334444", "خالد فهد المطيري", 0, null, null, "0522223333", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "المدينة المنورة، حي العزيزية", "O+", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1975, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "0577778888", "منى سليمان الحربي", 1, null, null, "0588889999", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "جدة، حي السلامة", "AB-", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1987, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "0588889999", "ياسر علي الزهراني", 0, null, null, "0577778888", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "الخبر، حي الثقبة", "A+", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1993, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "0599998888", "ريم محمد القرني", 1, null, null, "0511112222", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "مدير النظام", true, "admin123", 0, "admin" });

            migrationBuilder.InsertData(
                table: "QueueTickets",
                columns: new[] { "Id", "CallCount", "CalledAt", "CompletedAt", "CreatedAt", "DoctorId", "PatientId", "Status", "TicketNumber" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 2, 16, 8, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 16, 8, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 16, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), 6, 1 },
                    { new Guid("20000000-0000-0000-0000-000000000002"), 1, new DateTime(2026, 2, 16, 8, 35, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 2, 16, 8, 10, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002"), 5, 2 },
                    { new Guid("20000000-0000-0000-0000-000000000003"), 0, null, null, new DateTime(2026, 2, 16, 8, 20, 0, 0, DateTimeKind.Unspecified), null, new Guid("10000000-0000-0000-0000-000000000003"), 0, 3 },
                    { new Guid("20000000-0000-0000-0000-000000000004"), 0, null, null, new DateTime(2026, 2, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("10000000-0000-0000-0000-000000000004"), 0, 4 },
                    { new Guid("20000000-0000-0000-0000-000000000005"), 0, null, null, new DateTime(2026, 2, 16, 9, 15, 0, 0, DateTimeKind.Unspecified), null, new Guid("10000000-0000-0000-0000-000000000005"), 0, 5 }
                });

            migrationBuilder.InsertData(
                table: "Visits",
                columns: new[] { "Id", "Attachments", "CreatedAt", "Diagnosis", "DoctorId", "InvoiceAmount", "Notes", "PatientId", "Prescriptions", "VisitDate" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 2, 16, 8, 30, 0, 0, DateTimeKind.Unspecified), "التهاب الحلق الحاد", new Guid("00000000-0000-0000-0000-000000000001"), 150.00m, "المريض يعاني من ارتفاع طفيف في درجة الحرارة", new Guid("10000000-0000-0000-0000-000000000001"), "Amoxicillin 500mg - 3 مرات يومياً لمدة 5 أيام", new DateTime(2026, 2, 16, 8, 15, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 2, 15, 10, 20, 0, 0, DateTimeKind.Unspecified), "ارتفاع ضغط الدم", new Guid("00000000-0000-0000-0000-000000000001"), 200.00m, "ينصح بتقليل الملح في الطعام وممارسة الرياضة", new Guid("10000000-0000-0000-0000-000000000006"), "Amlodipine 5mg - مرة واحدة يومياً صباحاً", new DateTime(2026, 2, 15, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 2, 14, 11, 25, 0, 0, DateTimeKind.Unspecified), "فحص دوري - السكري", new Guid("00000000-0000-0000-0000-000000000001"), 180.00m, "مستوى السكر التراكمي مستقر، المتابعة بعد 3 أشهر", new Guid("10000000-0000-0000-0000-000000000008"), "Metformin 500mg - مرتين يومياً مع الوجبات", new DateTime(2026, 2, 14, 11, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduledTime",
                table: "Appointments",
                column: "ScheduledTime");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FullName",
                table: "Patients",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PhoneNumber",
                table: "Patients",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_QueueTickets_CreatedAt",
                table: "QueueTickets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_QueueTickets_DoctorId",
                table: "QueueTickets",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueTickets_PatientId",
                table: "QueueTickets",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueTickets_Status",
                table: "QueueTickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId",
                table: "Visits",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PatientId",
                table: "Visits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_VisitDate",
                table: "Visits",
                column: "VisitDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "QueueTickets");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
