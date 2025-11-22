using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class seedingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    categoryDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stockQuantity = table.Column<int>(type: "int", nullable: false),
                    categoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.productId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_categoryId",
                        column: x => x.categoryId,
                        principalTable: "Categories",
                        principalColumn: "categoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_Orders_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    cartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    productId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    dateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.cartItemId);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviws",
                columns: table => new
                {
                    reviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviws", x => x.reviewId);
                    table.ForeignKey(
                        name: "FK_Reviws_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviws_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    orderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: false),
                    productId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.orderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_orderId",
                        column: x => x.orderId,
                        principalTable: "Orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: false),
                    method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_orderId",
                        column: x => x.orderId,
                        principalTable: "Orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "categoryId", "categoryDescription", "categoryName" },
                values: new object[,]
                {
                    { 1, "Devices and gadgets", "Electronics" },
                    { 2, "Printed and digital books", "Books" },
                    { 3, "Apparel for men and women", "Clothing" },
                    { 4, "Home and kitchen", "Home" },
                    { 5, "Toys and games", "Toys" },
                    { 6, "Sporting goods", "Sports" },
                    { 7, "Beauty and personal care", "Beauty" },
                    { 8, "Garden and outdoor", "Garden" },
                    { 9, "Automotive parts and accessories", "Automotive" },
                    { 10, "Healthcare products", "Health" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "userId", "createdAt", "email", "password", "updatedAt", "userName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 21, 8, 50, 13, 0, DateTimeKind.Utc), "alice@example.com", "pass1", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "alice" },
                    { 2, new DateTime(2025, 9, 22, 8, 50, 13, 0, DateTimeKind.Utc), "bob@example.com", "pass2", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "bob" },
                    { 3, new DateTime(2025, 9, 23, 8, 50, 13, 0, DateTimeKind.Utc), "carol@example.com", "pass3", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "carol" },
                    { 4, new DateTime(2025, 9, 24, 8, 50, 13, 0, DateTimeKind.Utc), "dave@example.com", "pass4", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "dave" },
                    { 5, new DateTime(2025, 9, 25, 8, 50, 13, 0, DateTimeKind.Utc), "eve@example.com", "pass5", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "eve" },
                    { 6, new DateTime(2025, 9, 26, 8, 50, 13, 0, DateTimeKind.Utc), "frank@example.com", "pass6", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "frank" },
                    { 7, new DateTime(2025, 9, 27, 8, 50, 13, 0, DateTimeKind.Utc), "grace@example.com", "pass7", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "grace" },
                    { 8, new DateTime(2025, 9, 28, 8, 50, 13, 0, DateTimeKind.Utc), "heidi@example.com", "pass8", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "heidi" },
                    { 9, new DateTime(2025, 9, 29, 8, 50, 13, 0, DateTimeKind.Utc), "ivan@example.com", "pass9", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "ivan" },
                    { 10, new DateTime(2025, 9, 30, 8, 50, 13, 0, DateTimeKind.Utc), "judy@example.com", "pass10", new DateTime(2025, 10, 21, 8, 50, 13, 0, DateTimeKind.Utc), "judy" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "orderId", "createdDate", "status", "totalAmount", "userId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 11, 8, 50, 13, 0, DateTimeKind.Utc), "Completed", 150, 1 },
                    { 2, new DateTime(2025, 10, 12, 8, 50, 13, 0, DateTimeKind.Utc), "Processing", 299, 2 },
                    { 3, new DateTime(2025, 10, 13, 8, 50, 13, 0, DateTimeKind.Utc), "Shipped", 45, 3 },
                    { 4, new DateTime(2025, 10, 14, 8, 50, 13, 0, DateTimeKind.Utc), "Completed", 89, 4 },
                    { 5, new DateTime(2025, 10, 15, 8, 50, 13, 0, DateTimeKind.Utc), "Cancelled", 19, 5 },
                    { 6, new DateTime(2025, 10, 16, 8, 50, 13, 0, DateTimeKind.Utc), "Processing", 60, 6 },
                    { 7, new DateTime(2025, 10, 17, 8, 50, 13, 0, DateTimeKind.Utc), "Completed", 120, 7 },
                    { 8, new DateTime(2025, 10, 18, 8, 50, 13, 0, DateTimeKind.Utc), "Shipped", 230, 8 },
                    { 9, new DateTime(2025, 10, 19, 8, 50, 13, 0, DateTimeKind.Utc), "Completed", 15, 9 },
                    { 10, new DateTime(2025, 10, 20, 8, 50, 13, 0, DateTimeKind.Utc), "Processing", 75, 10 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "productId", "category", "categoryId", "description", "name", "price", "stockQuantity" },
                values: new object[,]
                {
                    { 1, "Electronics", 1, "Latest smartphone", "Smartphone", 699.99m, 50 },
                    { 2, "Electronics", 1, "Lightweight laptop", "Laptop", 1199.50m, 30 },
                    { 3, "Books", 2, "Bestselling novel", "Novel", 14.99m, 200 },
                    { 4, "Clothing", 3, "Cotton t-shirt", "T-Shirt", 19.99m, 150 },
                    { 5, "Home", 4, "Kitchen blender", "Blender", 49.99m, 40 },
                    { 6, "Toys", 5, "Family board game", "Board Game", 29.99m, 80 },
                    { 7, "Sports", 6, "Comfort running shoes", "Running Shoes", 89.99m, 60 },
                    { 8, "Beauty", 7, "Moisturizing cream", "Skin Cream", 24.99m, 120 },
                    { 9, "Garden", 8, "Durable hose", "Garden Hose", 34.99m, 70 },
                    { 10, "Health", 10, "Daily multivitamins", "Vitamins", 12.99m, 300 }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "cartItemId", "dateAdded", "productId", "quantity", "userId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 19, 8, 50, 13, 0, DateTimeKind.Utc), 3, 1, 1 },
                    { 2, new DateTime(2025, 10, 19, 8, 50, 13, 0, DateTimeKind.Utc), 4, 2, 2 },
                    { 3, new DateTime(2025, 10, 20, 8, 50, 13, 0, DateTimeKind.Utc), 5, 1, 3 },
                    { 4, new DateTime(2025, 10, 17, 8, 50, 13, 0, DateTimeKind.Utc), 6, 3, 4 },
                    { 5, new DateTime(2025, 10, 18, 8, 50, 13, 0, DateTimeKind.Utc), 7, 1, 5 },
                    { 6, new DateTime(2025, 10, 16, 8, 50, 13, 0, DateTimeKind.Utc), 8, 2, 6 },
                    { 7, new DateTime(2025, 10, 15, 8, 50, 13, 0, DateTimeKind.Utc), 9, 1, 7 },
                    { 8, new DateTime(2025, 10, 14, 8, 50, 13, 0, DateTimeKind.Utc), 10, 1, 8 },
                    { 9, new DateTime(2025, 10, 13, 8, 50, 13, 0, DateTimeKind.Utc), 1, 1, 9 },
                    { 10, new DateTime(2025, 10, 12, 8, 50, 13, 0, DateTimeKind.Utc), 2, 1, 10 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "orderItemId", "orderId", "price", "productId", "quantity" },
                values: new object[,]
                {
                    { 1, 1, 699.99m, 1, 1 },
                    { 2, 2, 1199.50m, 2, 1 },
                    { 3, 3, 14.99m, 3, 2 },
                    { 4, 4, 19.99m, 4, 3 },
                    { 5, 5, 49.99m, 5, 1 },
                    { 6, 6, 29.99m, 6, 1 },
                    { 7, 7, 89.99m, 7, 1 },
                    { 8, 8, 24.99m, 8, 2 },
                    { 9, 9, 34.99m, 9, 1 },
                    { 10, 10, 12.99m, 10, 5 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "paymentId", "amount", "createdAt", "method", "orderId", "status" },
                values: new object[,]
                {
                    { 1, 150m, new DateTime(2025, 10, 11, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 1, "Paid" },
                    { 2, 299m, new DateTime(2025, 10, 12, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 2, "Pending" },
                    { 3, 45m, new DateTime(2025, 10, 13, 8, 50, 13, 0, DateTimeKind.Utc), "PayPal", 3, "Paid" },
                    { 4, 89m, new DateTime(2025, 10, 14, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 4, "Paid" },
                    { 5, 19m, new DateTime(2025, 10, 15, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 5, "Refunded" },
                    { 6, 60m, new DateTime(2025, 10, 16, 8, 50, 13, 0, DateTimeKind.Utc), "PayPal", 6, "Pending" },
                    { 7, 120m, new DateTime(2025, 10, 17, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 7, "Paid" },
                    { 8, 230m, new DateTime(2025, 10, 18, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 8, "Paid" },
                    { 9, 15m, new DateTime(2025, 10, 19, 8, 50, 13, 0, DateTimeKind.Utc), "PayPal", 9, "Paid" },
                    { 10, 75m, new DateTime(2025, 10, 20, 8, 50, 13, 0, DateTimeKind.Utc), "Card", 10, "Pending" }
                });

            migrationBuilder.InsertData(
                table: "Reviws",
                columns: new[] { "reviewId", "CreatedAt", "comment", "productId", "rating", "userId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 1, 8, 50, 13, 0, DateTimeKind.Utc), "Excellent", 1, 5, 1 },
                    { 2, new DateTime(2025, 10, 2, 8, 50, 13, 0, DateTimeKind.Utc), "Very good", 2, 4, 2 },
                    { 3, new DateTime(2025, 10, 3, 8, 50, 13, 0, DateTimeKind.Utc), "Average", 3, 3, 3 },
                    { 4, new DateTime(2025, 10, 4, 8, 50, 13, 0, DateTimeKind.Utc), "Nice", 4, 5, 4 },
                    { 5, new DateTime(2025, 10, 5, 8, 50, 13, 0, DateTimeKind.Utc), "Not great", 5, 2, 5 },
                    { 6, new DateTime(2025, 10, 6, 8, 50, 13, 0, DateTimeKind.Utc), "Good", 6, 4, 6 },
                    { 7, new DateTime(2025, 10, 7, 8, 50, 13, 0, DateTimeKind.Utc), "Love it", 7, 5, 7 },
                    { 8, new DateTime(2025, 10, 8, 8, 50, 13, 0, DateTimeKind.Utc), "Okay", 8, 3, 8 },
                    { 9, new DateTime(2025, 10, 9, 8, 50, 13, 0, DateTimeKind.Utc), "Useful", 9, 4, 9 },
                    { 10, new DateTime(2025, 10, 10, 8, 50, 13, 0, DateTimeKind.Utc), "Recommended", 10, 5, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_productId",
                table: "CartItems",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_userId",
                table: "CartItems",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_orderId",
                table: "OrderItems",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_productId",
                table: "OrderItems",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_userId",
                table: "Orders",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_orderId",
                table: "Payments",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_categoryId",
                table: "Products",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviws_productId",
                table: "Reviws",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviws_userId",
                table: "Reviws",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reviws");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
