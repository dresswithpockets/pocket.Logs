using Microsoft.EntityFrameworkCore.Migrations;

namespace pocket.Logs.Migrations.Migrations
{
    public partial class AddProcessedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "RetrievedLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "RetrievedLogs");
        }
    }
}
