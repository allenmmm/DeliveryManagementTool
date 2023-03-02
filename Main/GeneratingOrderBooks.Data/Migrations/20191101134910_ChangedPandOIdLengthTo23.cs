using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.GeneratingOrderbooks.Data.Migrations
{
    public partial class ChangedPandOIdLengthTo23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint("PK_PlannedAndOverdueOrders",
                "PlannedAndOverdueOrders");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PlannedAndOverdueOrders",
                maxLength: 23,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey("PK_PlannedAndOverdueOrders",
                 "PlannedAndOverdueOrders",
                 "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PlannedAndOverdueOrders",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 23);
        }
    }
}
