using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanning.Data.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Event_and_Location_entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Locations");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
