using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed_SubThemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (1, 'Rock Fest')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (1, 'Classic orchestra')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (1, 'Blues band')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (2, 'Football match')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (2, 'Fitness event')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (2, 'Yoga class')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (3, 'IT club')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (3, 'Buisness coaching')" +
                $"INSERT INTO SubThemes (ThemeId, SubThemeName) VALUES (3, 'Literature talks')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
