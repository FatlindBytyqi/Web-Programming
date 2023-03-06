using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Data.Migrations
{
    public partial class profdept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Professors_ProfessorID",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ProfessorID",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ProfessorID",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentID",
                table: "Professors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_DepartmentID",
                table: "Professors",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Departments_DepartmentID",
                table: "Professors",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Departments_DepartmentID",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Professors_DepartmentID",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "Professors");

            migrationBuilder.AddColumn<int>(
                name: "ProfessorID",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ProfessorID",
                table: "Departments",
                column: "ProfessorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Professors_ProfessorID",
                table: "Departments",
                column: "ProfessorID",
                principalTable: "Professors",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
