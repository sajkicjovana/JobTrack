using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnologies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Technologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationTechnologies",
                columns: table => new
                {
                    JobApplicationId = table.Column<int>(type: "integer", nullable: false),
                    TechnologyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationTechnologies", x => new { x.JobApplicationId, x.TechnologyId });
                    table.ForeignKey(
                        name: "FK_JobApplicationTechnologies_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplicationTechnologies_Technologies_TechnologyId",
                        column: x => x.TechnologyId,
                        principalTable: "Technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Technologies",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, ".NET" },
                    { 3, "ASP.NET Core" },
                    { 4, "Angular" },
                    { 5, "React" },
                    { 6, "JavaScript" },
                    { 7, "TypeScript" },
                    { 8, "Java" },
                    { 9, "Python" },
                    { 10, "SQL" },
                    { 11, "PostgreSQL" },
                    { 12, "Node.js" },
                    { 13, "Git" },
                    { 14, "Docker" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationTechnologies_TechnologyId",
                table: "JobApplicationTechnologies",
                column: "TechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Technologies_Name",
                table: "Technologies",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplicationTechnologies");

            migrationBuilder.DropTable(
                name: "Technologies");
        }
    }
}
