using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingEventAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaystackEventId",
                table: "BillingEvents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Payload",
                table: "BillingEvents",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "NewStatus",
                table: "BillingEvents",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "BillingEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousStatus",
                table: "BillingEvents",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "BillingEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BillingEvents_PaymentId",
                table: "BillingEvents",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingEvents_Payments_PaymentId",
                table: "BillingEvents",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingEvents_Payments_PaymentId",
                table: "BillingEvents");

            migrationBuilder.DropIndex(
                name: "IX_BillingEvents_PaymentId",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "BillingEvents");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "BillingEvents");

            migrationBuilder.AlterColumn<string>(
                name: "PaystackEventId",
                table: "BillingEvents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Payload",
                table: "BillingEvents",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
