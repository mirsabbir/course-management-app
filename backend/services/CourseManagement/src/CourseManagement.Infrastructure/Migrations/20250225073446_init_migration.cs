using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                schema: "CourseManagement",
                table: "CourseStudent",
                newName: "AssignedById");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                schema: "CourseManagement",
                table: "ClassStudent",
                newName: "AssignedById");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                schema: "CourseManagement",
                table: "ClassCourse",
                newName: "AssignedById");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CourseManagement",
                table: "Students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                schema: "CourseManagement",
                table: "Students",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                schema: "CourseManagement",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignedByName",
                schema: "CourseManagement",
                table: "CourseStudent",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                schema: "CourseManagement",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignedByName",
                schema: "CourseManagement",
                table: "ClassStudent",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                schema: "CourseManagement",
                table: "Classes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignedByName",
                schema: "CourseManagement",
                table: "ClassCourse",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CourseManagement",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "CourseManagement",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                schema: "CourseManagement",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AssignedByName",
                schema: "CourseManagement",
                table: "CourseStudent");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                schema: "CourseManagement",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AssignedByName",
                schema: "CourseManagement",
                table: "ClassStudent");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                schema: "CourseManagement",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "AssignedByName",
                schema: "CourseManagement",
                table: "ClassCourse");

            migrationBuilder.RenameColumn(
                name: "AssignedById",
                schema: "CourseManagement",
                table: "CourseStudent",
                newName: "AssignedBy");

            migrationBuilder.RenameColumn(
                name: "AssignedById",
                schema: "CourseManagement",
                table: "ClassStudent",
                newName: "AssignedBy");

            migrationBuilder.RenameColumn(
                name: "AssignedById",
                schema: "CourseManagement",
                table: "ClassCourse",
                newName: "AssignedBy");
        }
    }
}
