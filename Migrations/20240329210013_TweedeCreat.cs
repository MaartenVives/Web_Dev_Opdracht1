using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opdracht_1.Migrations
{
    public partial class TweedeCreat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tickets_TicketsId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TicketsId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketsId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_TicketId",
                table: "OrderDetails",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Tickets_TicketId",
                table: "OrderDetails",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Tickets_TicketId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_TicketId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "TicketsId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TicketsId",
                table: "Orders",
                column: "TicketsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tickets_TicketsId",
                table: "Orders",
                column: "TicketsId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
