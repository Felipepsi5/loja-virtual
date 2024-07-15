using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSE.Identidade.API.Migrations
{
    public partial class SecKeysnew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Algorithm",
                table: "SecurityKeys");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "SecurityKeys",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "SecurityKeys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "SecurityKeys");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "SecurityKeys");

            migrationBuilder.AddColumn<string>(
                name: "Algorithm",
                table: "SecurityKeys",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
