using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestServer.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsTestPassed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsedTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestType = table.Column<int>(type: "int", nullable: false),
                    RightAnswers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserAnswers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsTestPassed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsedTests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsTestPassed", "Name" },
                values: new object[,]
                {
                    { 1, true, "User" },
                    { 2, false, "Bob" },
                    { 3, true, "Vadim" },
                    { 4, false, "David" },
                    { 5, true, "Anna" },
                    { 6, false, "Vasya" },
                    { 7, true, "Ivan" },
                    { 8, false, "Helen" },
                    { 9, true, "Igor" },
                    { 10, false, "Jane" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedTests_UserId",
                table: "UsedTests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedTests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
