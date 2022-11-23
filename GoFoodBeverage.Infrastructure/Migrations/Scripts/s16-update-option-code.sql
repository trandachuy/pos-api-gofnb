-- UPDATE OPTION CODE
-- LOOP STORES AND INITITAL STORE CONFIG
DECLARE @Code AS NVARCHAR(15)
DECLARE @Prefix AS VARCHAR(100)
DECLARE @CurrentIndex int

SET @Prefix = 'O'
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

	-- UPDATE OPTION CODE FOR EACH STORE
	DECLARE @RecordId uniqueidentifier
	DECLARE TABLE_RECORD_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR
	SELECT Id FROM [Option] WHERE StoreId = @StoreId ORDER BY CreatedTime ASC
	OPEN TABLE_RECORD_CURSOR
	FETCH NEXT FROM TABLE_RECORD_CURSOR INTO @RecordId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF LEN(@CurrentIndex) = 1 SET @Code = CONCAT(@Prefix, '000', @CurrentIndex)
		IF LEN(@CurrentIndex) = 2 SET @Code = CONCAT(@Prefix, '00', @CurrentIndex)
		IF LEN(@CurrentIndex) = 3 SET @Code = CONCAT(@Prefix, '0', @CurrentIndex)
		IF LEN(@CurrentIndex) = 4 SET @Code = CONCAT(@Prefix, @CurrentIndex)

		IF LEN(@Code) > 0
			UPDATE [Option] SET Code = @Code WHERE Id = @RecordId
			SET @CurrentIndex = @CurrentIndex + 1

		FETCH NEXT FROM TABLE_RECORD_CURSOR INTO @RecordId
	END
	CLOSE TABLE_RECORD_CURSOR
	DEALLOCATE TABLE_RECORD_CURSOR

    -- UPDATE STORE CONFIG
	IF EXISTS(SELECT 1 FROM [StoreConfig] WHERE StoreId = @StoreId)
		BEGIN
			UPDATE [StoreConfig] SET CurrentMaxOptionCode = @CurrentIndex WHERE StoreId = @StoreId
		END
	ELSE
		BEGIN
			INSERT [StoreConfig] (
			[Id],
			[StoreId],
			[CurrentMaxPurchaseOrderCode],
			[CurrentMaxOrderCode],
			[LastSavedUser],
			[LastSavedTime],
			[CreatedUser],
			[CreatedTime],
			[CurrentMaxMaterialCode],
			[CurrentMaxOptionCode],
			[CurrentMaxProductCategoryCode],
			[CurrentMaxToppingCode]
			) 
			VALUES (
			NEWID(),
			@StoreId,
			1,
			1,
			NULL,
			GETDATE(),
			NULL,
			GETDATE(),
			1,--CurrentMaxMaterialCode
			@CurrentIndex,--CurrentMaxOptionCode
			1,--CurrentMaxProductCategoryCode
			1--CurrentMaxToppingCode
			)
		END

	-- MOVE NEXT ELEMENT
    FETCH NEXT FROM STORE_CURSOR INTO @StoreId
END
CLOSE STORE_CURSOR
DEALLOCATE STORE_CURSOR