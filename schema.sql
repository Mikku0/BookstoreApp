CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Discounts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Discounts" PRIMARY KEY AUTOINCREMENT,
    "Percentage" TEXT NOT NULL,
    "Description" TEXT NOT NULL
);

CREATE TABLE "Reports" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reports" PRIMARY KEY AUTOINCREMENT,
    "Data" TEXT NOT NULL,
    "CreationDate" TEXT NOT NULL,
    "Notes" TEXT NOT NULL
);

CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "UserLogin" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "UserType" TEXT NOT NULL,
    "Address" TEXT NULL,
    "Employee_Salary" TEXT NULL,
    "Employee_DateOfEmployment" TEXT NULL,
    "Salary" TEXT NULL,
    "DateOfEmployment" TEXT NULL
);

CREATE TABLE "Warehouses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Warehouses" PRIMARY KEY AUTOINCREMENT
);

CREATE TABLE "Carts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Carts" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "Value" TEXT NOT NULL,
    CONSTRAINT "FK_Carts_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Orders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
    "ClientId" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "TotalPrice" TEXT NOT NULL,
    "EmployeeId" INTEGER NULL,
    "ManagerId" INTEGER NULL,
    CONSTRAINT "FK_Orders_Users_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Orders_Users_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Users" ("Id"),
    CONSTRAINT "FK_Orders_Users_ManagerId" FOREIGN KEY ("ManagerId") REFERENCES "Users" ("Id")
);

CREATE TABLE "Books" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Books" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Author" TEXT NOT NULL,
    "Price" TEXT NOT NULL,
    "Genre" TEXT NOT NULL,
    "WarehouseId" INTEGER NULL,
    CONSTRAINT "FK_Books_Warehouses_WarehouseId" FOREIGN KEY ("WarehouseId") REFERENCES "Warehouses" ("Id")
);

CREATE TABLE "Deliveries" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Deliveries" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    CONSTRAINT "FK_Deliveries_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "BookDiscount" (
    "BooksId" INTEGER NOT NULL,
    "DiscountsId" INTEGER NOT NULL,
    CONSTRAINT "PK_BookDiscount" PRIMARY KEY ("BooksId", "DiscountsId"),
    CONSTRAINT "FK_BookDiscount_Books_BooksId" FOREIGN KEY ("BooksId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BookDiscount_Discounts_DiscountsId" FOREIGN KEY ("DiscountsId") REFERENCES "Discounts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CartItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CartItems" PRIMARY KEY AUTOINCREMENT,
    "CartId" INTEGER NOT NULL,
    "BookId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    CONSTRAINT "FK_CartItems_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CartItems_Carts_CartId" FOREIGN KEY ("CartId") REFERENCES "Carts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "OrderDetails" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderDetails" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    "BookId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "Date" TEXT NOT NULL,
    CONSTRAINT "FK_OrderDetails_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderDetails_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

INSERT INTO "Books" ("Id", "Author", "Genre", "Name", "Price", "WarehouseId")
VALUES (1, 'Adam Mickiewicz', 'Klasyka', 'Pan Tadeusz', '29.99', NULL);
SELECT changes();

INSERT INTO "Books" ("Id", "Author", "Genre", "Name", "Price", "WarehouseId")
VALUES (2, 'Andrzej Sapkowski', 'Fantasy', 'Wiedźmin', '34.99', NULL);
SELECT changes();

INSERT INTO "Books" ("Id", "Author", "Genre", "Name", "Price", "WarehouseId")
VALUES (3, 'Stanisław Lem', 'Science Fiction', 'Solaris', '27.5', NULL);
SELECT changes();


CREATE INDEX "IX_BookDiscount_DiscountsId" ON "BookDiscount" ("DiscountsId");

CREATE INDEX "IX_Books_WarehouseId" ON "Books" ("WarehouseId");

CREATE INDEX "IX_CartItems_BookId" ON "CartItems" ("BookId");

CREATE INDEX "IX_CartItems_CartId" ON "CartItems" ("CartId");

CREATE UNIQUE INDEX "IX_Carts_UserId" ON "Carts" ("UserId");

CREATE INDEX "IX_Deliveries_OrderId" ON "Deliveries" ("OrderId");

CREATE INDEX "IX_OrderDetails_BookId" ON "OrderDetails" ("BookId");

CREATE INDEX "IX_OrderDetails_OrderId" ON "OrderDetails" ("OrderId");

CREATE INDEX "IX_Orders_ClientId" ON "Orders" ("ClientId");

CREATE INDEX "IX_Orders_EmployeeId" ON "Orders" ("EmployeeId");

CREATE INDEX "IX_Orders_ManagerId" ON "Orders" ("ManagerId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250608121852_InitialCreate', '9.0.5');

COMMIT;

