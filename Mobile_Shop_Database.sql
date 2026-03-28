-- ============================================================
-- Mobile Shop Management System
-- Database Creation Script
-- Verified against: mobile_shop_web_api + mobile_shop_web_app
-- Roles: Admin, Sales  (verified from AddUser.cshtml radio buttons)
-- Procedure names: verified from Data repository SqlCommand calls
-- ============================================================
-- drop database MobileShopDB;
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MobileShopDB')
    BEGIN
        CREATE DATABASE MobileShopDB;
    END
GO

USE MobileShopDB;
GO

-- ============================================================
-- TABLE: Categories
-- Referenced by: Products.category_id
-- Used in: Product Add/Edit form (dropdown via PR_Categories_Dropdown)
-- ============================================================

IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
CREATE TABLE Categories (
                            category_id    INT            IDENTITY(1,1) PRIMARY KEY,
                            category_name  NVARCHAR(100)  NOT NULL,
                            description    NVARCHAR(500)  NULL,
                            created_date   DATETIME       NOT NULL DEFAULT GETDATE(),
                            modified_date  DATETIME       NULL
);
GO

-- ============================================================
-- TABLE: Brands
-- Referenced by: Products.product_brand_id
-- Used in: Product Add/Edit form (dropdown via PR_Brands_Dropdown)
-- ============================================================

IF OBJECT_ID('dbo.Brands', 'U') IS NOT NULL DROP TABLE dbo.Brands;
CREATE TABLE Brands (
                        brand_id      INT            IDENTITY(1,1) PRIMARY KEY,
                        brand_name    NVARCHAR(100)  NOT NULL,
                        created_date  DATETIME       NOT NULL DEFAULT GETDATE(),
                        modified_date DATETIME       NULL
);
GO

-- ============================================================
-- TABLE: Users
-- role values:   'Admin' or 'Sales'   (verified from AddUser.cshtml)
-- status values: 'Active' or 'Inactive'
-- Referenced by: Invoices.user_id, StockTransactions.user_id
-- Used in: Login form, AddUser form (role radio buttons)
-- Dropdown: PR_Users_Dropdown
-- ============================================================

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
CREATE TABLE Users (
                       user_id       INT            IDENTITY(1,1) PRIMARY KEY,
                       user_name     NVARCHAR(50)   NOT NULL,
                       user_email    NVARCHAR(100)  NOT NULL UNIQUE,
                       password      NVARCHAR(256)  NOT NULL,
                       role          NVARCHAR(20)   NOT NULL CHECK (role IN ('Admin', 'Sales')),
                       status        NVARCHAR(20)   NOT NULL CHECK (status IN ('Active', 'Inactive')),
                       created_date  DATETIME       NOT NULL DEFAULT GETDATE(),
                       modified_date DATETIME       NULL
);
GO

-- ============================================================
-- TABLE: Customers
-- Referenced by: Invoices.customer_id
-- Used in: Invoice form (dropdown via PR_Customers_Dropdown)
-- ============================================================

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
CREATE TABLE Customers (
                           customer_id      INT            IDENTITY(1,1) PRIMARY KEY,
                           customer_name    NVARCHAR(100)  NOT NULL,
                           customer_email   NVARCHAR(100)  NOT NULL UNIQUE,
                           customer_phone   NVARCHAR(20)   NOT NULL,
                           customer_address NVARCHAR(300)  NOT NULL,
                           created_date     DATETIME       NOT NULL DEFAULT GETDATE(),
                           modified_date    DATETIME       NULL
);
GO

-- ============================================================
-- TABLE: Products
-- FK: category_id      -> Categories(category_id)
-- FK: product_brand_id -> Brands(brand_id)
-- Used in: Product Add/Edit form, Invoice line items, Stock Transaction form
-- Dropdown: PR_Products_Dropdown
-- ============================================================

IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
CREATE TABLE Products (
                          product_id          INT             IDENTITY(1,1) PRIMARY KEY,
                          category_id         INT             NOT NULL,
                          product_brand_id    INT             NOT NULL,
                          product_name        NVARCHAR(100)   NOT NULL,
                          product_image       NVARCHAR(500)   NULL,
                          product_price       DECIMAL(18,2)   NOT NULL CHECK (product_price > 0),
                          stock_quantity      INT             NOT NULL CHECK (stock_quantity >= 0),
                          product_description NVARCHAR(1000)  NULL,
                          status              NVARCHAR(20)    NOT NULL CHECK (status IN ('Available', 'Not Available')),
                          created_date        DATETIME        NOT NULL DEFAULT GETDATE(),
                          modified_date       DATETIME        NULL,

                          CONSTRAINT FK_Products_Category FOREIGN KEY (category_id)
                              REFERENCES Categories(category_id),

                          CONSTRAINT FK_Products_Brand FOREIGN KEY (product_brand_id)
                              REFERENCES Brands(brand_id)
);
GO

-- ============================================================
-- TABLE: Invoices
-- FK: customer_id -> Customers(customer_id)
-- FK: user_id     -> Users(user_id)
-- user_id is auto-filled from session (logged-in user)
-- customer_id selected from customer dropdown in Invoice form
-- payment_method: 'Cash', 'Card', 'Online'
-- payment_status: 'Paid', 'Pending', 'Cancelled'
-- ============================================================

IF OBJECT_ID('dbo.Invoices', 'U') IS NOT NULL DROP TABLE dbo.Invoices;
CREATE TABLE Invoices (
                          invoice_id     INT            IDENTITY(1,1) PRIMARY KEY,
                          customer_id    INT            NOT NULL,
                          user_id        INT            NOT NULL,
                          date           DATETIME       NOT NULL DEFAULT GETDATE(),
                          total_amount   DECIMAL(18,2)  NOT NULL DEFAULT 0,
                          payment_method NVARCHAR(50)   NOT NULL,
                          payment_status NVARCHAR(50)   NOT NULL,

                          CONSTRAINT FK_Invoices_Customer FOREIGN KEY (customer_id)
                              REFERENCES Customers(customer_id),

                          CONSTRAINT FK_Invoices_User FOREIGN KEY (user_id)
                              REFERENCES Users(user_id)
);
GO

-- ============================================================
-- TABLE: InvoiceItems
-- FK: invoice_id -> Invoices(invoice_id)  [CASCADE DELETE]
-- FK: product_id -> Products(product_id)
-- product_id selected from product dropdown in Invoice line-item form
-- ============================================================

IF OBJECT_ID('dbo.InvoiceItems', 'U') IS NOT NULL DROP TABLE dbo.InvoiceItems;
CREATE TABLE InvoiceItems (
                              invoice_item_id INT            IDENTITY(1,1) PRIMARY KEY,
                              invoice_id      INT            NOT NULL,
                              product_id      INT            NOT NULL,
                              quantity        INT            NOT NULL CHECK (quantity > 0),
                              unit_price      DECIMAL(18,2)  NOT NULL,
                              total_price     DECIMAL(18,2)  NOT NULL,

                              CONSTRAINT FK_InvoiceItems_Invoice FOREIGN KEY (invoice_id)
                                  REFERENCES Invoices(invoice_id) ON DELETE CASCADE,

                              CONSTRAINT FK_InvoiceItems_Product FOREIGN KEY (product_id)
                                  REFERENCES Products(product_id)
);
GO

-- ============================================================
-- TABLE: StockTransactions
-- FK: product_id -> Products(product_id)
-- FK: user_id    -> Users(user_id)
-- user_id is auto-filled from session (logged-in user)
-- product_id selected from product dropdown in Stock Transaction form
-- transaction_state: 'Purchase' (stock added) or 'Sale' (stock removed)
-- ============================================================

IF OBJECT_ID('dbo.StockTransactions', 'U') IS NOT NULL DROP TABLE dbo.StockTransactions;
CREATE TABLE StockTransactions (
                                   transaction_id          INT            IDENTITY(1,1) PRIMARY KEY,
                                   product_id              INT            NOT NULL,
                                   user_id                 INT            NOT NULL,
                                   stock_quantity          INT            NOT NULL,
                                   date                    DATETIME       NOT NULL DEFAULT GETDATE(),
                                   transaction_state       NVARCHAR(20)   NOT NULL CHECK (transaction_state IN ('Purchase', 'Sale')),
                                   transaction_description NVARCHAR(500)  NULL,

                                   CONSTRAINT FK_StockTrans_Product FOREIGN KEY (product_id)
                                       REFERENCES Products(product_id),

                                   CONSTRAINT FK_StockTrans_User FOREIGN KEY (user_id)
                                       REFERENCES Users(user_id)
);
GO

-- ============================================================
-- INDEXES
-- ============================================================

CREATE INDEX IX_Products_CategoryId    ON Products(category_id);
CREATE INDEX IX_Products_BrandId       ON Products(product_brand_id);
CREATE INDEX IX_Invoices_CustomerId    ON Invoices(customer_id);
CREATE INDEX IX_Invoices_UserId        ON Invoices(user_id);
CREATE INDEX IX_Invoices_Date          ON Invoices(date);
CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(invoice_id);
CREATE INDEX IX_InvoiceItems_ProductId ON InvoiceItems(product_id);
CREATE INDEX IX_StockTrans_ProductId   ON StockTransactions(product_id);
CREATE INDEX IX_StockTrans_UserId      ON StockTransactions(user_id);
GO

-- ============================================================
-- STORED PROCEDURES: Categories
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Categories_Select_All
AS
BEGIN
    SELECT category_id, category_name, description, created_date, modified_date
    FROM Categories
    ORDER BY category_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Categories_Select_By_Id
@CategoryId INT
AS
BEGIN
    SELECT category_id, category_name, description, created_date, modified_date
    FROM Categories
    WHERE category_id = @CategoryId;
END
GO

CREATE OR ALTER PROCEDURE PR_Categories_Insert
    @CategoryName NVARCHAR(100),
    @Description  NVARCHAR(500) = NULL
AS
BEGIN
    INSERT INTO Categories (category_name, description, created_date)
    VALUES (@CategoryName, @Description, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Categories_Update
    @CategoryId   INT,
    @CategoryName NVARCHAR(100),
    @Description  NVARCHAR(500) = NULL
AS
BEGIN
    UPDATE Categories
    SET category_name = @CategoryName,
        description   = @Description,
        modified_date = GETDATE()
    WHERE category_id = @CategoryId;
END
GO

CREATE OR ALTER PROCEDURE PR_Categories_Delete
@CategoryId INT
AS
BEGIN
    DELETE FROM Categories WHERE category_id = @CategoryId;
END
GO

-- Dropdown for Category select lists in forms
CREATE OR ALTER PROCEDURE PR_Categories_Dropdown
AS
BEGIN
    SELECT category_id, category_name
    FROM Categories
    ORDER BY category_name;
END
GO

-- ============================================================
-- STORED PROCEDURES: Brands
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Brands_Select_All
AS
BEGIN
    SELECT brand_id, brand_name, created_date, modified_date
    FROM Brands
    ORDER BY brand_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Brands_Select_By_Id
@BrandId INT
AS
BEGIN
    SELECT brand_id, brand_name, created_date, modified_date
    FROM Brands
    WHERE brand_id = @BrandId;
END
GO

CREATE OR ALTER PROCEDURE PR_Brands_Insert
@BrandName NVARCHAR(100)
AS
BEGIN
    INSERT INTO Brands (brand_name, created_date)
    VALUES (@BrandName, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Brands_Update
    @BrandId   INT,
    @BrandName NVARCHAR(100)
AS
BEGIN
    UPDATE Brands
    SET brand_name    = @BrandName,
        modified_date = GETDATE()
    WHERE brand_id = @BrandId;
END
GO

CREATE OR ALTER PROCEDURE PR_Brands_Delete
@BrandId INT
AS
BEGIN
    DELETE FROM Brands WHERE brand_id = @BrandId;
END
GO

-- Dropdown for Brand select lists in forms
CREATE OR ALTER PROCEDURE PR_Brands_Dropdown
AS
BEGIN
    SELECT brand_id, brand_name
    FROM Brands
    ORDER BY brand_name;
END
GO

-- ============================================================
-- STORED PROCEDURES: Users
-- role: 'Admin' or 'Sales' only
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Users_Select_All
AS
BEGIN
    SELECT user_id, user_name, user_email, password, role, status, created_date, modified_date
    FROM Users
    ORDER BY user_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Users_Select_By_Id
@UserId INT
AS
BEGIN
    SELECT user_id, user_name, user_email, password, role, status, created_date, modified_date
    FROM Users
    WHERE user_id = @UserId;
END
GO

CREATE OR ALTER PROCEDURE PR_Users_Insert
    @UserName  NVARCHAR(50),
    @UserEmail NVARCHAR(100),
    @Password  NVARCHAR(256),
    @Role      NVARCHAR(20),   -- 'Admin' or 'Sales'
    @Status    NVARCHAR(20)    -- 'Active' or 'Inactive'
AS
BEGIN
    INSERT INTO Users (user_name, user_email, password, role, status, created_date)
    VALUES (@UserName, @UserEmail, @Password, @Role, @Status, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Users_Update
    @UserId    INT,
    @UserName  NVARCHAR(50),
    @UserEmail NVARCHAR(100),
    @Password  NVARCHAR(256),
    @Role      NVARCHAR(20),   -- 'Admin' or 'Sales'
    @Status    NVARCHAR(20)    -- 'Active' or 'Inactive'
AS
BEGIN
    UPDATE Users
    SET user_name     = @UserName,
        user_email    = @UserEmail,
        password      = @Password,
        role          = @Role,
        status        = @Status,
        modified_date = GETDATE()
    WHERE user_id = @UserId;
END
GO

CREATE OR ALTER PROCEDURE PR_Users_Delete
@UserId INT
AS
BEGIN
    DELETE FROM Users WHERE user_id = @UserId;
END
GO

-- Sign In: checks email + password + Active status, returns user row + message
CREATE OR ALTER PROCEDURE PR_Users_Sign_In
    @UserEmail NVARCHAR(100),
    @Password  NVARCHAR(256)
AS
BEGIN
    DECLARE @UserId INT;

    SELECT @UserId = user_id
    FROM Users
    WHERE user_email = @UserEmail
      AND password   = @Password
      AND status     = 'Active';

    IF @UserId IS NOT NULL
        SELECT user_id, user_name, user_email, password, role, status,
               created_date, modified_date,
               'Login successful' AS Message
        FROM Users
        WHERE user_id = @UserId;
    ELSE
        SELECT NULL AS user_id,    NULL AS user_name,  NULL AS user_email,
               NULL AS password,   NULL AS role,       NULL AS status,
               NULL AS created_date, NULL AS modified_date,
               'Invalid Email or Password' AS Message;
END
GO

-- Sign Out: verifies credentials and confirms logout
CREATE OR ALTER PROCEDURE PR_Users_Sign_Out
    @UserEmail NVARCHAR(100),
    @Password  NVARCHAR(256)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE user_email = @UserEmail AND password = @Password)
        SELECT 'Logout successful' AS Message;
    ELSE
        SELECT 'Invalid Email or Password' AS Message;
END
GO

-- Dropdown for User select lists in forms (e.g. invoice user assignment)
CREATE OR ALTER PROCEDURE PR_Users_Dropdown
AS
BEGIN
    SELECT user_id, user_name
    FROM Users
    WHERE status = 'Active'
    ORDER BY user_name;
END
GO

-- ============================================================
-- STORED PROCEDURES: Customers
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Customers_Select_All
AS
BEGIN
    SELECT customer_id, customer_name, customer_email, customer_phone,
           customer_address, created_date, modified_date
    FROM Customers
    ORDER BY customer_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Customers_Select_By_Id
@CustomerId INT
AS
BEGIN
    SELECT customer_id, customer_name, customer_email, customer_phone,
           customer_address, created_date, modified_date
    FROM Customers
    WHERE customer_id = @CustomerId;
END
GO

CREATE OR ALTER PROCEDURE PR_Customers_Insert
    @CustomerName    NVARCHAR(100),
    @CustomerEmail   NVARCHAR(100),
    @CustomerPhone   NVARCHAR(20),
    @CustomerAddress NVARCHAR(300)
AS
BEGIN
    INSERT INTO Customers (customer_name, customer_email, customer_phone, customer_address, created_date)
    VALUES (@CustomerName, @CustomerEmail, @CustomerPhone, @CustomerAddress, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Customers_Update
    @CustomerId      INT,
    @CustomerName    NVARCHAR(100),
    @CustomerEmail   NVARCHAR(100),
    @CustomerPhone   NVARCHAR(20),
    @CustomerAddress NVARCHAR(300)
AS
BEGIN
    UPDATE Customers
    SET customer_name    = @CustomerName,
        customer_email   = @CustomerEmail,
        customer_phone   = @CustomerPhone,
        customer_address = @CustomerAddress,
        modified_date    = GETDATE()
    WHERE customer_id = @CustomerId;
END
GO

CREATE OR ALTER PROCEDURE PR_Customers_Delete
@CustomerId INT
AS
BEGIN
    DELETE FROM Customers WHERE customer_id = @CustomerId;
END
GO

-- Dropdown for Customer select lists in forms (e.g. invoice customer selection)
CREATE OR ALTER PROCEDURE PR_Customers_Dropdown
AS
BEGIN
    SELECT customer_id, customer_name
    FROM Customers
    ORDER BY customer_name;
END
GO

-- ============================================================
-- STORED PROCEDURES: Products
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Products_Select_All
    @CategoryId INT = NULL,
    @BrandId    INT = NULL
AS
BEGIN
    SELECT p.product_id,
           p.category_id,
           c.category_name,
           p.product_brand_id,
           b.brand_name,
           p.product_name,
           p.product_image,
           p.product_price,
           p.stock_quantity,
           p.product_description,
           p.status,
           p.created_date,
           p.modified_date
    FROM Products p
             INNER JOIN Categories c ON p.category_id      = c.category_id
             INNER JOIN Brands     b ON p.product_brand_id = b.brand_id
    WHERE (@CategoryId IS NULL OR p.category_id      = @CategoryId)
      AND (@BrandId    IS NULL OR p.product_brand_id = @BrandId)
    ORDER BY p.product_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Select_By_Id
@ProductId INT
AS
BEGIN
    SELECT p.product_id,
           p.category_id,
           c.category_name,
           p.product_brand_id,
           b.brand_name,
           p.product_name,
           p.product_image,
           p.product_price,
           p.stock_quantity,
           p.product_description,
           p.status,
           p.created_date,
           p.modified_date
    FROM Products p
             INNER JOIN Categories c ON p.category_id      = c.category_id
             INNER JOIN Brands     b ON p.product_brand_id = b.brand_id
    WHERE p.product_id = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Insert
    @CategoryId         INT,
    @ProductBrandId     INT,
    @ProductName        NVARCHAR(100),
    @ProductImage       NVARCHAR(500)  = NULL,
    @ProductPrice       DECIMAL(18,2),
    @StockQuantity      INT,
    @ProductDescription NVARCHAR(1000) = NULL,
    @Status             NVARCHAR(20)
AS
BEGIN
    INSERT INTO Products
    (category_id, product_brand_id, product_name, product_image,
     product_price, stock_quantity, product_description, status, created_date)
    VALUES
        (@CategoryId, @ProductBrandId, @ProductName, @ProductImage,
         @ProductPrice, @StockQuantity, @ProductDescription, @Status, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Update
    @ProductId          INT,
    @CategoryId         INT,
    @ProductBrandId     INT,
    @ProductName        NVARCHAR(100),
    @ProductImage       NVARCHAR(500)  = NULL,
    @ProductPrice       DECIMAL(18,2),
    @StockQuantity      INT,
    @ProductDescription NVARCHAR(1000) = NULL,
    @Status             NVARCHAR(20)
AS
BEGIN
    UPDATE Products
    SET category_id         = @CategoryId,
        product_brand_id    = @ProductBrandId,
        product_name        = @ProductName,
        product_image       = COALESCE(@ProductImage, product_image),
        product_price       = @ProductPrice,
        stock_quantity      = @StockQuantity,
        product_description = @ProductDescription,
        status              = @Status,
        modified_date       = GETDATE()
    WHERE product_id = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Delete
@ProductId INT
AS
BEGIN
    DELETE FROM Products WHERE product_id = @ProductId;
END
GO

-- Dropdown for Product select lists in forms (e.g. invoice line items, stock transactions)
CREATE OR ALTER PROCEDURE PR_Products_Dropdown
AS
BEGIN
    SELECT product_id, product_name
    FROM Products
    WHERE status = 'Available'
    ORDER BY product_name;
END
GO

-- ============================================================
-- STORED PROCEDURES: Invoices
-- PR_Invoices_Select_All filters by user_id so each user
-- only sees their own invoices
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Invoices_Select_All
    @UserId     INT,
    @CustomerId INT          = NULL,
    @Status     NVARCHAR(50) = NULL
AS
BEGIN
    SELECT i.invoice_id,
           i.customer_id,
           i.user_id,
           c.customer_name,
           u.user_name,
           i.date,
           i.total_amount,
           i.payment_method,
           i.payment_status
    FROM Invoices i
             INNER JOIN Customers c ON i.customer_id = c.customer_id
             INNER JOIN Users     u ON i.user_id     = u.user_id
    WHERE i.user_id = @UserId
      AND (@CustomerId IS NULL OR i.customer_id    = @CustomerId)
      AND (@Status     IS NULL OR i.payment_status = @Status)
    ORDER BY i.date DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Select_By_Id
@InvoiceId INT
AS
BEGIN
    SELECT i.invoice_id,
           i.customer_id,
           i.user_id,
           c.customer_name,
           u.user_name,
           i.date,
           i.total_amount,
           i.payment_method,
           i.payment_status
    FROM Invoices i
             INNER JOIN Customers c ON i.customer_id = c.customer_id
             INNER JOIN Users     u ON i.user_id     = u.user_id
    WHERE i.invoice_id = @InvoiceId;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Insert
    @CustomerId    INT,
    @UserId        INT,
    @TotalAmount   DECIMAL(18,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentStatus NVARCHAR(50),
    @InvoiceId     INT OUTPUT
AS
BEGIN
    INSERT INTO Invoices (customer_id, user_id, date, total_amount, payment_method, payment_status)
    VALUES (@CustomerId, @UserId, GETDATE(), @TotalAmount, @PaymentMethod, @PaymentStatus);

    SET @InvoiceId = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Update
    @InvoiceId     INT,
    @CustomerId    INT,
    @UserId        INT,
    @TotalAmount   DECIMAL(18,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentStatus NVARCHAR(50)
AS
BEGIN
    UPDATE Invoices
    SET customer_id    = @CustomerId,
        user_id        = @UserId,
        total_amount   = @TotalAmount,
        payment_method = @PaymentMethod,
        payment_status = @PaymentStatus
    WHERE invoice_id = @InvoiceId;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Delete
@InvoiceId INT
AS
BEGIN
    DELETE FROM Invoices WHERE invoice_id = @InvoiceId;
END
GO

-- ============================================================
-- STORED PROCEDURES: Invoice Items
-- Note: procedure prefix is PR_Invoice_Items_ (with underscore)
--       NOT PR_InvoiceItems_ — verified from code screenshots
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Invoice_Items_Select_All
AS
BEGIN
    SELECT ii.invoice_item_id,
           ii.invoice_id,
           ii.product_id,
           p.product_name,
           ii.quantity,
           ii.unit_price,
           ii.total_price
    FROM InvoiceItems ii
             INNER JOIN Products p ON ii.product_id = p.product_id
    ORDER BY ii.invoice_id;
END
GO

-- Select single invoice item by its own ID
CREATE OR ALTER PROCEDURE PR_Invoice_Items_Select_By_Id
@InvoiceItemId INT
AS
BEGIN
    SELECT ii.invoice_item_id,
           ii.invoice_id,
           ii.product_id,
           p.product_name,
           ii.quantity,
           ii.unit_price,
           ii.total_price
    FROM InvoiceItems ii
             INNER JOIN Products p ON ii.product_id = p.product_id
    WHERE ii.invoice_item_id = @InvoiceItemId;
END
GO

-- Select all line items belonging to a specific invoice
CREATE OR ALTER PROCEDURE PR_Invoice_Items_Select_By_InvoiceId
@InvoiceId INT
AS
BEGIN
    SELECT ii.invoice_item_id,
           ii.invoice_id,
           ii.product_id,
           p.product_name,
           ii.quantity,
           ii.unit_price,
           ii.total_price
    FROM InvoiceItems ii
             INNER JOIN Products p ON ii.product_id = p.product_id
    WHERE ii.invoice_id = @InvoiceId;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoice_Items_Insert
    @InvoiceId  INT,
    @ProductId  INT,
    @Quantity   INT
AS
BEGIN
    DECLARE @UnitPrice DECIMAL(18,2);
    DECLARE @TotalPrice DECIMAL(18,2);

    -- Get UnitPrice from Products table
    SELECT @UnitPrice = product_price 
    FROM Products 
    WHERE product_id = @ProductId;

    -- Calculate TotalPrice
    SET @TotalPrice = @UnitPrice * @Quantity;

    BEGIN TRANSACTION;

    INSERT INTO InvoiceItems (invoice_id, product_id, quantity, unit_price, total_price)
    VALUES (@InvoiceId, @ProductId, @Quantity, @UnitPrice, @TotalPrice);

    -- Update Invoice Total Amount
    UPDATE Invoices 
    SET total_amount = total_amount + @TotalPrice
    WHERE invoice_id = @InvoiceId;

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoice_Items_Update
    @InvoiceItemId INT,
    @InvoiceId     INT,
    @ProductId     INT,
    @Quantity      INT
AS
BEGIN
    DECLARE @UnitPrice DECIMAL(18,2);
    DECLARE @NewTotalPrice DECIMAL(18,2);
    DECLARE @OldTotalPrice DECIMAL(18,2);

    -- Get OldTotalPrice to adjust Invoice Total
    SELECT @OldTotalPrice = total_price 
    FROM InvoiceItems 
    WHERE invoice_item_id = @InvoiceItemId;

    -- Get UnitPrice from Products table
    SELECT @UnitPrice = product_price 
    FROM Products 
    WHERE product_id = @ProductId;

    -- Calculate NewTotalPrice
    SET @NewTotalPrice = @UnitPrice * @Quantity;

    BEGIN TRANSACTION;

    UPDATE InvoiceItems
    SET invoice_id  = @InvoiceId,
        product_id  = @ProductId,
        quantity    = @Quantity,
        unit_price  = @UnitPrice,
        total_price = @NewTotalPrice
    WHERE invoice_item_id = @InvoiceItemId;

    -- Adjust Invoice Total Amount
    UPDATE Invoices 
    SET total_amount = total_amount - @OldTotalPrice + @NewTotalPrice
    WHERE invoice_id = @InvoiceId;

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoice_Items_Delete
@InvoiceItemId INT
AS
BEGIN
    DECLARE @OldTotalPrice DECIMAL(18,2);
    DECLARE @InvoiceId INT;

    SELECT @OldTotalPrice = total_price, @InvoiceId = invoice_id
    FROM InvoiceItems 
    WHERE invoice_item_id = @InvoiceItemId;

    BEGIN TRANSACTION;

    DELETE FROM InvoiceItems WHERE invoice_item_id = @InvoiceItemId;

    -- Adjust Invoice Total Amount
    IF @InvoiceId IS NOT NULL
    BEGIN
        UPDATE Invoices 
        SET total_amount = total_amount - @OldTotalPrice
        WHERE invoice_id = @InvoiceId;
    END

    COMMIT TRANSACTION;
END
GO

-- ============================================================
-- STORED PROCEDURES: Stock Transactions
-- ============================================================

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Select_All
AS
BEGIN
    SELECT st.transaction_id,
           st.product_id,
           p.product_name,
           st.user_id,
           u.user_name,
           st.stock_quantity,
           st.date,
           st.transaction_state,
           st.transaction_description
    FROM StockTransactions st
             INNER JOIN Products p ON st.product_id = p.product_id
             INNER JOIN Users    u ON st.user_id    = u.user_id
    ORDER BY st.date DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Select_By_ProductId
@ProductId INT
AS
BEGIN
    SELECT st.transaction_id,
           st.product_id,
           st.user_id,
           u.user_name,
           st.stock_quantity,
           st.date,
           st.transaction_state,
           st.transaction_description
    FROM StockTransactions st
             INNER JOIN Users u ON st.user_id = u.user_id
    WHERE st.product_id = @ProductId
    ORDER BY st.date DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Insert
    @ProductId              INT,
    @StockQuantity          INT,
    @TransactionState       NVARCHAR(20),  -- 'Purchase' or 'Sale'
    @TransactionDescription NVARCHAR(500) = NULL,
    @UserId                 INT
AS
BEGIN
    IF @TransactionState = 'Sale'
        BEGIN
            DECLARE @CurrentStock INT;
            SELECT @CurrentStock = stock_quantity FROM Products WHERE product_id = @ProductId;

            IF @CurrentStock < @StockQuantity
                BEGIN
                    RAISERROR('Insufficient stock for this transaction.', 16, 1);
                    RETURN;
                END
        END

    BEGIN TRANSACTION;

    INSERT INTO StockTransactions
    (product_id, user_id, stock_quantity, date, transaction_state, transaction_description)
    VALUES
        (@ProductId, @UserId, @StockQuantity, GETDATE(), @TransactionState, @TransactionDescription);

    IF @TransactionState = 'Purchase'
        UPDATE Products SET stock_quantity = stock_quantity + @StockQuantity WHERE product_id = @ProductId;
    ELSE IF @TransactionState = 'Sale'
        UPDATE Products SET stock_quantity = stock_quantity - @StockQuantity WHERE product_id = @ProductId;

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Update
    @TransactionId          INT,
    @ProductId              INT,
    @StockQuantity          INT,
    @TransactionState       NVARCHAR(20),
    @TransactionDescription NVARCHAR(500),
    @UserId                 INT
AS
BEGIN
    DECLARE @OldProductId INT, @OldQuantity INT, @OldState NVARCHAR(20);

    -- Get Old Transaction Details
    SELECT @OldProductId = product_id, @OldQuantity = stock_quantity, @OldState = transaction_state
    FROM StockTransactions
    WHERE transaction_id = @TransactionId;

    BEGIN TRANSACTION;

    -- Reverse old transaction from Products
    IF @OldState = 'Purchase'
        UPDATE Products SET stock_quantity = stock_quantity - @OldQuantity WHERE product_id = @OldProductId;
    ELSE IF @OldState = 'Sale'
        UPDATE Products SET stock_quantity = stock_quantity + @OldQuantity WHERE product_id = @OldProductId;

    -- Update the transaction record
    UPDATE StockTransactions
    SET product_id              = @ProductId,
        user_id                 = @UserId,
        stock_quantity          = @StockQuantity,
        transaction_state       = @TransactionState,
        transaction_description = @TransactionDescription
    WHERE transaction_id = @TransactionId;

    -- Apply new transaction to Products
    IF @TransactionState = 'IN'
        UPDATE Products SET stock_quantity = stock_quantity + @StockQuantity WHERE product_id = @ProductId;
    ELSE IF @TransactionState = 'OUT'
        UPDATE Products SET stock_quantity = stock_quantity - @StockQuantity WHERE product_id = @ProductId;

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Delete
@TransactionId INT
AS
BEGIN
    DECLARE @OldProductId INT, @OldQuantity INT, @OldState NVARCHAR(20);

    SELECT @OldProductId = product_id, @OldQuantity = stock_quantity, @OldState = transaction_state
    FROM StockTransactions
    WHERE transaction_id = @TransactionId;

    BEGIN TRANSACTION;

    DELETE FROM StockTransactions WHERE transaction_id = @TransactionId;

    -- Reverse old transaction from Products
    IF @OldState = 'Purchase'
        UPDATE Products SET stock_quantity = stock_quantity - @OldQuantity WHERE product_id = @OldProductId;
    ELSE IF @OldState = 'Sale'
        UPDATE Products SET stock_quantity = stock_quantity + @OldQuantity WHERE product_id = @OldProductId;

    COMMIT TRANSACTION;
END
GO

-- ============================================================
-- STORED PROCEDURES: Dashboard
-- ============================================================

CREATE OR ALTER PROCEDURE FetchDashboardStatistics
AS
BEGIN
    SELECT
        (SELECT COUNT(*)                     FROM Customers)                               AS TotalCustomers,
        (SELECT COUNT(*)                     FROM Users    WHERE status = 'Active')        AS TotalUsers,
        (SELECT COUNT(*)                     FROM Products WHERE status = 'Available')        AS TotalProducts,
        (SELECT ISNULL(SUM(total_amount), 0) FROM Invoices WHERE payment_status = 'Paid') AS TotalSales;
END
GO

CREATE OR ALTER PROCEDURE GetLowStockProducts
@Threshold INT = 10
AS
BEGIN
    SELECT product_id, product_name, stock_quantity
    FROM Products
    WHERE stock_quantity <= @Threshold
      AND status = 'Available'
    ORDER BY stock_quantity ASC;
END
GO

CREATE OR ALTER PROCEDURE GetCustomerPurchaseHistory
@CustomerId INT
AS
BEGIN
    SELECT i.invoice_id,
           i.date,
           ii.product_id,
           p.product_name,
           ii.quantity,
           ii.total_price
    FROM Invoices i
             INNER JOIN InvoiceItems ii ON i.invoice_id  = ii.invoice_id
             INNER JOIN Products     p  ON ii.product_id = p.product_id
    WHERE i.customer_id = @CustomerId
    ORDER BY i.date DESC;
END
GO

CREATE OR ALTER PROCEDURE GetSalesReport
    @StartDate DATETIME,
    @EndDate   DATETIME
AS
BEGIN
    SELECT i.invoice_id,
           i.date,
           c.customer_name,
           i.total_amount
    FROM Invoices i
             INNER JOIN Customers c ON i.customer_id = c.customer_id
    WHERE i.date BETWEEN @StartDate AND @EndDate
      AND i.payment_status = 'Paid'
    ORDER BY i.date DESC;
END
GO

-- ============================================================
-- STORED PROCEDURES: Sales Dashboard (all filtered by user_id)
-- ============================================================

CREATE OR ALTER PROCEDURE GetSalesTrend
@UserId INT
AS
BEGIN
    SELECT FORMAT(i.date, 'MMM yyyy') AS SalesMonth,
           SUM(i.total_amount)         AS TotalSales
    FROM Invoices i
    WHERE i.user_id        = @UserId
      AND i.payment_status = 'Paid'
    GROUP BY FORMAT(i.date, 'MMM yyyy'), YEAR(i.date), MONTH(i.date)
    ORDER BY YEAR(i.date), MONTH(i.date);
END
GO

CREATE OR ALTER PROCEDURE GetTopSellingProducts
@UserId INT
AS
BEGIN
    SELECT TOP 10
        p.product_name,
        SUM(ii.quantity) AS TotalSold
    FROM InvoiceItems ii
             INNER JOIN Products p ON ii.product_id = p.product_id
             INNER JOIN Invoices i ON ii.invoice_id = i.invoice_id
    WHERE i.user_id        = @UserId
      AND i.payment_status = 'Paid'
    GROUP BY p.product_name
    ORDER BY TotalSold DESC;
END
GO

CREATE OR ALTER PROCEDURE GetSalesByCategory
@UserId INT
AS
BEGIN
    SELECT c.category_name,
           SUM(ii.total_price) AS TotalRevenue
    FROM InvoiceItems ii
             INNER JOIN Products   p ON ii.product_id = p.product_id
             INNER JOIN Categories c ON p.category_id = c.category_id
             INNER JOIN Invoices   i ON ii.invoice_id = i.invoice_id
    WHERE i.user_id        = @UserId
      AND i.payment_status = 'Paid'
    GROUP BY c.category_name
    ORDER BY TotalRevenue DESC;
END
GO

CREATE OR ALTER PROCEDURE GetDailySales
@UserId INT
AS
BEGIN
    SELECT FORMAT(i.date, 'dd MMM yyyy') AS SalesDate,
           SUM(i.total_amount)            AS TotalSales
    FROM Invoices i
    WHERE i.user_id        = @UserId
      AND i.payment_status = 'Paid'
    GROUP BY FORMAT(i.date, 'dd MMM yyyy'), CAST(i.date AS DATE)
    ORDER BY CAST(i.date AS DATE) DESC;
END
GO

CREATE OR ALTER PROCEDURE GetTopCustomers
@UserId INT
AS
BEGIN
    SELECT TOP 10
        c.customer_name,
        COUNT(i.invoice_id) AS PurchaseCount
    FROM Invoices  i
             INNER JOIN Customers c ON i.customer_id = c.customer_id
    WHERE i.user_id        = @UserId
      AND i.payment_status = 'Paid'
    GROUP BY c.customer_name
    ORDER BY PurchaseCount DESC;
END
GO

CREATE OR ALTER PROCEDURE GetPaymentStatus
@UserId INT
AS
BEGIN
    SELECT payment_status,
           COUNT(*) AS TotalInvoices
    FROM Invoices
    WHERE user_id = @UserId
    GROUP BY payment_status;
END
GO

-- ============================================================
-- SEED DATA
-- ============================================================

INSERT INTO Categories (category_name, description) VALUES
                                                        ('Smartphones', 'Mobile phones and smartphones'),
                                                        ('Accessories', 'Mobile accessories such as cases, chargers, cables'),
                                                        ('Tablets',     'Tablet devices and iPad variants'),
                                                        ('Wearables',   'Smartwatches and fitness trackers'),
                                                        ('Audio',       'Headphones, earbuds and speakers');

INSERT INTO Brands (brand_name) VALUES
                                    ('Apple'), ('Samsung'), ('OnePlus'), ('Xiaomi'), ('Oppo'), ('Vivo'), ('Realme'), ('Sony');

-- Roles: 'Admin' and 'Sales' ONLY (verified from AddUser.cshtml: value="Admin" / value="Sales")
INSERT INTO Users (user_name, user_email, password, role, status) VALUES
                                                                      ('Admin User', 'admin@mobileshop.com', 'Admin@123', 'Admin', 'Active'),
                                                                      ('Sales User', 'sales@mobileshop.com', 'Sales@123', 'Sales', 'Active');

INSERT INTO Customers (customer_name, customer_email, customer_phone, customer_address) VALUES
                                                                                            ('Rahul Shah',  'rahul@gmail.com', '9876543210', 'Ahmedabad, Gujarat'),
                                                                                            ('Priya Patel', 'priya@gmail.com', '9876543211', 'Surat, Gujarat'),
                                                                                            ('Amit Mehta',  'amit@gmail.com',  '9876543212', 'Vadodara, Gujarat');

INSERT INTO Products (category_id, product_brand_id, product_name, product_price, stock_quantity, status) VALUES
                                                                                                              (1, 1, 'iPhone 15 Pro',         129900.00, 25, 'Available'),
                                                                                                              (1, 2, 'Samsung Galaxy S24',     89999.00, 30, 'Available'),
                                                                                                              (1, 3, 'OnePlus 12',             64999.00, 20, 'Available'),
                                                                                                              (2, 1, 'Apple MagSafe Charger',   3999.00, 50, 'Available'),
                                                                                                              (3, 2, 'Samsung Galaxy Tab S9',  72999.00, 15, 'Available'),
                                                                                                              (4, 2, 'Samsung Galaxy Watch 6', 29999.00, 18, 'Available');

-- ============================================================
-- END OF SCRIPT
-- Stored Procedures Summary (34 total):
--
-- Categories (6):  PR_Categories_Select_All, PR_Categories_Select_By_Id,
--                  PR_Categories_Insert, PR_Categories_Update,
--                  PR_Categories_Delete, PR_Categories_Dropdown
--
-- Brands (6):      PR_Brands_Select_All, PR_Brands_Select_By_Id,
--                  PR_Brands_Insert, PR_Brands_Update,
--                  PR_Brands_Delete, PR_Brands_Dropdown
--
-- Users (8):       PR_Users_Select_All, PR_Users_Select_By_Id,
--                  PR_Users_Insert, PR_Users_Update, PR_Users_Delete,
--                  PR_Users_Sign_In, PR_Users_Sign_Out, PR_Users_Dropdown
--
-- Customers (6):   PR_Customers_Select_All, PR_Customers_Select_By_Id,
--                  PR_Customers_Insert, PR_Customers_Update,
--                  PR_Customers_Delete, PR_Customers_Dropdown
--
-- Products (6):    PR_Products_Select_All, PR_Products_Select_By_Id,
--                  PR_Products_Insert, PR_Products_Update,
--                  PR_Products_Delete, PR_Products_Dropdown
--
-- Invoices (5):    PR_Invoices_Select_All, PR_Invoices_Select_By_Id,
--                  PR_Invoices_Insert, PR_Invoices_Update, PR_Invoices_Delete
--
-- InvoiceItems(6): PR_Invoice_Items_Select_All, PR_Invoice_Items_Select_By_Id,
--                  PR_Invoice_Items_Select_By_InvoiceId,
--                  PR_Invoice_Items_Insert, PR_Invoice_Items_Update,
--                  PR_Invoice_Items_Delete
--
-- Stock (5):       PR_Stock_Transactions_Select_All,
--                  PR_Stock_Transactions_Select_By_ProductId,
--                  PR_Stock_Transactions_Insert, PR_Stock_Transactions_Update,
--                  PR_Stock_Transactions_Delete
--
-- Dashboard (8):   FetchDashboardStatistics, GetLowStockProducts,
--                  GetCustomerPurchaseHistory, GetSalesReport,
--                  GetSalesTrend, GetTopSellingProducts, GetSalesByCategory,
--                  GetDailySales, GetTopCustomers, GetPaymentStatus
-- ============================================================

PRINT 'MobileShopDB created successfully.';
PRINT '8 Tables created with all foreign keys and indexes.';
PRINT 'Valid Roles: Admin, Sales';
PRINT '34 Stored Procedures created.';
GO