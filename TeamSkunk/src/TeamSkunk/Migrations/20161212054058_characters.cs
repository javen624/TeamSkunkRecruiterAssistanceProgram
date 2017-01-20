using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamSkunk.Migrations
{
    public partial class characters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllyCode",
                table: "Characters");

            migrationBuilder.AddColumn<int>(
                name: "Gear",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SevenStarG10",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stars",
                table: "Characters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_MemberId",
                table: "Characters",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Members_MemberId",
                table: "Characters",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Members_MemberId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_MemberId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Gear",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SevenStarG10",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Stars",
                table: "Characters");

            migrationBuilder.AddColumn<string>(
                name: "AllyCode",
                table: "Characters",
                nullable: true);
        }
    }
}
