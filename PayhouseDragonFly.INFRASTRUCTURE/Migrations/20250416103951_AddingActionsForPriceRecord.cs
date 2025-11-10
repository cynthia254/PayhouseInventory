using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayhouseDragonFly.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class AddingActionsForPriceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivatedBy",
                table: "PriceRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateActivated",
                table: "PriceRecord",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "PriceRecord",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "PriceRecord",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "PriceRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PriceRecord",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PriceRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivatedBy",
                table: "PriceRecord");

            migrationBuilder.DropColumn(
                name: "DateActivated",
                table: "PriceRecord");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "PriceRecord");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "PriceRecord");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PriceRecord");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PriceRecord");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PriceRecord");
        }
    }
}
