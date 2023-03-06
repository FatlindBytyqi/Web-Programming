using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Data.Migrations
{
    public partial class DeptBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeptBranch",
                table: "DeptBranch");

            migrationBuilder.DropIndex(
                name: "IX_DeptBranch_DepartmentID",
                table: "DeptBranch");

            migrationBuilder.DropColumn(
                name: "DeptBranchID",
                table: "DeptBranch");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentID",
                table: "DeptBranch",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "BranchID",
                table: "DeptBranch",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeptBranch",
                table: "DeptBranch",
                columns: new[] { "DepartmentID", "BranchID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeptBranch",
                table: "DeptBranch");

            migrationBuilder.AlterColumn<int>(
                name: "BranchID",
                table: "DeptBranch",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentID",
                table: "DeptBranch",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<int>(
                name: "DeptBranchID",
                table: "DeptBranch",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeptBranch",
                table: "DeptBranch",
                column: "DeptBranchID");

            migrationBuilder.CreateIndex(
                name: "IX_DeptBranch_DepartmentID",
                table: "DeptBranch",
                column: "DepartmentID");
        }
    }
}
