using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixTeamModifiedAtAndEpicUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Epics_TeamId_Title",
                table: "Epics");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Teams",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_TeamId",
                table: "Epics",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Epics_TeamId",
                table: "Epics");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Teams");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_TeamId_Title",
                table: "Epics",
                columns: new[] { "TeamId", "Title" },
                unique: true);
        }
    }
}
