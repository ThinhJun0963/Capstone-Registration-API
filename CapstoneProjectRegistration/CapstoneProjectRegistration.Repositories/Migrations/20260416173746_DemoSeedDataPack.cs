using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapstoneProjectRegistration.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class DemoSeedDataPack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "Id", "Email", "Name", "Status" },
                values: new object[] { 1, "admin@capstone.local", "System Admin", "Active" });

            migrationBuilder.InsertData(
                table: "Lecture",
                columns: new[] { "Id", "Email", "Name", "Phone", "Specialization", "Status", "Title" },
                values: new object[] { 4, "lecturer.d@example.com", "Pham Thi D", "0900000004", "Data Science", "Active", "Prof" });

            migrationBuilder.InsertData(
                table: "RegistrationPeriod",
                columns: new[] { "Id", "EndDate", "Name", "SemesterId", "StartDate", "Status" },
                values: new object[] { 2, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Summer 2026", 1, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inactive" });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Email", "GroupRole", "Name", "Phone", "Status", "StudentCode" },
                values: new object[,]
                {
                    { 1, "thinhpdqse171589@fpt.edu.vn", "Leader", "Pham Dinh Quoc Thinh", "0842918005", "Active", "SE171589" },
                    { 2, "namnnse182539@fpt.edu.vn", "Member", "Nguyen Nhat Nam", "0704656071", "Active", "SE182539" }
                });

            migrationBuilder.InsertData(
                table: "Topic",
                columns: new[] { "Id", "CreatedAt", "CreatorId", "Description", "EnglishName", "PublicStatus", "RegistrationPeriodId", "ReviewStatus", "SemesterId", "Status", "TopicCode", "VietnameseName" },
                values: new object[,]
                {
                    { 1001, new DateTime(2026, 4, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "Interactive VQA and RAG based learning system for bone diseases.", "BoneVisQA Interactive Learning", "Public", 1, "Approved", 1, "Approved", "SP26SE110", "BoneVisQA Hoi Dap Truc Quan" },
                    { 1002, new DateTime(2026, 4, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 4, "Detect duplicated project ideas via fuzzy matching and semantic scoring.", "Capstone Topic Similarity Scanner", "Private", 1, "Rejected", 1, "Rejected", "SU26SE002", "He thong so khop de tai do an" }
                });

            migrationBuilder.InsertData(
                table: "TopicReview",
                columns: new[] { "Id", "Comment", "Decision", "IsFinalized", "ReviewDate", "ReviewerId", "TopicId" },
                values: new object[,]
                {
                    { 5001, "Clear scope.", "Approved", true, new DateTime(2026, 4, 6, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 1001 },
                    { 5002, "Good practical value.", "Approved", true, new DateTime(2026, 4, 6, 11, 0, 0, 0, DateTimeKind.Unspecified), 3, 1001 },
                    { 5003, "Topic overlaps too much with existing works.", "Rejected", true, new DateTime(2026, 4, 7, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 1002 },
                    { 5004, "Acceptable but revise objectives.", "Approved", true, new DateTime(2026, 4, 7, 11, 0, 0, 0, DateTimeKind.Unspecified), 3, 1002 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admin",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RegistrationPeriod",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TopicReview",
                keyColumn: "Id",
                keyValue: 5001);

            migrationBuilder.DeleteData(
                table: "TopicReview",
                keyColumn: "Id",
                keyValue: 5002);

            migrationBuilder.DeleteData(
                table: "TopicReview",
                keyColumn: "Id",
                keyValue: 5003);

            migrationBuilder.DeleteData(
                table: "TopicReview",
                keyColumn: "Id",
                keyValue: 5004);

            migrationBuilder.DeleteData(
                table: "Topic",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Topic",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
