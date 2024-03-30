using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class CreateTableSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeasonItems",
                columns: table => new
                {
                    SeasonID = table.Column<Guid>(nullable: false),
                    SsnNumberString = table.Column<string>(nullable: true),
                    SsnSerieID_FK = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonItems", x => x.SeasonID);
                    table.ForeignKey(
                        name: "FK_SeasonItems_SerieItems_SsnSerieID_FK",
                        column: x => x.SsnSerieID_FK,
                        principalTable: "SerieItems",
                        principalColumn: "SerieID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonItems_SsnSerieID_FK",
                table: "SeasonItems",
                column: "SsnSerieID_FK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeasonItems");
        }
    }
}
