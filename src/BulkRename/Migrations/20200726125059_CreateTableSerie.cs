using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class CreateTableSerie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SerieItems",
                columns: table => new
                {
                    SerieID = table.Column<Guid>(nullable: false),
                    SerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerieItems", x => x.SerieID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SerieItems");
        }
    }
}
