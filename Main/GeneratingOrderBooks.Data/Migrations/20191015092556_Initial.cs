using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMT.GeneratingOrderbooks.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlannedAndOverdueOrders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    SupplierName = table.Column<string>(maxLength: 80, nullable: true),
                    PartNumber = table.Column<string>(maxLength: 80, nullable: true),
                    PartDescription = table.Column<string>(maxLength: 100, nullable: true),
                    PurchaseOrder = table.Column<string>(maxLength: 8, nullable: true),
                    POLineItem = table.Column<int>(nullable: false),
                    POSchedLine = table.Column<int>(nullable: false),
                    OpenPOQty = table.Column<int>(nullable: false),
                    ItemDeliveryDate = table.Column<DateTime>(nullable: false),
                    StatDeliverySchedule = table.Column<DateTime>(nullable: false),
                    DatePulled = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedAndOverdueOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(fixedLength: true, maxLength: 12, nullable: false),
                    Details_Name = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orderbooks",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    DateStamp_DatePulled = table.Column<DateTime>(nullable: false),
                    DateStamp_DateCreated = table.Column<DateTime>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orderbooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orderbooks_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 21, nullable: false),
                    OrderbookId = table.Column<decimal>(nullable: false),
                    Details_PurchaseOrder = table.Column<string>(maxLength: 8, nullable: true),
                    Details_POLineItem = table.Column<int>(nullable: false),
                    Details_POSchedLine = table.Column<int>(nullable: false),
                    Details_OpenPOQty = table.Column<int>(nullable: false),
                    Details_ItemDeliveryDate = table.Column<DateTime>(nullable: false),
                    Details_StatDeliverySchedule = table.Column<DateTime>(nullable: false),
                    Part_Number = table.Column<string>(maxLength: 80, nullable: true),
                    Part_Description = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Orderbooks_OrderbookId",
                        column: x => x.OrderbookId,
                        principalTable: "Orderbooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orderbooks_SupplierId",
                table: "Orderbooks",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderbookId",
                table: "Orders",
                column: "OrderbookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PlannedAndOverdueOrders");

            migrationBuilder.DropTable(
                name: "Orderbooks");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
