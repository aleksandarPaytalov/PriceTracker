using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreationOfDataBaseWithJson : Migration
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
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f59", "ca08276f-4252-4392-beaf-3c2675e893b7", "Administrator", "ADMINISTRATOR" },
                    { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f60", "464a71ab-5a1a-4bee-85d7-5cd0d8beac63", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "56f4b198-3f3e-4f72-9a80-7d903bf24a1e", 0, "fff35fac-e197-47b2-8576-ff2c8073d7a4", "admin@mail.com", true, false, null, "ADMIN@MAIL.COM", "ADMIN@MAIL.COM", "AQAAAAIAAYagAAAAENLHgRqrtTjfuK+8WmygVjKZm6BxmqmMVWJvRWGObQ3HpLibvqVQum1JEKBQIKxnEg==", null, false, "625d125f-8ca7-4245-bfc8-70319603ceeb", false, "admin@mail.com" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Brand", "Category", "ProductName", "Quantity" },
                values: new object[,]
                {
                    { 1, "Родопи", 3, "Ябълки Златна превъзходна", 1000 },
                    { 2, "Лудогорско", 3, "Пилешко филе", 500 },
                    { 3, "Градус", 3, "Пуешко филе", 500 },
                    { 4, "Еко Меат", 3, "Телешко бон филе", 500 },
                    { 5, "БМК", 3, "Натурален йогурт 3.6%", 400 },
                    { 6, "Верея", 3, "Прясно мляко 3.6%", 1000 },
                    { 7, "Маджаров", 3, "Кашкавал от краве мляко", 400 },
                    { 8, "Лакрима", 3, "Сирене от краве мляко", 500 },
                    { 9, "Верея", 3, "Масло краве 82%", 200 },
                    { 10, "Нашата ферма", 3, "Яйца размер L", 10 },
                    { 11, "Добруджа", 3, "Хляб пълнозърнест", 500 },
                    { 12, "Булгарплод", 3, "Картофи", 2000 },
                    { 13, "Марица Земеделие", 3, "Домати", 1000 },
                    { 14, "Марица Земеделие", 3, "Краставици", 500 },
                    { 15, "Булгарплод", 3, "Лук", 1000 },
                    { 16, "Булгарплод", 3, "Моркови", 1000 },
                    { 17, "Арпа", 3, "Ориз", 1000 },
                    { 18, "Арпа", 3, "Макарони", 500 },
                    { 19, "Добруджа", 3, "Брашно тип 500", 1000 },
                    { 20, "Арпа", 3, "Захар бяла кристална", 1000 },
                    { 21, "Златна Добруджа", 3, "Олио", 1000 },
                    { 22, "Солница", 3, "Сол", 500 },
                    { 23, "Орехите", 3, "Колбас Елена", 100 },
                    { 24, "Карлово Мес", 3, "Луканка", 200 },
                    { 25, "Дерони", 3, "Зеле", 500 },
                    { 26, "БМК", 3, "Айран", 500 },
                    { 27, "Български мед", 3, "Мед полифлорен", 500 },
                    { 28, "Арпа", 3, "Фасул бял", 500 },
                    { 29, "Арпа", 3, "Леща червена", 500 },
                    { 30, "Арпа", 3, "Нахут", 500 },
                    { 31, "Дерони", 3, "Консерва риба тон", 160 },
                    { 32, "Дерони", 3, "Консерва пъстърва", 200 },
                    { 33, "Рила", 3, "Сок портокал 100%", 1000 },
                    { 34, "Михалково", 3, "Минерална вода", 500 },
                    { 35, "Верея", 3, "Чай черен пакетчета", 25 },
                    { 36, "OMV", 6, "Дизелово гориво", 1 },
                    { 37, "Лукойл", 6, "Бензин А95", 1 },
                    { 38, "Пропанола", 6, "Пропан-бутан", 1 },
                    { 39, "Auto Gas", 6, "Газ пропан за кола", 1 },
                    { 40, "Shell", 6, "Бензин А98", 1 },
                    { 41, "ЧЕЗ", 9, "Ток домакински", 1 },
                    { 42, "Софийска вода", 9, "Вода канализация", 1 },
                    { 43, "Топлофикация София", 9, "Отопление топлофикация", 1 },
                    { 44, "Спортмаркет", 5, "Футболна топка", 1 },
                    { 45, "Пулс Фитнес", 5, "Абонамент фитнес", 1 },
                    { 46, "Софарма", 4, "Лекарства витамини", 30 },
                    { 47, "Лавена", 8, "Почистващ препарат", 500 },
                    { 48, "Комикс БГ", 10, "Детски играчки", 1 },
                    { 49, "Анубис", 7, "Книга учебна", 1 },
                    { 50, "А1", 9, "Мобилен интернет", 1 }
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
                    { 5, "Fantastico" },
                    { 6, "Piccadilly" },
                    { 7, "T-Market" },
                    { 8, "CBA" },
                    { 9, "Penny Market" },
                    { 10, "Lukoil" },
                    { 11, "Shell" },
                    { 12, "OMV" },
                    { 13, "Petrol" },
                    { 14, "EKO" },
                    { 15, "Т-маркет" },
                    { 16, "EVN" },
                    { 17, "ВИК Пловдив" },
                    { 18, "BBB" },
                    { 19, "Аптеки 366" },
                    { 20, "X-fit" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c3a2cde1-caac-4a6c-a24f-1b5d35b47f59", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" });

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
                    { 6, 12.50m, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Месо за мезе", 2, 6, 2, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 7, 3.20m, new DateTime(2025, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Кисело мляко за закуска", 2, 5, 3, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 8, 2.50m, new DateTime(2025, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Хляб за домакински нужди", 6, 8, 5, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 9, 5.50m, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Прясно мляко с лешник", 2, 9, 3, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 10, 125.80m, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Месечна сметка за ток", 3, 41, 16, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.InsertData(
                table: "MonthlyBudgets",
                columns: new[] { "BudgedId", "BudgetAmount", "Month", "UserId" },
                values: new object[,]
                {
                    { 1, 1200.00m, 4, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, 1300.00m, 5, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, 1300.00m, 6, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 4, 1400.00m, 7, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 5, 1350.00m, 8, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 6, 1450.00m, 9, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 7, 1500.00m, 10, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 8, 1250.00m, 11, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 9, 1600.00m, 12, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 10, 1550.00m, 1, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "PriceId", "DateChecked", "ProductId", "SellingPrice", "StoreId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4.99m, 1 },
                    { 2, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4.49m, 2 },
                    { 3, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5.29m, 3 },
                    { 4, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 18.90m, 1 },
                    { 5, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 17.50m, 4 },
                    { 6, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 19.99m, 18 },
                    { 7, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 21.50m, 1 },
                    { 8, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 20.99m, 5 },
                    { 9, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 35.90m, 4 },
                    { 10, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 32.99m, 1 },
                    { 11, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3.45m, 1 },
                    { 12, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3.29m, 2 },
                    { 13, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3.59m, 18 },
                    { 14, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2.89m, 1 },
                    { 15, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2.95m, 3 },
                    { 16, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2.79m, 9 },
                    { 17, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 24.99m, 1 },
                    { 18, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 23.50m, 18 },
                    { 19, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 18.99m, 2 },
                    { 20, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 19.49m, 5 },
                    { 21, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 7.89m, 1 },
                    { 22, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 7.99m, 3 },
                    { 23, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 4.50m, 1 },
                    { 24, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 4.39m, 5 },
                    { 25, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 4.69m, 8 },
                    { 26, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 2.45m, 1 },
                    { 27, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 2.29m, 2 },
                    { 28, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 1.89m, 1 },
                    { 29, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 1.79m, 2 },
                    { 30, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 1.99m, 9 },
                    { 31, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 4.99m, 1 },
                    { 32, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 5.29m, 3 },
                    { 33, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 4.79m, 7 },
                    { 34, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, 3.99m, 1 },
                    { 35, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, 3.79m, 5 },
                    { 36, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 2.29m, 2 },
                    { 37, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 2.49m, 8 },
                    { 38, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, 2.99m, 1 },
                    { 39, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, 3.19m, 3 },
                    { 40, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, 3.45m, 1 },
                    { 41, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, 3.29m, 4 },
                    { 42, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 18, 2.89m, 1 },
                    { 43, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 18, 2.69m, 2 },
                    { 44, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 19, 2.99m, 1 },
                    { 45, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 19, 2.79m, 4 },
                    { 46, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, 2.79m, 1 },
                    { 47, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, 2.69m, 2 },
                    { 48, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 21, 4.99m, 1 },
                    { 49, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 21, 5.19m, 5 },
                    { 50, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 22, 1.29m, 1 },
                    { 51, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 22, 1.19m, 9 },
                    { 52, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 23, 18.90m, 1 },
                    { 53, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 24, 22.50m, 4 },
                    { 54, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, 1.99m, 1 },
                    { 55, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, 2.19m, 3 },
                    { 56, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 26, 2.89m, 1 },
                    { 57, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 26, 2.99m, 18 },
                    { 58, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 27, 12.90m, 1 },
                    { 59, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 28, 4.59m, 1 },
                    { 60, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 28, 4.29m, 4 },
                    { 61, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 29, 5.99m, 1 },
                    { 62, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 6.49m, 4 },
                    { 63, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 31, 4.89m, 1 },
                    { 64, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 31, 5.19m, 3 },
                    { 65, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 32, 5.99m, 1 },
                    { 66, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 33, 3.99m, 1 },
                    { 67, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 33, 3.79m, 2 },
                    { 68, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 34, 1.39m, 1 },
                    { 69, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 34, 1.29m, 5 },
                    { 70, new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 35, 4.79m, 1 }
                });

            migrationBuilder.InsertData(
                table: "ToDoItems",
                columns: new[] { "TaskId", "CreatedAt", "Description", "DueDate", "Priority", "TaskStatus", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Buy food for the next two weeks", new DateTime(2025, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Shopping", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Get the car ready for the trip", new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Fill with fuel", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pay the bills for this month.", new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, "House bills", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 4, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Buy groceries", new DateTime(2025, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Weekly grocery shopping", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 5, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Book routine dental checkup for next month", new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Schedule dentist appointment", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 6, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Finish reading 'The Clean Coder' by Robert Martin", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Read book", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 7, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Complete 30-minute workout session", new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Exercise routine", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 8, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Research and book summer vacation destination and accommodation", new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Plan vacation", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 9, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Add recent projects and skills to professional resume", new DateTime(2025, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "The resume must be updated", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 10, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Complete online course on React.js fundamentals", new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Learn new technology", "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationId", "CreatedAt", "IsRead", "Message", "NotificationTime", "TaskId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Reminder to buy a food for next two week", new DateTime(2025, 6, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 2, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Reminder to fill up the car with gas and check it, so it is ready for the trip next week.", new DateTime(2025, 6, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), 2, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Check and pay the bills", new DateTime(2025, 6, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 4, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Don't forget your weekly grocery shopping is due tomorrow!", new DateTime(2025, 6, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), 4, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 5, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Time to schedule your dentist appointment", new DateTime(2025, 6, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 6, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Reading time! Your book deadline is approaching", new DateTime(2025, 6, 25, 19, 0, 0, 0, DateTimeKind.Unspecified), 6, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 7, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Daily workout reminder - let's get moving!", new DateTime(2025, 6, 2, 7, 0, 0, 0, DateTimeKind.Unspecified), 7, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 8, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Start planning your vacation - deadline coming up!", new DateTime(2025, 6, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 8, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 9, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Time to change your resume with recent achievements", new DateTime(2025, 6, 11, 14, 0, 0, 0, DateTimeKind.Unspecified), 9, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" },
                    { 10, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Learning reminder: Continue with your React.js course", new DateTime(2025, 7, 1, 18, 0, 0, 0, DateTimeKind.Unspecified), 10, "56f4b198-3f3e-4f72-9a80-7d903bf24a1e" }
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
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

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
