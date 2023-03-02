using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.GeneratingOrderbooks.Data.Migrations
{
    public partial class notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Details_PurchaseOrder",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 8,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Details_PurchaseOrder",
                table: "Orders",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
