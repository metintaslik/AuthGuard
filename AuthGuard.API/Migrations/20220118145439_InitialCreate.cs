using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthGuard.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "text", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "text", maxLength: 25, nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false, comment: "0: I Don't Want to Specify\n 1: Male\n 2: Female"),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
