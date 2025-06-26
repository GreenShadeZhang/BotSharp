using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotSharp.Plugin.PostgreSqlFileStorage.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file_storages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    file_data = table.Column<byte[]>(type: "bytea", nullable: true),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    directory = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_storages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_file_storages_category",
                table: "file_storages",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_file_storages_created_at",
                table: "file_storages",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_file_storages_directory",
                table: "file_storages",
                column: "directory");

            migrationBuilder.CreateIndex(
                name: "IX_file_storages_entity_id",
                table: "file_storages",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_file_storages_file_path",
                table: "file_storages",
                column: "file_path",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_storages");
        }
    }
}
