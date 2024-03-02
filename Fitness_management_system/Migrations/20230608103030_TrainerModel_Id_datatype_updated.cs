using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_management_system.Migrations
{
    /// <inheritdoc />
    public partial class TrainerModel_Id_datatype_updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Trainer");

            migrationBuilder.AddColumn<string>(
                name: "TrainerID",
                table: "Trainer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer",
                column: "TrainerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer");

            migrationBuilder.DropColumn(
                name: "TrainerID",
                table: "Trainer");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Trainer",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer",
                column: "ID");
        }
    }
}
