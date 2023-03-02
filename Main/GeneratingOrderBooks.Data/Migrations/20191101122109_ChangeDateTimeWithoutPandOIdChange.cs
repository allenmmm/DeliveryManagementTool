using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.GeneratingOrderbooks.Data.Migrations
{
    public partial class ChangeDateTimeWithoutPandOIdChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StatDeliverySchedule",
                table: "PlannedAndOverdueOrders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ItemDeliveryDate",
                table: "PlannedAndOverdueOrders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePulled",
                table: "PlannedAndOverdueOrders",
                type: "smalldatetime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Details_StatDeliverySchedule",
                table: "Orders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Details_PurchaseOrder",
                table: "Orders",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Details_ItemDeliveryDate",
                table: "Orders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Orders",
                maxLength: 23,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 21);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateStamp_DatePulled",
                table: "Orderbooks",
                type: "smalldatetime",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StatDeliverySchedule",
                table: "PlannedAndOverdueOrders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ItemDeliveryDate",
                table: "PlannedAndOverdueOrders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePulled",
                table: "PlannedAndOverdueOrders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Details_StatDeliverySchedule",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Details_PurchaseOrder",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Details_ItemDeliveryDate",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Orders",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 23);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateStamp_DatePulled",
                table: "Orderbooks",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");
        }
    }
}
