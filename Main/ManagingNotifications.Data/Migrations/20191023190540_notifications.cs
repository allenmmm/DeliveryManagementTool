using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.ManagingNotifications.Data.Migrations
{
    public partial class notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                schema: "ManagingNotifications",
                table: "Notifications",
                newName: "Detail_CustomMessage");

            migrationBuilder.AddColumn<DateTime>(
                name: "Detail_DateRaised",
                schema: "ManagingNotifications",
                table: "Notifications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Detail_StatusId",
                schema: "ManagingNotifications",
                table: "Notifications",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "ManagingNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    NotificationState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "ManagingNotifications",
                table: "Statuses",
                columns: new[] { "Id", "Description", "NotificationState" },
                values: new object[,]
                {
                    { new Guid("a09e8195-763c-4b27-aaec-82ced2a41ac8"), "All expected orderbook(s) generated", 1 },
                    { new Guid("c38fc7ec-6b56-4a05-a069-0d4847d72626"), "The planned and overdues table could not be deleted,this will potentially corrupt future orderbook generations", 2 },
                    { new Guid("6a73fe66-4384-4789-9ce1-0887a0957076"), "No expected orderbook(s) generated", 3 },
                    { new Guid("bce9693d-05e0-4cd1-842f-cfe2b3a29eda"), "Orderbook not generated", 3 },
                    { new Guid("e4335e16-f8ac-4b95-8e7a-b5cefa2fcadf"), "Orderbook(s) generation terminated.  Planned and overdues table could not be validated/accessed", 3 },
                    { new Guid("6d7d561b-b477-4f7c-96ba-e3c878273ac9"), "Orderbook(s) generation started", 4 },
                    { new Guid("a26292d7-be2b-481d-aabb-09e97723a6d9"), "Expected orderbook generated", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "ManagingNotifications");

            migrationBuilder.DropColumn(
                name: "Detail_DateRaised",
                schema: "ManagingNotifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Detail_StatusId",
                schema: "ManagingNotifications",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Detail_CustomMessage",
                schema: "ManagingNotifications",
                table: "Notifications",
                newName: "Message");
        }
    }
}
