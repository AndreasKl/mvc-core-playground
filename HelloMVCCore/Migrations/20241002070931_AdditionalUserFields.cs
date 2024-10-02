using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloMVCCore.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT"
            );

            migrationBuilder.AddColumn<int>(
                name: "CurrentThingID",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "CurrentThingRole",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 200,
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CurrentThingID", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "CurrentThingRole", table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true
            );
        }
    }
}
