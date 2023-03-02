using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.ManagingNotifications.Data.Migrations
{
    public partial class UpdatedStausCodeMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("6a73fe66-4384-4789-9ce1-0887a0957076"),
                column: "Description",
                value: "Errors occurred during generating of orderbook(s)");

            migrationBuilder.UpdateData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("a26292d7-be2b-481d-aabb-09e97723a6d9"),
                column: "Description",
                value: "Orderbook generated");

            migrationBuilder.UpdateData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("e4335e16-f8ac-4b95-8e7a-b5cefa2fcadf"),
                column: "Description",
                value: "Orderbook(s) generation terminated.  Planned and overdues table empty or could not be validated/accessed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("6a73fe66-4384-4789-9ce1-0887a0957076"),
                column: "Description",
                value: "No expected orderbook(s) generated");

            migrationBuilder.UpdateData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("a26292d7-be2b-481d-aabb-09e97723a6d9"),
                column: "Description",
                value: "Expected orderbook generated");

            migrationBuilder.UpdateData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("e4335e16-f8ac-4b95-8e7a-b5cefa2fcadf"),
                column: "Description",
                value: "Orderbook(s) generation terminated.  Planned and overdues table could not be validated/accessed");
        }
    }
}
