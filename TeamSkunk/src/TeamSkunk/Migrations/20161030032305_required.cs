using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamSkunk.Migrations
{
    public partial class required : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiscordName",
                table: "Person",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Members",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Guilds",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiscordName",
                table: "Person",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Members",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Guilds",
                nullable: true);
        }
    }
}
