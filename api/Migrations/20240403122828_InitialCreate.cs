using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO_tags.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Metadata",
                columns: table => new
                {
                    TotalTags = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => x.TotalTags);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    HasSynonyms = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsModeratorOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    Synonyms = table.Column<string>(type: "TEXT", nullable: true),
                    AllTagsPercentage = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Count",
                table: "Tags",
                column: "Count");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metadata");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
