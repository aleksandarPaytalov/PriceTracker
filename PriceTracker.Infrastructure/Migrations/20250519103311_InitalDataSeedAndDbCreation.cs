using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitalDataSeedAndDbCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                },
                comment: "User DB Model");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false, comment: "Product identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Product quantity"),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Product name"),
                    Brand = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Product brand"),
                    Category = table.Column<int>(type: "int", maxLength: 255, nullable: false, comment: "Product category")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                },
                comment: "Product Db model");

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false, comment: "Store identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Store name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                },
                comment: "Store Db model");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBudgets",
                columns: table => new
                {
                    BudgedId = table.Column<int>(type: "int", nullable: false, comment: "Budged identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "User identifier"),
                    BudgetAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, comment: "Total amount of money or budged we have for the current month"),
                    Month = table.Column<int>(type: "int", nullable: false, comment: "Month we spend current budged in")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBudgets", x => x.BudgedId);
                    table.ForeignKey(
                        name: "FK_MonthlyBudgets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "MonthlyBudget Db model");

            migrationBuilder.CreateTable(
                name: "ToDoItems",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false, comment: "Task identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "User identifier"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Title of the current task"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Description of the current task"),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Whitin the day that current task must be finished"),
                    Priority = table.Column<int>(type: "int", nullable: false, comment: "Task priority level"),
                    TaskStatus = table.Column<int>(type: "int", nullable: false, comment: "Task status"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The date that task is created")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItems", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_ToDoItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                },
                comment: "Tasks Db model");

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false, comment: "Expense identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "User identifier"),
                    ExpenseType = table.Column<int>(type: "int", nullable: false, comment: "Expense type"),
                    ProductId = table.Column<int>(type: "int", nullable: false, comment: "Product identifier"),
                    StoreId = table.Column<int>(type: "int", nullable: false, comment: "Store identifier"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Description for maked expense"),
                    AmountSpent = table.Column<decimal>(type: "decimal(10,2)", nullable: false, comment: "Amount of money spent"),
                    DateSpent = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date when the expense was made")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Expense Db model");

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PriceId = table.Column<int>(type: "int", nullable: false, comment: "Price identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false, comment: "Product identifier"),
                    StoreId = table.Column<int>(type: "int", nullable: false, comment: "Store identifier"),
                    SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false, comment: "Current price of a product in the store"),
                    DateChecked = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "The date of the record for the price on a product")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PriceId);
                    table.ForeignKey(
                        name: "FK_Prices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prices_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Price Db model");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false, comment: "Notification identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "User identifier"),
                    TaskId = table.Column<int>(type: "int", nullable: false, comment: "Task identifier"),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Notification message"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, comment: "Track if the message is readed or not"),
                    NotificationTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Time of the notification - due Date"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Time when the notification was created")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_ToDoItems_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ToDoItems",
                        principalColumn: "TaskId");
                },
                comment: "Notification Db model");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f59", "7c1707e8-8f7e-4e75-915c-fc70f06358b3", "Administrator", "ADMINISTRATOR" },
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f60", "465593d2-ddaf-45dc-a1d2-af9247c94f76", "User", "USER" },
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f61", "ba25e161-7171-4e2c-8461-d7d615f6e685", "Guest", "GUEST" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "56f4b198-3f3e-4f72-9a80-7d903bf24a1e", 0, "38e6855b-4c3b-441b-b99f-d25ac606e3b4", "admin@mail.com", false, false, null, "ADMIN@MAIL.COM", "ADMIN@MAIL.COM", "AQAAAAIAAYagAAAAEChV3s80DpUHCQxoTrErIVHXJqDEvRkLOAFT5xl3UwwzCwDk+/WAzkcNdEsaqPF58Q==", null, false, "3a5e2a1f-7d8c-48a2-8edb-520203064fef", false, "admin@mail.com" },
                    { "8d1f0bdc-f59a-4c8d-9549-98673c32c25d", 0, "6bde1c9b-f966-4f2e-a16e-a589922a62fb", "user@mail.com", false, false, null, "USER@MAIL.COM", "USER@MAIL.COM", "AQAAAAIAAYagAAAAEN0Lir9KBA1yR/yajkXuX8lUCIQhfGitZ9Muj6hGH1dlIefAzynAayEdE8VH4rWPbQ==", null, false, "e3839d9d-bcee-4c9b-84c5-ccf637d49b62", false, "user@mail.com" },
                    { "cf41999b-9cad-4b75-977d-a2fdb3d02e77", 0, "986e542c-d39f-4e54-9e25-7bf36b504214", "guest@mail.com", false, false, null, "GUEST@MAIL.COM", "GUEST@MAIL.COM", "AQAAAAIAAYagAAAAEMt2rKCgSayjQhiYsJD025Esdv3M3nKFcypJW660Q+DtBdHAB7OkPsEt+sPxHsXvVw==", null, false, "f35d6841-2d05-4626-905d-673c28968610", false, "guest@mail.com" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Brand", "Category", "ProductName", "Quantity" },
                values: new object[,]
                {
                    { 1, "Нашата ферма", 3, "Яйца", 10 },
                    { 2, "Маджаров", 3, "Кашкавал от краве мляко", 400 },
                    { 3, "Лакрима", 3, "Сирене", 700 },
                    { 4, " А95", 6, "Бензин", 1 },
                    { 5, "Верея", 3, "Кисело мляко 2%", 400 },
                    { 6, "Орехите", 3, "Филе Елена", 100 },
                    { 7, "Auto-gas", 6, "Газ", 1 },
                    { 8, "Добруджа", 3, "Хляб с лимец", 500 },
                    { 9, "Верея", 3, "Прясно мляко лешник", 1000 }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "StoreId", "Name" },
                values: new object[,]
                {
                    { 1, "Kaufland" },
                    { 2, "Lidl" },
                    { 3, "Billa" },
                    { 4, "Metro" },
                    { 5, "BBB" },
                    { 6, "T-Market" },
                    { 7, "Marketplace" },
                    { 8, "Lukoil" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f59", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f60", "8d1f0bdc-f59a-4c8d-9549-98673c32c25d" },
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f61", "cf41999b-9cad-4b75-977d-a2fdb3d02e77" }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "ExpenseId", "AmountSpent", "DateSpent", "Description", "ExpenseType", "ProductId", "StoreId", "UserId" },
                values: new object[,]
                {
                    { 1, 31.59m, new DateTime(2025, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Заредена газ за пътуване до София", 5, 7, 8, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, 4.50m, new DateTime(2025, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Покупка на яйца размер L", 2, 1, 1, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, 14.50m, new DateTime(2025, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Кашкавал", 2, 2, 1, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 4, 50.00m, new DateTime(2025, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Бензин", 5, 4, 8, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 5, 12.50m, new DateTime(2025, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Сирене за баница", 2, 3, 5, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 6, 12.50m, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Месо за мезе", 2, 6, 2, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.InsertData(
                table: "MonthlyBudgets",
                columns: new[] { "BudgedId", "BudgetAmount", "Month", "UserId" },
                values: new object[,]
                {
                    { 1, 1200m, 4, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, 1300m, 5, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, 1300m, 6, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "PriceId", "DateChecked", "ProductId", "SellingPrice", "StoreId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4.50m, 1 },
                    { 2, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 17.59m, 5 },
                    { 3, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 16.00m, 1 },
                    { 4, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 2.55m, 8 },
                    { 5, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1.55m, 3 },
                    { 6, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 24.99m, 1 },
                    { 7, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 1.25m, 8 },
                    { 8, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 2.50m, 5 },
                    { 9, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 5.50m, 3 }
                });

            migrationBuilder.InsertData(
                table: "ToDoItems",
                columns: new[] { "TaskId", "CreatedAt", "Description", "DueDate", "Priority", "TaskStatus", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Buy food for the next two weeks", new DateTime(2025, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Shopping", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Get the car ready for the trip", new DateTime(2025, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Fill with fuel", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pay the bills for this month.", new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "House bills", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationId", "CreatedAt", "IsRead", "Message", "NotificationTime", "TaskId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Reminder to buy a food for next two week", new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Reminder to fill up the car with gas and check it, so it is ready for the trip next week.", new DateTime(2025, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Check and pay the bills", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ProductId",
                table: "Expenses",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_StoreId",
                table: "Expenses",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudgets_UserId_Month",
                table: "MonthlyBudgets",
                columns: new[] { "UserId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TaskId",
                table: "Notifications",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ProductId_StoreId_DateChecked",
                table: "Prices",
                columns: new[] { "ProductId", "StoreId", "DateChecked" },
                unique: true,
                filter: "[DateChecked] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_StoreId",
                table: "Prices",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Name",
                table: "Stores",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_UserId",
                table: "ToDoItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "MonthlyBudgets");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ToDoItems");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
