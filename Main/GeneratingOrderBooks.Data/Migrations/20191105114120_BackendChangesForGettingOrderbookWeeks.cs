using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.GeneratingOrderbooks.Data.Migrations
{
    public partial class BackendChangesForGettingOrderbookWeeks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Orders",
                newName: "PandOId");

            migrationBuilder.AddColumn<string>(
                name: "DateStamp_OrderbookWeek",
                table: "Orderbooks",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                columns: new[] { "PandOId", "OrderbookId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateStamp_OrderbookWeek",
                table: "Orderbooks");

            migrationBuilder.RenameColumn(
                name: "PandOId",
                table: "Orders",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");
        }
    }
}
