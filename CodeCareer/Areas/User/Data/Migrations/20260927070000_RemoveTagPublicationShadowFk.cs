using CodeCareer.Areas.User.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCareer.Areas.User.Data.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260927070000_RemoveTagPublicationShadowFk")]
    public partial class RemoveTagPublicationShadowFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tags_publications_PublicationModelId",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_tags_PublicationModelId",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "PublicationModelId",
                table: "tags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublicationModelId",
                table: "tags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tags_PublicationModelId",
                table: "tags",
                column: "PublicationModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_tags_publications_PublicationModelId",
                table: "tags",
                column: "PublicationModelId",
                principalTable: "publications",
                principalColumn: "Id");
        }
    }
}
