using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.ManagingNotifications.Data.Migrations
{
    public partial class AddedUnableToRetrieveOrderbookCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "ManagingNotifications",
                table: "Statuses",
                columns: new[] { "Id", "Description", "NotificationState" },
                values: new object[] { new Guid("c4faf266-389c-45a5-9b5a-3fe6fc18562e"), "Orderbook can not be retrieved, none exist or database error", 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("c4faf266-389c-45a5-9b5a-3fe6fc18562e"));
        }
    }
}
