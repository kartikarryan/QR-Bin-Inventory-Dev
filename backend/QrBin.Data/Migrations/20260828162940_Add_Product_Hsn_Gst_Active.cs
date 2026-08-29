using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrBin.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Product_Hsn_Gst_Active : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GstRate",
                table: "Product",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 18m);

            migrationBuilder.AddColumn<string>(
                name: "HsnCode",
                table: "Product",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GstRate",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "HsnCode",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Product");
        }
    }
}
