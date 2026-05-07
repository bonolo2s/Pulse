using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.StatusPages.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatusPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusPageEndpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusPageId = table.Column<Guid>(type: "uuid", nullable: false),
                    EndpointId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusPageEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusPageEndpoints_StatusPages_StatusPageId",
                        column: x => x.StatusPageId,
                        principalTable: "StatusPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusPageEndpoints_EndpointId",
                table: "StatusPageEndpoints",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusPageEndpoints_StatusPageId",
                table: "StatusPageEndpoints",
                column: "StatusPageId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusPages_Slug",
                table: "StatusPages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusPages_UserId",
                table: "StatusPages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusPageEndpoints");

            migrationBuilder.DropTable(
                name: "StatusPages");
        }
    }
}
