using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_Theatre_and_Art_Exhibition_Themes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"INSERT INTO Themes (ThemeName) VALUES ('Theatre')\n" +
                $"INSERT INTO Themes (ThemeName) VALUES ('Art Exhibition')\n" +
                $"GO\n"+

                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (5, 'Ballet')\n" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (5, 'Opera')\n" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (5, 'Conceptual performance')\n" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (6, 'Modern Art')\n" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (6, 'Paintings')\n" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (6, 'Sculptures')\n");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
