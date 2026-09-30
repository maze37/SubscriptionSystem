using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveSubscriptionConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "billing_period",
                table: "invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "plan_id",
                table: "invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "purpose",
                table: "invoices",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE invoices AS i
                SET plan_id = s.plan_id,
                    billing_period = p.billing_period,
                    purpose = 'InitialSubscription'
                FROM subscriptions AS s
                INNER JOIN plans AS p ON p.id = s.plan_id
                WHERE i."SubscriptionId" = s.id;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "billing_period",
                table: "invoices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "plan_id",
                table: "invoices",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "purpose",
                table: "invoices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ux_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_subscriptions_active_user",
                table: "subscriptions",
                column: "user_id",
                unique: true,
                filter: "\"status\" IN ('PendingPayment', 'Trial', 'Active', 'PastDue')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_users_email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ux_subscriptions_active_user",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "billing_period",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "plan_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "purpose",
                table: "invoices");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");
        }
    }
}
