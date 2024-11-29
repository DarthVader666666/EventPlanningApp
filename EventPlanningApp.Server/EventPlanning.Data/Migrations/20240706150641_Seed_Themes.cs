using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Themes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"INSERT INTO Themes (ThemeName) VALUES ('Music')" +
                $"INSERT INTO Themes (ThemeName) VALUES ('Sport')" +
                $"INSERT INTO Themes (ThemeName) VALUES ('Conference')" +
                $"INSERT INTO Themes (ThemeName) VALUES ('Corporate Party')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
