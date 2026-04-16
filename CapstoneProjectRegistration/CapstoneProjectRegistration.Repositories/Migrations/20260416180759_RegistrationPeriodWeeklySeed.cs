using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProjectRegistration.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RegistrationPeriodWeeklySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "admin@gmail.com");

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer1@gmail.com", "Lecturer 1" });

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer2@gmail.com", "Lecturer 2" });

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer3@gmail.com", "Lecturer 3" });

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer4@gmail.com", "Lecturer 4" });

            migrationBuilder.UpdateData(
                table: "RegistrationPeriod",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "Name" },
                values: new object[] { new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spring 2026 - Week 1" });

            migrationBuilder.UpdateData(
                table: "RegistrationPeriod",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndDate", "Name", "StartDate" },
                values: new object[] { new DateTime(2026, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spring 2026 - Week 2", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "RegistrationPeriod",
                columns: new[] { "Id", "EndDate", "Name", "SemesterId", "StartDate", "Status" },
                values: new object[] { 3, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spring 2026 - Week 3", 1, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inactive" });

            migrationBuilder.UpdateData(
                table: "Topic",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "Description", "TopicCode", "VietnameseName" },
                values: new object[] { "Learning system for bone diseases.", "SU26SE0011", "BoneVisQA Hỏi Đáp Trực Quan" });

            migrationBuilder.UpdateData(
                table: "Topic",
                keyColumn: "Id",
                keyValue: 1002,
                column: "VietnameseName",
                value: "Hệ thống số khớp đề tài do an");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationPeriod",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "admin@capstone.local");

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer.a@example.com", "Nguyen Van A" });

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer.b@example.com", "Tran Thi B" });

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer.c@example.com", "Le Van C" });

            migrationBuilder.UpdateData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "Name" },
                values: new object[] { "lecturer.d@example.com", "Pham Thi D" });

            migrationBuilder.UpdateData(
                table: "RegistrationPeriod",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "Name" },
                values: new object[] { new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spring 2026" });

            migrationBuilder.UpdateData(
                table: "RegistrationPeriod",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndDate", "Name", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Summer 2026", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Topic",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "Description", "TopicCode", "VietnameseName" },
                values: new object[] { "Interactive VQA and RAG based learning system for bone diseases.", "SP26SE110", "BoneVisQA Hoi Dap Truc Quan" });

            migrationBuilder.UpdateData(
                table: "Topic",
                keyColumn: "Id",
                keyValue: 1002,
                column: "VietnameseName",
                value: "He thong so khop de tai do an");
        }
    }
}
