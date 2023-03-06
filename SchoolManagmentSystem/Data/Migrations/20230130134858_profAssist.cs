using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Data.Migrations
{
    public partial class profAssist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assistants_ProfessorID",
                table: "Assistants");

            migrationBuilder.CreateIndex(
                name: "IX_Assistants_ProfessorID",
                table: "Assistants",
                column: "ProfessorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assistants_ProfessorID",
                table: "Assistants");

            migrationBuilder.CreateIndex(
                name: "IX_Assistants_ProfessorID",
                table: "Assistants",
                column: "ProfessorID",
                unique: true);
        }
    }
}
