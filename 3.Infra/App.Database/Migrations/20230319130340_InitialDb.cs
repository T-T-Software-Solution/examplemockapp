using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aliens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "คีร์ของข้อมูล", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true, comment: "Alien Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    species = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true, comment: "Species")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    originplanet = table.Column<string>(name: "origin_planet", type: "varchar(4000)", maxLength: 4000, nullable: true, comment: "Origin Planet")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "เวลาสร้าง"),
                    updated = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "เวลาปรับปรุงล่าสุด"),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "ใช้งานได้หรือไม่")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aliens", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sightings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "คีร์ของข้อมูล", collation: "ascii_general_ci"),
                    alienid = table.Column<Guid>(name: "alien_id", type: "char(36)", nullable: true, comment: "Alien", collation: "ascii_general_ci"),
                    founddate = table.Column<DateTime>(name: "found_date", type: "datetime(6)", nullable: true, comment: "Found Date"),
                    location = table.Column<string>(type: "longtext", nullable: true, comment: "Location")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    witness = table.Column<string>(type: "longtext", nullable: true, comment: "Witness")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "เวลาสร้าง"),
                    updated = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "เวลาปรับปรุงล่าสุด"),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "ใช้งานได้หรือไม่")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sightings", x => x.id);
                    table.ForeignKey(
                        name: "FK_sightings_aliens_alien_id",
                        column: x => x.alienid,
                        principalTable: "aliens",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_sightings_alien_id",
                table: "sightings",
                column: "alien_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sightings");

            migrationBuilder.DropTable(
                name: "aliens");
        }
    }
}
