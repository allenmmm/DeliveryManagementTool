using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.ManagingNotifications.Data.Migrations
{
    public partial class NewOrderBookPreviewErrorStatusCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "ManagingNotifications",
                table: "Statuses",
                columns: new[] { "Id", "Description", "NotificationState" },
                values: new object[] { new Guid("7d625937-1cc2-4dc5-a3ce-2223b3444e18"), "Orderbook previews can not be retrieved, none exist or database error", 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("7d625937-1cc2-4dc5-a3ce-2223b3444e18"));
        }
    }
}
