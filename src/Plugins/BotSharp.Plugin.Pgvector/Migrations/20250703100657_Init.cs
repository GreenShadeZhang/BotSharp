using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace BotSharp.Plugin.Pgvector.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "vector_collections",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Dimension = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IndexType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "hnsw"),
                    DistanceFunction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "cosine"),
                    IsIndexed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vector_collections", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "vector_data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CollectionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    DataSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "api"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vector_data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vector_data_vector_collections_CollectionName",
                        column: x => x.CollectionName,
                        principalTable: "vector_collections",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vector_collections_CreatedAt",
                table: "vector_collections",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_vector_collections_Type",
                table: "vector_collections",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_vector_data_CollectionName",
                table: "vector_data",
                column: "CollectionName");

            migrationBuilder.CreateIndex(
                name: "IX_vector_data_CreatedAt",
                table: "vector_data",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_vector_data_DataSource",
                table: "vector_data",
                column: "DataSource");

            migrationBuilder.CreateIndex(
                name: "IX_vector_data_PayloadJson",
                table: "vector_data",
                column: "PayloadJson")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vector_data");

            migrationBuilder.DropTable(
                name: "vector_collections");
        }
    }
}
