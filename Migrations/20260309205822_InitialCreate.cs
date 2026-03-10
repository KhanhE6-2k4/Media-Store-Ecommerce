using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaStore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryInfo",
                columns: table => new
                {
                    delivery_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    province = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryInfo", x => x.delivery_id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    totalQuantity = table.Column<int>(type: "int", nullable: false),
                    weight = table.Column<double>(type: "float", nullable: false),
                    rushOrderSupported = table.Column<bool>(type: "bit", nullable: true),
                    imageUrl = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    barcode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    productDimension = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    importDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.media_id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    paymentAmount = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    bankTransactionId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cardType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransaction", x => x.transaction_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", unicode: false, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfo",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shippingFees = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    delivery_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfo", x => x.order_id);
                    table.ForeignKey(
                        name: "FK__OrderInfo__deliv__45F365D3",
                        column: x => x.delivery_id,
                        principalTable: "DeliveryInfo",
                        principalColumn: "delivery_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false),
                    authors = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    coverType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    publisher = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    publicationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    pages = table.Column<int>(type: "int", nullable: true),
                    language = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    genre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.media_id);
                    table.ForeignKey(
                        name: "FK__Book__media_id__3B75D760",
                        column: x => x.media_id,
                        principalTable: "Media",
                        principalColumn: "media_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CD_and_LP",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false),
                    isCD = table.Column<bool>(type: "bit", nullable: false),
                    artists = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    recordLabel = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    trackList = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    genre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    releaseDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CD_and_LP", x => x.media_id);
                    table.ForeignKey(
                        name: "FK__CD_and_LP__media__3E52440B",
                        column: x => x.media_id,
                        principalTable: "Media",
                        principalColumn: "media_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DVD",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false),
                    dvdType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    director = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    runtime = table.Column<int>(type: "int", nullable: false),
                    studio = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    language = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    subtitles = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    releasedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    genre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DVD", x => x.media_id);
                    table.ForeignKey(
                        name: "FK__DVD__media_id__412EB0B6",
                        column: x => x.media_id,
                        principalTable: "Media",
                        principalColumn: "media_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    invoice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    totalAmount = table.Column<int>(type: "int", nullable: false),
                    transaction_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.invoice_id);
                    table.ForeignKey(
                        name: "FK__Invoice__order_i__52593CB8",
                        column: x => x.order_id,
                        principalTable: "OrderInfo",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Invoice__transac__5165187F",
                        column: x => x.transaction_id,
                        principalTable: "PaymentTransaction",
                        principalColumn: "transaction_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Media",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    orderType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Media", x => new { x.media_id, x.order_id });
                    table.ForeignKey(
                        name: "FK__Order_Med__media__48CFD27E",
                        column: x => x.media_id,
                        principalTable: "Media",
                        principalColumn: "media_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Order_Med__order__49C3F6B7",
                        column: x => x.order_id,
                        principalTable: "OrderInfo",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RushOrderInfo",
                columns: table => new
                {
                    rush_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deliveryTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    instruction = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RushOrderInfo", x => x.rush_id);
                    table.ForeignKey(
                        name: "FK__RushOrder__order__4CA06362",
                        column: x => x.order_id,
                        principalTable: "OrderInfo",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "invoice_order_id_index",
                table: "Invoice",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "invoice_transaction_id_index",
                table: "Invoice",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Media_order_id",
                table: "Order_Media",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "delivery_id_index",
                table: "OrderInfo",
                column: "delivery_id");

            migrationBuilder.CreateIndex(
                name: "rush_order_id_index",
                table: "RushOrderInfo",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropTable(
                name: "CD_and_LP");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "DVD");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Order_Media");

            migrationBuilder.DropTable(
                name: "RushOrderInfo");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "PaymentTransaction");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "OrderInfo");

            migrationBuilder.DropTable(
                name: "DeliveryInfo");
        }
    }
}
