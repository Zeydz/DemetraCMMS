using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_projektuppgift.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateFieldsFromTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Technicians");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Technicians",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Technicians");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Technicians",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Technicians",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Technicians",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
