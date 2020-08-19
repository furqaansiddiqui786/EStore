using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlipShop_OnlineShopping.Data.Migrations
{
    public partial class AddProductsToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductsModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    About = table.Column<string>(nullable: false),
                    ProductPhoto = table.Column<byte[]>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsModel_categoriesModels_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categoriesModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsModel_CategoryId",
                table: "ProductsModel",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsModel");
        }
    }
}
