using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkRename.Migrations
{
    /// <inheritdoc />
    public partial class CreateMoreFieldsAboutFileInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EpsEpisodeFileSizeInMb",
                table: "EpisodeItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "EpsFileExtension",
                table: "EpisodeItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EpsLastWriteTimeTimeUtc",
                table: "EpisodeItems",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpsEpisodeFileSizeInMb",
                table: "EpisodeItems");

            migrationBuilder.DropColumn(
                name: "EpsFileExtension",
                table: "EpisodeItems");

            migrationBuilder.DropColumn(
                name: "EpsLastWriteTimeTimeUtc",
                table: "EpisodeItems");
        }
    }
}
