using Microsoft.EntityFrameworkCore.Migrations;

namespace Avito2.Migrations
{
    public partial class AdsUpd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreationAuthorId",
                table: "Advertisements",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationAuthorId",
                table: "Advertisements");
        }
    }
}
