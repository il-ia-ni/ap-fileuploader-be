using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLibrary.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PRO");

            migrationBuilder.CreateTable(
                name: "DC_METADATA",
                schema: "PRO",
                columns: table => new
                {
                    ITEM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ITEM_SOURCE = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    HOST = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ITEM_CONTAINER = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ITEM_NAME = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ITEM_TYPE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ITEM_COMMENT = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UNIT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SCALING = table.Column<double>(type: "float", nullable: true),
                    UPDATE_CYCLE = table.Column<double>(type: "float", nullable: true),
                    SENSOR = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    MIN_VAL = table.Column<double>(type: "float", nullable: true),
                    MAX_VAL = table.Column<double>(type: "float", nullable: true),
                    ORIENTATION = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    LAST_MODIFIED_AT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DcMetadata", x => x.ITEM_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DC_METADATA",
                schema: "PRO");
        }
    }
}
