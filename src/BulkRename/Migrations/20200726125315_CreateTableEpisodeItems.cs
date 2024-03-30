using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class CreateTableEpisodeItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EpisodeItems",
                columns: table => new
                {
                    EpisodeID = table.Column<Guid>(nullable: false),
                    EpsNumberString = table.Column<string>(nullable: true),
                    EpsSeasonID_FK = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeItems", x => x.EpisodeID);
                    table.ForeignKey(
                        name: "FK_EpisodeItems_SeasonItems_EpsSeasonID_FK",
                        column: x => x.EpsSeasonID_FK,
                        principalTable: "SeasonItems",
                        principalColumn: "SeasonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeItems_EpsSeasonID_FK",
                table: "EpisodeItems",
                column: "EpsSeasonID_FK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpisodeItems");
        }
    }
}
