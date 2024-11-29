using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Entity_Ids_nullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubThemes_Themes_ThemeId",
                table: "SubThemes");

            migrationBuilder.AlterColumn<int>(
                name: "ThemeId",
                table: "SubThemes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SubThemes_Themes_ThemeId",
                table: "SubThemes",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "ThemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubThemes_Themes_ThemeId",
                table: "SubThemes");

            migrationBuilder.AlterColumn<int>(
                name: "ThemeId",
                table: "SubThemes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubThemes_Themes_ThemeId",
                table: "SubThemes",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "ThemeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
