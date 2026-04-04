using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixedhitbox.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    discord_id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    global_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    aredl_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    country = table.Column<int>(type: "INTEGER", nullable: true),
                    created_in_aredl_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    linked_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_update_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkedUsers_discord_id",
                table: "LinkedUsers",
                column: "discord_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkedUsers");
        }
    }
}
