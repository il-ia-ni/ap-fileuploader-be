using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLibrary.Migrations
{
    public partial class AddAuthTMIUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "DC_METADATA_PK",
                schema: "PRO",
                table: "DC_METADATA",
                column: "ITEM_ID");

            migrationBuilder.CreateTable(
                name: "TMI_USER",
                schema: "PRO",
                columns: table => new
                {
                    USERNAME = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FIRST_NAME = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    LAST_NAME = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    LANDING_PAGE = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ROLE_NAME = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    LAST_MODIFIED_AT = table.Column<DateTime>(type: "datetime", nullable: false),
                    PASSWORD_HASH = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    PASSWORD_SALT = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TMI_USER_PK", x => x.USERNAME);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TMI_USER",
                schema: "PRO");

            migrationBuilder.DropPrimaryKey(
                name: "DC_METADATA_PK",
                schema: "PRO",
                table: "DC_METADATA");
        }
    }
}
