using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDaySummaryAndScheduleEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DaySummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SummaryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TasksCompleted = table.Column<int>(type: "integer", nullable: false),
                    TasksTotal = table.Column<int>(type: "integer", nullable: false),
                    FocusHours = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductivityScore = table.Column<int>(type: "integer", nullable: false),
                    ProductivityGrade = table.Column<string>(type: "text", nullable: true),
                    StreakDays = table.Column<int>(type: "integer", nullable: false),
                    TomorrowPriority1 = table.Column<string>(type: "text", nullable: true),
                    TomorrowPriority2 = table.Column<string>(type: "text", nullable: true),
                    TomorrowPriority3 = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaySummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DaySummaries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ColorHex = table.Column<string>(type: "text", nullable: true),
                    IsUrgent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleEvents_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduleEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DaySummaries_UserId",
                table: "DaySummaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEvents_TaskId",
                table: "ScheduleEvents",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEvents_UserId",
                table: "ScheduleEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DaySummaries");

            migrationBuilder.DropTable(
                name: "ScheduleEvents");
        }
    }
}
