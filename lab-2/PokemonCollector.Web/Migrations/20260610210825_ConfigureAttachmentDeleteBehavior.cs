using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonCollector.Web.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureAttachmentDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_PokemonCards_PokemonCardId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments",
                column: "CardSetId",
                principalTable: "CardSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_PokemonCards_PokemonCardId",
                table: "Attachments",
                column: "PokemonCardId",
                principalTable: "PokemonCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_PokemonCards_PokemonCardId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments",
                column: "CardSetId",
                principalTable: "CardSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_PokemonCards_PokemonCardId",
                table: "Attachments",
                column: "PokemonCardId",
                principalTable: "PokemonCards",
                principalColumn: "Id");
        }
    }
}
