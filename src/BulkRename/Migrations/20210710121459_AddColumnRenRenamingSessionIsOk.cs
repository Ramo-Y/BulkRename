using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class AddColumnRenRenamingSessionIsOk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RenRenamingSessionIsOk",
                table: "RenamingSessionItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenRenamingSessionIsOk",
                table: "RenamingSessionItems");
        }
    }
}
