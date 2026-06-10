using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonCollector.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonCardAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments");

            migrationBuilder.AlterColumn<int>(
                name: "CardSetId",
                table: "Attachments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PokemonCardId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PokemonCardId",
                table: "Attachments",
                column: "PokemonCardId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_PokemonCards_PokemonCardId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_PokemonCardId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "PokemonCardId",
                table: "Attachments");

            migrationBuilder.AlterColumn<int>(
                name: "CardSetId",
                table: "Attachments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_CardSets_CardSetId",
                table: "Attachments",
                column: "CardSetId",
                principalTable: "CardSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
