using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeManager.Web.Data.Migrations
{
    public partial class PreferredHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "PreferredHoursPerDay",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredHoursPerDay",
                table: "AspNetUsers");
        }
    }
}
