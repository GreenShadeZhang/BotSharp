using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace BotSharp.Plugin.Pgvector.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "vector_data",
                type: "vector(384)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "vector_data",
                type: "vector",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(384)");
        }
    }
}
