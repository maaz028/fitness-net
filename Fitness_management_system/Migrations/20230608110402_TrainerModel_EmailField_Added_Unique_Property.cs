using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_management_system.Migrations
{
    /// <inheritdoc />
    public partial class TrainerModel_EmailField_Added_Unique_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Trainer",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Trainer_Email",
                table: "Trainer",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainer_Email",
                table: "Trainer");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Trainer",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
