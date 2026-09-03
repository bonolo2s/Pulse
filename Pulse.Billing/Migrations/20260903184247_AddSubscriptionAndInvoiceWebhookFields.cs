using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Billing.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionAndInvoiceWebhookFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailToken",
                table: "Subscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaystackCustomerCode",
                table: "Subscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCode",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailToken",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PaystackCustomerCode",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "InvoiceCode",
                table: "Invoices");
        }
    }
}
