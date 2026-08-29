using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QrBin.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_BillReturns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillReturn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    BillId = table.Column<int>(type: "integer", nullable: false),
                    BillItemId = table.Column<int>(type: "integer", nullable: false),
                    ReturnedQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReplacementProductId = table.Column<int>(type: "integer", nullable: true),
                    ReplacementQuantity = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillReturn_BillItem_BillItemId",
                        column: x => x.BillItemId,
                        principalTable: "BillItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillReturn_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillReturn_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillReturn_Product_ReplacementProductId",
                        column: x => x.ReplacementProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillReturn_BillId",
                table: "BillReturn",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_BillReturn_BillItemId",
                table: "BillReturn",
                column: "BillItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BillReturn_OrganizationId",
                table: "BillReturn",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_BillReturn_ReplacementProductId",
                table: "BillReturn",
                column: "ReplacementProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillReturn");
        }
    }
}
