using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Topics",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Topics");
        }
    }
}
