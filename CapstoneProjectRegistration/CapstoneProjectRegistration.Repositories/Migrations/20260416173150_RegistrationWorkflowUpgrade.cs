using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapstoneProjectRegistration.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RegistrationWorkflowUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TopicReview_TopicId",
                table: "TopicReview");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "TopicReview",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Topic",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Topic",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicStatus",
                table: "Topic",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RegistrationPeriodId",
                table: "Topic",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReviewStatus",
                table: "Topic",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TopicCode",
                table: "Topic",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupRole",
                table: "Student",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "Student",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Lecture",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Lecture",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RegistrationPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationPeriod_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Lecture",
                columns: new[] { "Id", "Email", "Name", "Phone", "Specialization", "Status", "Title" },
                values: new object[,]
                {
                    { 1, "lecturer.a@example.com", "Nguyen Van A", "0900000001", "Software Engineering", "Active", "Dr" },
                    { 2, "lecturer.b@example.com", "Tran Thi B", "0900000002", "Information Systems", "Active", "Ms" },
                    { 3, "lecturer.c@example.com", "Le Van C", "0900000003", "AI", "Active", "Mr" }
                });

            migrationBuilder.InsertData(
                table: "Semester",
                columns: new[] { "Id", "EndDate", "Name", "StartDate", "Status" },
                values: new object[] { 1, new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spring 2026", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active" });

            migrationBuilder.InsertData(
                table: "RegistrationPeriod",
                columns: new[] { "Id", "EndDate", "Name", "SemesterId", "StartDate", "Status" },
                values: new object[] { 1, new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spring 2026", 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_TopicReview_TopicId_ReviewerId",
                table: "TopicReview",
                columns: new[] { "TopicId", "ReviewerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topic_RegistrationPeriodId",
                table: "Topic",
                column: "RegistrationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_TopicCode",
                table: "Topic",
                column: "TopicCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriod_SemesterId",
                table: "RegistrationPeriod",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_RegistrationPeriod_RegistrationPeriodId",
                table: "Topic",
                column: "RegistrationPeriodId",
                principalTable: "RegistrationPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_RegistrationPeriod_RegistrationPeriodId",
                table: "Topic");

            migrationBuilder.DropTable(
                name: "RegistrationPeriod");

            migrationBuilder.DropIndex(
                name: "IX_TopicReview_TopicId_ReviewerId",
                table: "TopicReview");

            migrationBuilder.DropIndex(
                name: "IX_Topic_RegistrationPeriodId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_TopicCode",
                table: "Topic");

            migrationBuilder.DeleteData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lecture",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Semester",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "TopicReview");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "PublicStatus",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "RegistrationPeriodId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "TopicCode",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "GroupRole",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Lecture");

            migrationBuilder.CreateIndex(
                name: "IX_TopicReview_TopicId",
                table: "TopicReview",
                column: "TopicId");
        }
    }
}
