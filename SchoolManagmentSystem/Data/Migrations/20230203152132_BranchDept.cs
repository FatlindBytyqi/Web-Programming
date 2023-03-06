using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Data.Migrations
{
    public partial class BranchDept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SMIAL = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchID);
                });

            migrationBuilder.CreateTable(
                name: "DeptBranch",
                columns: table => new
                {
                    DeptBranchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeptBranch", x => x.DeptBranchID);
                    table.ForeignKey(
                        name: "FK_DeptBranch_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeptBranch_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeptBranch_BranchID",
                table: "DeptBranch",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_DeptBranch_DepartmentID",
                table: "DeptBranch",
                column: "DepartmentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeptBranch");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
