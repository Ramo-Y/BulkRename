using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class CreateTableRenamingSessionItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RenamingSessionItems",
                columns: table => new
                {
                    RenamingSessionID = table.Column<Guid>(nullable: false),
                    RenName = table.Column<string>(nullable: true),
                    RenExecutingDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenamingSessionItems", x => x.RenamingSessionID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RenamingSessionItems");
        }
    }
}
