using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.ManagingNotifications.Data.Migrations
{
    public partial class BackendChangesForGettingOrderbookWeeks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "ManagingNotifications",
                table: "Statuses",
                columns: new[] { "Id", "Description", "NotificationState" },
                values: new object[] { new Guid("4d623d87-0afd-44fd-9b5a-be2d164f891e"), "Orderbook weeks can not be retrieved, unable to query for generated orderbooks", 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ManagingNotifications",
                table: "Statuses",
                keyColumn: "Id",
                keyValue: new Guid("4d623d87-0afd-44fd-9b5a-be2d164f891e"));
        }
    }
}
