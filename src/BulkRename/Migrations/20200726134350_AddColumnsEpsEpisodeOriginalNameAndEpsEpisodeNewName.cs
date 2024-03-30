using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkRename.Migrations
{
    public partial class AddColumnsEpsEpisodeOriginalNameAndEpsEpisodeNewName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EpsEpisodeNewName",
                table: "EpisodeItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EpsEpisodeOriginalName",
                table: "EpisodeItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpsEpisodeNewName",
                table: "EpisodeItems");

            migrationBuilder.DropColumn(
                name: "EpsEpisodeOriginalName",
                table: "EpisodeItems");
        }
    }
}
