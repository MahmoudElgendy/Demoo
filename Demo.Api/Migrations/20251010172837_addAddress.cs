using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Demo.Api.Migrations
{
    /// <inheritdoc />
    public partial class addAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Persons",
                type: "nvarchar(46)",
                maxLength: 46,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine2",
                table: "Persons",
                type: "nvarchar(46)",
                maxLength: 46,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Persons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_CountryCode",
                table: "Persons",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_County",
                table: "Persons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Address_Location",
                table: "Persons",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Persons",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_RegionCode",
                table: "Persons",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine2",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_CountryCode",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_County",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_Location",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Address_RegionCode",
                table: "Persons");
        }
    }
}
