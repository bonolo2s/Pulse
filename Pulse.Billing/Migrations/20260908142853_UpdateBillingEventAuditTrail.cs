using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Billing.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillingEventAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingEvents_Payments_PaymentId",
                table: "BillingEvents");

            migrationBuilder.DropIndex(
                name: "IX_BillingEvents_PaymentId",
                table: "BillingEvents");

            migrationBuilder.DropIndex(
                name: "IX_BillingEvents_PaystackEventId",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "BillingEvents");

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "BillingEvents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingEvents_PaymentReference",
                table: "BillingEvents",
                column: "PaymentReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BillingEvents_PaymentReference",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "BillingEvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "BillingEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "BillingEvents",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "BillingEvents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingEvents_PaymentId",
                table: "BillingEvents",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingEvents_PaystackEventId",
                table: "BillingEvents",
                column: "PaystackEventId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BillingEvents_Payments_PaymentId",
                table: "BillingEvents",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
