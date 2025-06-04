using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class AddSetTimingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "SetDuration",
                table: "Sets",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SetStartTime",
                table: "Sets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetDuration",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "SetStartTime",
                table: "Sets");
        }
    }
}
