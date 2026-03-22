-- ============================================================
-- Mobile Shop Management System
-- Database Creation Script
-- Generated from: mobile_shop_web_api (.NET Web API)
-- ============================================================

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
-- ============================================================
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;

CREATE TABLE Categories (
    category_id       INT IDENTITY(1,1) PRIMARY KEY,
    category_name     NVARCHAR(100)     NOT NULL,
    description       NVARCHAR(500)     NULL,
    created_date      DATETIME          NOT NULL DEFAULT GETDATE(),
    modified_date     DATETIME          NULL
);
GO

-- ============================================================
-- TABLE: Brands
-- ============================================================
IF OBJECT_ID('dbo.Brands', 'U') IS NOT NULL DROP TABLE dbo.Brands;

CREATE TABLE Brands (
    brand_id          INT IDENTITY(1,1) PRIMARY KEY,
    brand_name        NVARCHAR(100)     NOT NULL,
    created_date      DATETIME          NOT NULL DEFAULT GETDATE(),
    modified_date     DATETIME          NULL
);
GO

-- ============================================================
-- TABLE: Users
-- ============================================================
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

CREATE TABLE Users (
    user_id           INT IDENTITY(1,1) PRIMARY KEY,
    user_name         NVARCHAR(50)      NOT NULL,
    user_email        NVARCHAR(100)     NOT NULL UNIQUE,
    password          NVARCHAR(256)     NOT NULL,
    role              NVARCHAR(20)      NOT NULL,   -- e.g. 'Admin', 'Staff'
    status            NVARCHAR(20)      NOT NULL,   -- e.g. 'Active', 'Inactive'
    created_date      DATETIME          NOT NULL DEFAULT GETDATE(),
    modified_date     DATETIME          NULL
);
GO

-- ============================================================
-- TABLE: Customers
-- ============================================================
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;

CREATE TABLE Customers (
    customer_id       INT IDENTITY(1,1) PRIMARY KEY,
    customer_name     NVARCHAR(100)     NOT NULL,
    customer_email    NVARCHAR(100)     NOT NULL UNIQUE,
    customer_phone    NVARCHAR(20)      NOT NULL,
    customer_address  NVARCHAR(300)     NOT NULL,
    created_date      DATETIME          NOT NULL DEFAULT GETDATE(),
    modified_date     DATETIME          NULL
);
GO

-- ============================================================
-- TABLE: Products
-- ============================================================
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;

CREATE TABLE Products (
    product_id          INT IDENTITY(1,1) PRIMARY KEY,
    category_id         INT               NOT NULL,
    product_brand_id    INT               NOT NULL,
    product_name        NVARCHAR(100)     NOT NULL,
    product_image       NVARCHAR(500)     NULL,
    product_price       DECIMAL(18,2)     NOT NULL CHECK (product_price > 0),
    stock_quantity      INT               NOT NULL CHECK (stock_quantity >= 0),
    product_description NVARCHAR(1000)    NULL,
    status              NVARCHAR(20)      NOT NULL,   -- e.g. 'Active', 'Inactive'
    created_date        DATETIME          NOT NULL DEFAULT GETDATE(),
    modified_date       DATETIME          NULL,

    CONSTRAINT FK_Products_Category FOREIGN KEY (category_id)
        REFERENCES Categories(category_id),
    CONSTRAINT FK_Products_Brand FOREIGN KEY (product_brand_id)
        REFERENCES Brands(brand_id)
);
GO

-- ============================================================
-- TABLE: Invoices
-- ============================================================
IF OBJECT_ID('dbo.Invoices', 'U') IS NOT NULL DROP TABLE dbo.Invoices;

CREATE TABLE Invoices (
    invoice_id        INT IDENTITY(1,1) PRIMARY KEY,
    customer_id       INT               NOT NULL,
    user_id           INT               NOT NULL,
    date              DATETIME          NOT NULL DEFAULT GETDATE(),
    total_amount      DECIMAL(18,2)     NOT NULL DEFAULT 0,
    payment_method    NVARCHAR(50)      NOT NULL,   -- e.g. 'Cash', 'Card', 'Online'
    payment_status    NVARCHAR(50)      NOT NULL,   -- e.g. 'Paid', 'Pending', 'Cancelled'

    CONSTRAINT FK_Invoices_Customer FOREIGN KEY (customer_id)
        REFERENCES Customers(customer_id),
    CONSTRAINT FK_Invoices_User FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
);
GO

-- ============================================================
-- TABLE: InvoiceItems
-- ============================================================
IF OBJECT_ID('dbo.InvoiceItems', 'U') IS NOT NULL DROP TABLE dbo.InvoiceItems;

CREATE TABLE InvoiceItems (
    invoice_item_id   INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id        INT               NOT NULL,
    product_id        INT               NOT NULL,
    quantity          INT               NOT NULL CHECK (quantity > 0),
    unit_price        DECIMAL(18,2)     NOT NULL,
    total_price       DECIMAL(18,2)     NOT NULL,

    CONSTRAINT FK_InvoiceItems_Invoice FOREIGN KEY (invoice_id)
        REFERENCES Invoices(invoice_id) ON DELETE CASCADE,
    CONSTRAINT FK_InvoiceItems_Product FOREIGN KEY (product_id)
        REFERENCES Products(product_id)
);
GO

-- ============================================================
-- TABLE: StockTransactions
-- ============================================================
IF OBJECT_ID('dbo.StockTransactions', 'U') IS NOT NULL DROP TABLE dbo.StockTransactions;

CREATE TABLE StockTransactions (
    transaction_id          INT IDENTITY(1,1) PRIMARY KEY,
    product_id              INT               NOT NULL,
    user_id                 INT               NOT NULL,
    stock_quantity          INT               NOT NULL,
    date                    DATETIME          NOT NULL DEFAULT GETDATE(),
    transaction_state       NVARCHAR(20)      NOT NULL,  -- e.g. 'IN', 'OUT'
    transaction_description NVARCHAR(500)     NULL,

    CONSTRAINT FK_StockTrans_Product FOREIGN KEY (product_id)
        REFERENCES Products(product_id),
    CONSTRAINT FK_StockTrans_User FOREIGN KEY (user_id)
        REFERENCES Users(user_id)
);
GO

-- ============================================================
-- INDEXES
-- ============================================================
CREATE INDEX IX_Products_CategoryId     ON Products(category_id);
CREATE INDEX IX_Products_BrandId        ON Products(product_brand_id);
CREATE INDEX IX_Invoices_CustomerId     ON Invoices(customer_id);
CREATE INDEX IX_Invoices_UserId         ON Invoices(user_id);
CREATE INDEX IX_Invoices_Date           ON Invoices(date);
CREATE INDEX IX_InvoiceItems_InvoiceId  ON InvoiceItems(invoice_id);
CREATE INDEX IX_InvoiceItems_ProductId  ON InvoiceItems(product_id);
CREATE INDEX IX_StockTrans_ProductId    ON StockTransactions(product_id);
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
    @CategoryName   NVARCHAR(100),
    @Description    NVARCHAR(500) = NULL
AS
BEGIN
    INSERT INTO Categories (category_name, description, created_date)
    VALUES (@CategoryName, @Description, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Categories_Update
    @CategoryId     INT,
    @CategoryName   NVARCHAR(100),
    @Description    NVARCHAR(500) = NULL
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
    @BrandId    INT,
    @BrandName  NVARCHAR(100)
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

-- ============================================================
-- STORED PROCEDURES: Users
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
    @UserName   NVARCHAR(50),
    @UserEmail  NVARCHAR(100),
    @Password   NVARCHAR(256),
    @Role       NVARCHAR(20),
    @Status     NVARCHAR(20)
AS
BEGIN
    INSERT INTO Users (user_name, user_email, password, role, status, created_date)
    VALUES (@UserName, @UserEmail, @Password, @Role, @Status, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Users_Update
    @UserId     INT,
    @UserName   NVARCHAR(50),
    @UserEmail  NVARCHAR(100),
    @Password   NVARCHAR(256),
    @Role       NVARCHAR(20),
    @Status     NVARCHAR(20)
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

CREATE OR ALTER PROCEDURE PR_Users_Sign_In
    @UserEmail  NVARCHAR(100),
    @Password   NVARCHAR(256)
AS
BEGIN
    DECLARE @UserId INT;
    SELECT @UserId = user_id FROM Users
    WHERE user_email = @UserEmail AND password = @Password AND status = 'Active';

    IF @UserId IS NOT NULL
        SELECT user_id, user_name, user_email, password, role, status,
               created_date, modified_date,
               'Login successful' AS Message
        FROM Users WHERE user_id = @UserId;
    ELSE
        SELECT NULL AS user_id, NULL AS user_name, NULL AS user_email,
               NULL AS password, NULL AS role, NULL AS status,
               NULL AS created_date, NULL AS modified_date,
               'Invalid Email or Password' AS Message;
END
GO

CREATE OR ALTER PROCEDURE PR_Users_Sign_Out
    @UserEmail  NVARCHAR(100),
    @Password   NVARCHAR(256)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE user_email = @UserEmail AND password = @Password)
        SELECT 'Logout successful' AS Message;
    ELSE
        SELECT 'Invalid Email or Password' AS Message;
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
    @CustomerName       NVARCHAR(100),
    @CustomerEmail      NVARCHAR(100),
    @CustomerPhone      NVARCHAR(20),
    @CustomerAddress    NVARCHAR(300)
AS
BEGIN
    INSERT INTO Customers (customer_name, customer_email, customer_phone, customer_address, created_date)
    VALUES (@CustomerName, @CustomerEmail, @CustomerPhone, @CustomerAddress, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Customers_Update
    @CustomerId         INT,
    @CustomerName       NVARCHAR(100),
    @CustomerEmail      NVARCHAR(100),
    @CustomerPhone      NVARCHAR(20),
    @CustomerAddress    NVARCHAR(300)
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

-- ============================================================
-- STORED PROCEDURES: Products
-- ============================================================
CREATE OR ALTER PROCEDURE PR_Products_Select_All
    @CategoryId INT = NULL,
    @BrandId    INT = NULL
AS
BEGIN
    SELECT p.product_id, c.category_name, b.brand_name,
           p.product_name, p.product_image, p.product_price,
           p.stock_quantity, p.product_description, p.status,
           p.created_date, p.modified_date,
           p.category_id, p.product_brand_id
    FROM Products p
    INNER JOIN Categories c ON p.category_id = c.category_id
    INNER JOIN Brands b     ON p.product_brand_id = b.brand_id
    WHERE (@CategoryId IS NULL OR p.category_id = @CategoryId)
      AND (@BrandId    IS NULL OR p.product_brand_id = @BrandId)
    ORDER BY p.product_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Select_By_Id
    @ProductId INT
AS
BEGIN
    SELECT p.product_id, c.category_name, b.brand_name,
           p.product_name, p.product_image, p.product_price,
           p.stock_quantity, p.product_description, p.status,
           p.created_date, p.modified_date,
           p.category_id, p.product_brand_id
    FROM Products p
    INNER JOIN Categories c ON p.category_id = c.category_id
    INNER JOIN Brands b     ON p.product_brand_id = b.brand_id
    WHERE p.product_id = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Insert
    @CategoryId         INT,
    @ProductName        NVARCHAR(100),
    @ProductImage       NVARCHAR(500) = NULL,
    @ProductBrandId     INT,
    @ProductPrice       DECIMAL(18,2),
    @StockQuantity      INT,
    @ProductDescription NVARCHAR(1000) = NULL,
    @Status             NVARCHAR(20)
AS
BEGIN
    INSERT INTO Products (category_id, product_name, product_image, product_brand_id,
                          product_price, stock_quantity, product_description, status, created_date)
    VALUES (@CategoryId, @ProductName, @ProductImage, @ProductBrandId,
            @ProductPrice, @StockQuantity, @ProductDescription, @Status, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE PR_Products_Update
    @ProductId          INT,
    @CategoryId         INT,
    @ProductName        NVARCHAR(100),
    @ProductImage       NVARCHAR(500) = NULL,
    @ProductBrandId     INT,
    @ProductPrice       DECIMAL(18,2),
    @StockQuantity      INT,
    @ProductDescription NVARCHAR(1000) = NULL,
    @Status             NVARCHAR(20)
AS
BEGIN
    UPDATE Products
    SET category_id         = @CategoryId,
        product_name        = @ProductName,
        product_image       = @ProductImage,
        product_brand_id    = @ProductBrandId,
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

-- ============================================================
-- STORED PROCEDURES: Invoices
-- ============================================================
CREATE OR ALTER PROCEDURE PR_Invoices_Select_All
    @UserId     INT,
    @CustomerId INT    = NULL,
    @Status     NVARCHAR(50) = NULL
AS
BEGIN
    SELECT i.invoice_id, i.customer_id, i.user_id,
           c.customer_name, u.user_name,
           i.date, i.total_amount, i.payment_method, i.payment_status
    FROM Invoices i
    INNER JOIN Customers c ON i.customer_id = c.customer_id
    INNER JOIN Users u     ON i.user_id     = u.user_id
    WHERE i.user_id = @UserId
      AND (@CustomerId IS NULL OR i.customer_id = @CustomerId)
      AND (@Status     IS NULL OR i.payment_status = @Status)
    ORDER BY i.date DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Select_By_Id
    @InvoiceId INT
AS
BEGIN
    SELECT i.invoice_id, i.customer_id, i.user_id,
           c.customer_name, u.user_name,
           i.date, i.total_amount, i.payment_method, i.payment_status
    FROM Invoices i
    INNER JOIN Customers c ON i.customer_id = c.customer_id
    INNER JOIN Users u     ON i.user_id     = u.user_id
    WHERE i.invoice_id = @InvoiceId;
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Insert
    @CustomerId     INT,
    @UserId         INT,
    @TotalAmount    DECIMAL(18,2),
    @PaymentMethod  NVARCHAR(50),
    @PaymentStatus  NVARCHAR(50),
    @InvoiceId      INT OUTPUT
AS
BEGIN
    INSERT INTO Invoices (customer_id, user_id, date, total_amount, payment_method, payment_status)
    VALUES (@CustomerId, @UserId, GETDATE(), @TotalAmount, @PaymentMethod, @PaymentStatus);

    SET @InvoiceId = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE PR_Invoices_Update
    @InvoiceId      INT,
    @CustomerId     INT,
    @UserId         INT,
    @TotalAmount    DECIMAL(18,2),
    @PaymentMethod  NVARCHAR(50),
    @PaymentStatus  NVARCHAR(50)
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
-- STORED PROCEDURES: InvoiceItems
-- ============================================================
CREATE OR ALTER PROCEDURE PR_InvoiceItems_Select_All
AS
BEGIN
    SELECT ii.invoice_item_id, ii.invoice_id, ii.product_id,
           p.product_name, ii.quantity, ii.unit_price, ii.total_price
    FROM InvoiceItems ii
    INNER JOIN Products p ON ii.product_id = p.product_id
    ORDER BY ii.invoice_id;
END
GO

CREATE OR ALTER PROCEDURE PR_InvoiceItems_Select_By_InvoiceId
    @InvoiceId INT
AS
BEGIN
    SELECT ii.invoice_item_id, ii.invoice_id, ii.product_id,
           p.product_name, ii.quantity, ii.unit_price, ii.total_price
    FROM InvoiceItems ii
    INNER JOIN Products p ON ii.product_id = p.product_id
    WHERE ii.invoice_id = @InvoiceId;
END
GO

CREATE OR ALTER PROCEDURE PR_InvoiceItems_Insert
    @InvoiceId  INT,
    @ProductId  INT,
    @Quantity   INT,
    @UnitPrice  DECIMAL(18,2),
    @TotalPrice DECIMAL(18,2)
AS
BEGIN
    INSERT INTO InvoiceItems (invoice_id, product_id, quantity, unit_price, total_price)
    VALUES (@InvoiceId, @ProductId, @Quantity, @UnitPrice, @TotalPrice);
END
GO

CREATE OR ALTER PROCEDURE PR_InvoiceItems_Update
    @InvoiceItemId  INT,
    @InvoiceId      INT,
    @ProductId      INT,
    @Quantity       INT,
    @UnitPrice      DECIMAL(18,2),
    @TotalPrice     DECIMAL(18,2)
AS
BEGIN
    UPDATE InvoiceItems
    SET invoice_id  = @InvoiceId,
        product_id  = @ProductId,
        quantity    = @Quantity,
        unit_price  = @UnitPrice,
        total_price = @TotalPrice
    WHERE invoice_item_id = @InvoiceItemId;
END
GO

CREATE OR ALTER PROCEDURE PR_InvoiceItems_Delete
    @InvoiceItemId INT
AS
BEGIN
    DELETE FROM InvoiceItems WHERE invoice_item_id = @InvoiceItemId;
END
GO

-- ============================================================
-- STORED PROCEDURES: Stock Transactions
-- ============================================================
CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Select_All
AS
BEGIN
    SELECT st.transaction_id, st.product_id, p.product_name,
           st.user_id, u.user_name,
           st.stock_quantity, st.date, st.transaction_state, st.transaction_description
    FROM StockTransactions st
    INNER JOIN Products p ON st.product_id = p.product_id
    INNER JOIN Users u    ON st.user_id    = u.user_id
    ORDER BY st.date DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Select_By_ProductId
    @ProductId INT
AS
BEGIN
    SELECT st.transaction_id, st.product_id,
           st.user_id, u.user_name,
           st.stock_quantity, st.date, st.transaction_state, st.transaction_description
    FROM StockTransactions st
    INNER JOIN Users u ON st.user_id = u.user_id
    WHERE st.product_id = @ProductId
    ORDER BY st.date DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Insert
    @ProductId              INT,
    @StockQuantity          INT,
    @TransactionState       NVARCHAR(20),
    @TransactionDescription NVARCHAR(500) = NULL,
    @UserId                 INT
AS
BEGIN
    -- Validate sufficient stock for OUT transactions
    IF @TransactionState = 'OUT'
    BEGIN
        DECLARE @CurrentStock INT;
        SELECT @CurrentStock = stock_quantity FROM Products WHERE product_id = @ProductId;
        IF @CurrentStock < @StockQuantity
            RAISERROR('Insufficient stock for this transaction.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
        INSERT INTO StockTransactions
            (product_id, user_id, stock_quantity, date, transaction_state, transaction_description)
        VALUES
            (@ProductId, @UserId, @StockQuantity, GETDATE(), @TransactionState, @TransactionDescription);

        -- Update product stock
        IF @TransactionState = 'IN'
            UPDATE Products SET stock_quantity = stock_quantity + @StockQuantity WHERE product_id = @ProductId;
        ELSE IF @TransactionState = 'OUT'
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
    UPDATE StockTransactions
    SET product_id              = @ProductId,
        user_id                 = @UserId,
        stock_quantity          = @StockQuantity,
        transaction_state       = @TransactionState,
        transaction_description = @TransactionDescription
    WHERE transaction_id = @TransactionId;
END
GO

CREATE OR ALTER PROCEDURE PR_Stock_Transactions_Delete
    @TransactionId INT
AS
BEGIN
    DELETE FROM StockTransactions WHERE transaction_id = @TransactionId;
END
GO

-- ============================================================
-- STORED PROCEDURES: Dashboard
-- ============================================================
CREATE OR ALTER PROCEDURE FetchDashboardStatistics
AS
BEGIN
    SELECT
        (SELECT COUNT(*) FROM Customers)                              AS TotalCustomers,
        (SELECT COUNT(*) FROM Users WHERE status = 'Active')         AS TotalUsers,
        (SELECT COUNT(*) FROM Products WHERE status = 'Active')      AS TotalProducts,
        (SELECT ISNULL(SUM(total_amount), 0) FROM Invoices
            WHERE payment_status = 'Paid')                           AS TotalSales;
END
GO

CREATE OR ALTER PROCEDURE GetLowStockProducts
    @threshold INT = 10
AS
BEGIN
    SELECT product_id, product_name, stock_quantity
    FROM Products
    WHERE stock_quantity <= @threshold AND status = 'Active'
    ORDER BY stock_quantity ASC;
END
GO

CREATE OR ALTER PROCEDURE GetCustomerPurchaseHistory
    @customer_id INT
AS
BEGIN
    SELECT i.invoice_id, i.date, ii.product_id,
           p.product_name, ii.quantity, ii.total_price
    FROM Invoices i
    INNER JOIN InvoiceItems ii ON i.invoice_id = ii.invoice_id
    INNER JOIN Products p      ON ii.product_id = p.product_id
    WHERE i.customer_id = @customer_id
    ORDER BY i.date DESC;
END
GO

CREATE OR ALTER PROCEDURE GetSalesReport
    @start_date DATETIME,
    @end_date   DATETIME
AS
BEGIN
    SELECT i.invoice_id, i.date, c.customer_name, i.total_amount
    FROM Invoices i
    INNER JOIN Customers c ON i.customer_id = c.customer_id
    WHERE i.date BETWEEN @start_date AND @end_date
      AND i.payment_status = 'Paid'
    ORDER BY i.date DESC;
END
GO

-- ============================================================
-- STORED PROCEDURES: Sales Dashboard
-- ============================================================
CREATE OR ALTER PROCEDURE GetSalesTrend
    @user_id INT
AS
BEGIN
    SELECT FORMAT(i.date, 'MMM yyyy') AS SalesMonth,
           SUM(i.total_amount)        AS TotalSales
    FROM Invoices i
    WHERE i.user_id = @user_id AND i.payment_status = 'Paid'
    GROUP BY FORMAT(i.date, 'MMM yyyy'), YEAR(i.date), MONTH(i.date)
    ORDER BY YEAR(i.date), MONTH(i.date);
END
GO

CREATE OR ALTER PROCEDURE GetTopSellingProducts
    @user_id INT
AS
BEGIN
    SELECT TOP 10 p.product_name, SUM(ii.quantity) AS TotalSold
    FROM InvoiceItems ii
    INNER JOIN Products p  ON ii.product_id = p.product_id
    INNER JOIN Invoices i  ON ii.invoice_id = i.invoice_id
    WHERE i.user_id = @user_id AND i.payment_status = 'Paid'
    GROUP BY p.product_name
    ORDER BY TotalSold DESC;
END
GO

CREATE OR ALTER PROCEDURE GetSalesByCategory
    @user_id INT
AS
BEGIN
    SELECT c.category_name, SUM(ii.total_price) AS TotalRevenue
    FROM InvoiceItems ii
    INNER JOIN Products p   ON ii.product_id  = p.product_id
    INNER JOIN Categories c ON p.category_id  = c.category_id
    INNER JOIN Invoices i   ON ii.invoice_id  = i.invoice_id
    WHERE i.user_id = @user_id AND i.payment_status = 'Paid'
    GROUP BY c.category_name
    ORDER BY TotalRevenue DESC;
END
GO

CREATE OR ALTER PROCEDURE GetDailySales
    @user_id INT
AS
BEGIN
    SELECT FORMAT(i.date, 'dd MMM yyyy') AS SalesDate,
           SUM(i.total_amount)           AS TotalSales
    FROM Invoices i
    WHERE i.user_id = @user_id AND i.payment_status = 'Paid'
    GROUP BY FORMAT(i.date, 'dd MMM yyyy'), CAST(i.date AS DATE)
    ORDER BY CAST(i.date AS DATE) DESC;
END
GO

CREATE OR ALTER PROCEDURE GetTopCustomers
    @user_id INT
AS
BEGIN
    SELECT TOP 10 c.customer_name, COUNT(i.invoice_id) AS PurchaseCount
    FROM Invoices i
    INNER JOIN Customers c ON i.customer_id = c.customer_id
    WHERE i.user_id = @user_id AND i.payment_status = 'Paid'
    GROUP BY c.customer_name
    ORDER BY PurchaseCount DESC;
END
GO

CREATE OR ALTER PROCEDURE GetPaymentStatus
    @user_id INT
AS
BEGIN
    SELECT payment_status, COUNT(*) AS TotalInvoices
    FROM Invoices
    WHERE user_id = @user_id
    GROUP BY payment_status;
END
GO

-- ============================================================
-- DROPDOWN PROCEDURE (for UI dropdowns)
-- ============================================================
CREATE OR ALTER PROCEDURE PR_Dropdown_Categories
AS
BEGIN
    SELECT category_id AS Id, category_name AS Name
    FROM Categories ORDER BY category_name;
END
GO

CREATE OR ALTER PROCEDURE PR_Dropdown_Brands
AS
BEGIN
    SELECT brand_id AS Id, brand_name AS Name
    FROM Brands ORDER BY brand_name;
END
GO

-- ============================================================
-- SAMPLE SEED DATA
-- ============================================================
INSERT INTO Categories (category_name, description) VALUES
('Smartphones',     'Mobile phones and smartphones'),
('Accessories',     'Mobile accessories such as cases, chargers, cables'),
('Tablets',         'Tablet devices and iPad variants'),
('Wearables',       'Smartwatches and fitness trackers'),
('Audio',           'Headphones, earbuds and speakers');

INSERT INTO Brands (brand_name) VALUES
('Apple'), ('Samsung'), ('OnePlus'), ('Xiaomi'), ('Oppo'), ('Vivo'), ('Realme'), ('Sony');

INSERT INTO Users (user_name, user_email, password, role, status) VALUES
('Admin User',  'admin@mobileshop.com',  'Admin@123',  'Admin', 'Active'),
('Staff User',  'staff@mobileshop.com',  'Staff@123',  'Staff', 'Active');

INSERT INTO Customers (customer_name, customer_email, customer_phone, customer_address) VALUES
('Rahul Shah',      'rahul@gmail.com',   '9876543210', 'Ahmedabad, Gujarat'),
('Priya Patel',     'priya@gmail.com',   '9876543211', 'Surat, Gujarat'),
('Amit Mehta',      'amit@gmail.com',    '9876543212', 'Vadodara, Gujarat');

INSERT INTO Products (category_id, product_brand_id, product_name, product_price, stock_quantity, status) VALUES
(1, 1, 'iPhone 15 Pro',          129900.00, 25, 'Active'),
(1, 2, 'Samsung Galaxy S24',      89999.00, 30, 'Active'),
(1, 3, 'OnePlus 12',              64999.00, 20, 'Active'),
(2, 1, 'Apple MagSafe Charger',    3999.00, 50, 'Active'),
(3, 2, 'Samsung Galaxy Tab S9',   72999.00, 15, 'Active'),
(4, 2, 'Samsung Galaxy Watch 6',  29999.00, 18, 'Active');

-- ============================================================
-- END OF SCRIPT
-- ============================================================
PRINT 'MobileShopDB created successfully with all tables, stored procedures, and seed data.';
GO
