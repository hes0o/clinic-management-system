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
                    NurseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Diagnosis = table.Column<string>(type: "TEXT", nullable: true),
                    Prescriptions = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    BloodPressure = table.Column<string>(type: "TEXT", nullable: true),
                    Temperature = table.Column<decimal>(type: "TEXT", nullable: true),
                    HeartRate = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: true),
                    Attachments = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Visits_Users_NurseId",
                        column: x => x.NurseId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VisitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VisitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TestName = table.Column<string>(type: "TEXT", nullable: false),
                    ResultNotes = table.Column<string>(type: "TEXT", nullable: true),
                    AttachmentPath = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    PerformedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTests_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabTests_Users_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTests_Users_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTests_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "مدير النظام", true, "admin123", 0, "admin" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "د. أحمد صالح", true, "doctor123", 2, "dr_ahmed" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "الممرضة فاطمة", true, "nurse123", 3, "nurse_fatima" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "المحاسب عمر", true, "cashier123", 5, "cashier_omar" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "فني المختبر سالم", true, "tech123", 6, "tech_salem" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "م. الاستقبال نور", true, "rec123", 4, "rec_nour" }
                });

            migrationBuilder.InsertData(
                table: "QueueTickets",
                columns: new[] { "Id", "CallCount", "CalledAt", "CompletedAt", "CreatedAt", "DoctorId", "PatientId", "Status", "TicketNumber" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 2, 16, 8, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 16, 8, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 16, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), 6, 1 },
                    { new Guid("20000000-0000-0000-0000-000000000002"), 1, new DateTime(2026, 2, 16, 8, 35, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 2, 16, 8, 10, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002"), 5, 2 },
                    { new Guid("20000000-0000-0000-0000-000000000003"), 0, null, null, new DateTime(2026, 2, 16, 8, 20, 0, 0, DateTimeKind.Unspecified), null, new Guid("10000000-0000-0000-0000-000000000003"), 0, 3 },
                    { new Guid("20000000-0000-0000-0000-000000000004"), 0, null, null, new DateTime(2026, 2, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("10000000-0000-0000-0000-000000000004"), 0, 4 },
                    { new Guid("20000000-0000-0000-0000-000000000005"), 0, null, null, new DateTime(2026, 2, 16, 9, 15, 0, 0, DateTimeKind.Unspecified), null, new Guid("10000000-0000-0000-0000-000000000005"), 0, 5 }
                });

            migrationBuilder.InsertData(
                table: "Visits",
                columns: new[] { "Id", "Attachments", "BloodPressure", "CreatedAt", "Diagnosis", "DoctorId", "HeartRate", "Notes", "NurseId", "PatientId", "Prescriptions", "Temperature", "VisitDate", "Weight" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), null, "120/80", new DateTime(2026, 2, 16, 8, 30, 0, 0, DateTimeKind.Unspecified), "التهاب الحلق الحاد", new Guid("00000000-0000-0000-0000-000000000002"), 85, "المريض يعاني من ارتفاع طفيف في درجة الحرارة", new Guid("00000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001"), "Amoxicillin 500mg - 3 مرات يومياً لمدة 5 أيام", 38.2m, new DateTime(2026, 2, 16, 8, 15, 0, 0, DateTimeKind.Unspecified), 70.0m },
                    { new Guid("30000000-0000-0000-0000-000000000002"), null, "150/95", new DateTime(2026, 2, 15, 10, 20, 0, 0, DateTimeKind.Unspecified), "ارتفاع ضغط الدم", new Guid("00000000-0000-0000-0000-000000000002"), 72, "ينصح بتقليل الملح في الطعام وممارسة الرياضة", new Guid("00000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000006"), "Amlodipine 5mg - مرة واحدة يومياً صباحاً", 36.8m, new DateTime(2026, 2, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 95.5m },
                    { new Guid("30000000-0000-0000-0000-000000000003"), null, null, new DateTime(2026, 2, 14, 11, 25, 0, 0, DateTimeKind.Unspecified), "فحص دوري - السكري", new Guid("00000000-0000-0000-0000-000000000002"), null, "مستوى السكر التراكمي مستقر، المتابعة بعد 3 أشهر", new Guid("00000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000008"), "Metformin 500mg - مرتين يومياً مع الوجبات", null, new DateTime(2026, 2, 14, 11, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "Amount", "CreatedAt", "CreatedById", "PaidAt", "PatientId", "PaymentMethod", "Status", "TaxAmount", "VisitId" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), 150.00m, new DateTime(2026, 2, 16, 8, 35, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2026, 2, 16, 8, 40, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000001"), 1, 1, 22.50m, new Guid("30000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000002"), 200.00m, new DateTime(2026, 2, 15, 10, 25, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000004"), null, new Guid("10000000-0000-0000-0000-000000000006"), null, 0, 30.00m, new Guid("30000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "LabTests",
                columns: new[] { "Id", "AttachmentPath", "CompletedAt", "PatientId", "PerformedById", "RequestedAt", "RequestedById", "ResultNotes", "Status", "TestName", "VisitId" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 2, 14, 13, 0, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2026, 2, 14, 11, 10, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "النتيجة: 6.2% - طبيعي بالنسبة لمريض سكري", 2, "فحص السكر التراكمي Hba1c", new Guid("30000000-0000-0000-0000-000000000003") });

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
                name: "IX_Invoices_CreatedById",
                table: "Invoices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientId",
                table: "Invoices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status",
                table: "Invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_VisitId",
                table: "Invoices",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_PatientId",
                table: "LabTests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_PerformedById",
                table: "LabTests",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_RequestedById",
                table: "LabTests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_Status",
                table: "LabTests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_VisitId",
                table: "LabTests",
                column: "VisitId");

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
                name: "IX_Visits_NurseId",
                table: "Visits",
                column: "NurseId");

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
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "LabTests");

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
