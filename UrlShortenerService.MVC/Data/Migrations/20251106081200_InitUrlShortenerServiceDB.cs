using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrlShortenerService.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitUrlShortenerServiceDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrls", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ShortUrls",
                columns: new[] { "Id", "CreatedAt", "OriginalUrl", "ShortCode" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 6, 7, 44, 28, 0, DateTimeKind.Utc), "https://www.microsoft.com", "msft2025" },
                    { 2, new DateTime(2025, 11, 6, 7, 44, 29, 0, DateTimeKind.Utc), "https://www.github.com", "gh2025" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortUrls");
        }
    }
}
