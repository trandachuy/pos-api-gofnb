-- UPDATE PURCHASE ORDER CODE
-- LOOP STORES AND INITITAL STORE CONFIG
DECLARE @Code AS NVARCHAR(15)
DECLARE @PurchaseOrderPrefix AS VARCHAR(100)
DECLARE @CurrentIndex int

SET @PurchaseOrderPrefix = 'PO-'
SET @CurrentIndex = 1

DECLARE @StoreId uniqueidentifier
DECLARE STORE_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR
SELECT Id FROM Store ORDER BY CreatedTime ASC
OPEN STORE_CURSOR
FETCH NEXT FROM STORE_CURSOR INTO @StoreId
WHILE @@FETCH_STATUS = 0
BEGIN 
	-- RESET INDEX
	SET @CurrentIndex = 1

	-- UPDATE PURCHASE ORDER CODE FOR EACH STORE
	DECLARE @RecordId uniqueidentifier
	DECLARE TABLE_RECORD_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR
	SELECT Id FROM PurchaseOrder WHERE StoreId = @StoreId ORDER BY CreatedTime ASC
	OPEN TABLE_RECORD_CURSOR
	FETCH NEXT FROM TABLE_RECORD_CURSOR INTO @RecordId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF LEN(@CurrentIndex) = 1 SET @Code = CONCAT(@PurchaseOrderPrefix, '000', @CurrentIndex)
		IF LEN(@CurrentIndex) = 2 SET @Code = CONCAT(@PurchaseOrderPrefix, '00', @CurrentIndex)
		IF LEN(@CurrentIndex) = 3 SET @Code = CONCAT(@PurchaseOrderPrefix, '0', @CurrentIndex)
		IF LEN(@CurrentIndex) = 4 SET @Code = CONCAT(@PurchaseOrderPrefix, @CurrentIndex)

		IF LEN(@Code) > 0
			UPDATE PurchaseOrder SET Code = @Code WHERE Id = @RecordId
			SET @CurrentIndex = @CurrentIndex + 1

		FETCH NEXT FROM TABLE_RECORD_CURSOR INTO @RecordId
	END
	CLOSE TABLE_RECORD_CURSOR
	DEALLOCATE TABLE_RECORD_CURSOR

    -- UPDATE STORE CONFIG
	IF EXISTS(SELECT 1 FROM [StoreConfig] WHERE StoreId = @StoreId)
		BEGIN
			UPDATE [StoreConfig] SET CurrentMaxPurchaseOrderCode = @CurrentIndex WHERE StoreId = @StoreId
		END
	ELSE
		BEGIN
			INSERT [StoreConfig] ([Id], [StoreId], [CurrentMaxPurchaseOrderCode], [CurrentMaxOrderCode], [LastSavedUser], [LastSavedTime], [CreatedUser], [CreatedTime]) 
			VALUES (NEWID() , @StoreId , @CurrentIndex, 1, NULL, GETDATE(), NULL, GETDATE())
		END

	-- MOVE NEXT ELEMENT
    FETCH NEXT FROM STORE_CURSOR INTO @StoreId
END
CLOSE STORE_CURSOR
DEALLOCATE STORE_CURSOR
