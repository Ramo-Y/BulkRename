using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class CreateTableRenamingSessionToEpisodeItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RenamingSessionToEpisodeItems",
                columns: table => new
                {
                    RenamingSessionToEpisodeID = table.Column<Guid>(nullable: false),
                    RseRenamingSessionID_FK = table.Column<Guid>(nullable: false),
                    RseEpisodeID_FK = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenamingSessionToEpisodeItems", x => x.RenamingSessionToEpisodeID);
                    table.ForeignKey(
                        name: "FK_RenamingSessionToEpisodeItems_EpisodeItems_RseEpisodeID_FK",
                        column: x => x.RseEpisodeID_FK,
                        principalTable: "EpisodeItems",
                        principalColumn: "EpisodeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RenamingSessionToEpisodeItems_RenamingSessionItems_RseRenamingSessionID_FK",
                        column: x => x.RseRenamingSessionID_FK,
                        principalTable: "RenamingSessionItems",
                        principalColumn: "RenamingSessionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RenamingSessionToEpisodeItems_RseEpisodeID_FK",
                table: "RenamingSessionToEpisodeItems",
                column: "RseEpisodeID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_RenamingSessionToEpisodeItems_RseRenamingSessionID_FK",
                table: "RenamingSessionToEpisodeItems",
                column: "RseRenamingSessionID_FK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RenamingSessionToEpisodeItems");
        }
    }
}
