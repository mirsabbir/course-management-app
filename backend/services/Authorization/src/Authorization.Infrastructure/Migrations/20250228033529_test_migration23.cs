using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test_migration23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1f8211b7-f591-4453-92a9-a8bcc0122903",
                columns: new[] { "DateOfBirth", "FullName" },
                values: new object[] { new DateTime(1995, 2, 1, 18, 0, 0, 0, DateTimeKind.Utc), "Staff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1f8211b7-f591-4453-92a9-a8bcc0122903",
                columns: new[] { "DateOfBirth", "FullName" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" });
        }
    }
}
