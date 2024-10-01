using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloMVCCore.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Language", table: "AspNetUsers");
        }
    }
}
