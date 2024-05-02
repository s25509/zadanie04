CREATE PROCEDURE AddProductToWarehouse @IdProduct INT,
                                       @IdWarehouse INT,
                                       @Amount INT,
                                       @CreatedAt DATETIME
AS
BEGIN
    SET XACT_ABORT ON; --good call, but I'm moving it here so RAISERRORS terminate code execution too
    
    DECLARE @IdOrder INT, @OrderCreatedAt DATETIME, @Price DECIMAL(25, 2);

    SELECT TOP 1 @IdOrder = IdOrder, @OrderCreatedAt = CreatedAt
    FROM "Order"
    WHERE IdProduct = @IdProduct
      AND Amount = @Amount;

    SELECT @Price = Product.Price FROM Product WHERE IdProduct = @IdProduct

    IF @Amount < 1
        BEGIN
            RAISERROR ('Invalid parameter: Provided Amount cannot be less than 1', 15, 21);
            RETURN; -- this is bullshit, in some versions RAISERROR will terminate execution if the severity is set to 17 or 18,
                    -- in others not, the XACT_ABORT option is supposed to impact the behavior of the RAISERROR statement,
                    -- or not, figure it out by trial and error, because the official documentation is written by madmen...
        END;

    IF @Price IS NULL
        BEGIN
            RAISERROR ('Invalid parameter: Product with ID: %i was not found', 15, 22, @IdProduct);
            RETURN; -- I am keeping this workaround even tho my IDE screams: "Unreachable code"
        END;

    IF @IdOrder IS NULL
        BEGIN
            RAISERROR ('Invalid parameter: Order with Product ID: %i and Amount: %i was not found', 15, 23, @IdProduct, @Amount);
            RETURN;
        END;

    IF NOT EXISTS(SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse)
        BEGIN
            RAISERROR ('Invalid parameter: Warehouse with ID: %i was not found', 15, 24, @IdWarehouse);
            RETURN;
        END;

    IF EXISTS(SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder)
        BEGIN
            RAISERROR ('Invalid parameter: Order with ID: %i is already being processed', 15, 25, @IdOrder);
            RETURN;
        END;

    IF @OrderCreatedAt > @CreatedAt
        BEGIN
            RAISERROR ('Invalid parameter: Order with ID: %i was created LATER than this request', 15, 26, @IdOrder);
            RETURN;
        END;

    BEGIN TRAN;

    UPDATE "Order"
    SET FulfilledAt=@CreatedAt
    WHERE IdOrder = @IdOrder;

    INSERT INTO Product_Warehouse(IdWarehouse,
                                  IdProduct, IdOrder, Amount, Price, CreatedAt)
    OUTPUT INSERTED.*
    VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount * @Price, @CreatedAt);

    COMMIT;
END