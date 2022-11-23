
--- Check exist IsAllMatch column
IF NOT EXISTS( SELECT NULL
            FROM INFORMATION_SCHEMA.COLUMNS
           WHERE table_name = 'CustomerSegment'
             AND table_schema = 'dbo'
             AND column_name = 'IsAllMatch')
BEGIN
  ALTER TABLE dbo.CustomerSegment ADD IsAllMatch BIT NOT NULL DEFAULT 0;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CalculateCustomersBySegment')
DROP PROC [dbo].[SP_CalculateCustomersBySegment]
GO

CREATE PROC [dbo].[SP_CalculateCustomersBySegment]
	@StoreId VARCHAR(50),
	@CustomerSegmentId VARCHAR(50)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX) = N'SELECT * FROM Customer c WHERE c.StoreId = ''' + @StoreId + ''' AND (';
	DECLARE @IsAllMatch BIT;
	DECLARE @MatchStr NVARCHAR(20) = ''; -- Match condition string
	DECLARE @CountAll INT = 1; -- Count all conditions

	SELECT @IsAllMatch = s.IsAllMatch FROM CustomerSegment s WHERE s.StoreId = @StoreId AND s.Id = @CustomerSegmentId; 

	-- Set match condition string
    BEGIN
		 IF @IsAllMatch = 1
			SET @MatchStr = ' AND ';
	     ELSE IF @IsAllMatch = 0
			SET @MatchStr = ' OR ';
    END;

	-- Set query get customer data
		-- Registration date on
		DECLARE @InitCountRegistrationOn INT = 1; 
		DECLARE @CountRegistrationOn INT = 0; 
		SELECT @CountRegistrationOn = COUNT(*) FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
			AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 1 AND sc.RegistrationDateConditionId = 1 AND sc.RegistrationTime IS NOT NULL

		IF (@CountRegistrationOn > 0) -- RegistrationDate -> On
		BEGIN
			DECLARE @TblRegistrationOn TABLE(RegistrationTime DATETIME);
			INSERT INTO @TblRegistrationOn
			SELECT RegistrationTime FROM
			(
				SELECT sc.RegistrationTime FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
					AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 1 AND sc.RegistrationDateConditionId = 1 AND sc.RegistrationTime IS NOT NULL
			) tmp
			
			WHILE (@InitCountRegistrationOn <= @CountRegistrationOn) 
			BEGIN
				SET @SQL = CONCAT(@SQL, CASE WHEN @CountAll > 1 THEN @MatchStr ELSE '' END,' CAST(c.CreatedTime as DATE) = ''' , 
					CAST((SELECT t.RegistrationTime FROM @TblRegistrationOn t ORDER BY t.RegistrationTime
						OFFSET (@InitCountRegistrationOn - 1) ROWS FETCH NEXT 1 ROWS ONLY) AS date), '''');

				SET @InitCountRegistrationOn = @InitCountRegistrationOn + 1;
				SET @CountAll = @CountAll + 1;
			END;
			DELETE @TblRegistrationOn
		END;

		-- Registration date before
		DECLARE @InitCountRegistrationBefore INT = 1; 
		DECLARE @CountRegistrationBefore INT = 0; 
		SELECT @CountRegistrationBefore = COUNT(*) FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
			AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 1 AND sc.RegistrationDateConditionId = 2 AND sc.RegistrationTime IS NOT NULL

		IF (@CountRegistrationBefore > 0) -- RegistrationDate -> On
		BEGIN
			DECLARE @TblRegistrationBefore TABLE(RegistrationTime DATETIME);
			INSERT INTO @TblRegistrationBefore
			SELECT RegistrationTime FROM
			(
				SELECT sc.RegistrationTime FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
					AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 1 AND sc.RegistrationDateConditionId = 2 AND sc.RegistrationTime IS NOT NULL
			) tmp
			
			WHILE (@InitCountRegistrationBefore <= @CountRegistrationBefore) 
			BEGIN
				SET @SQL = CONCAT(@SQL, CASE WHEN @CountAll > 1 THEN @MatchStr ELSE '' END,' CAST(c.CreatedTime as DATE) < ''' , 
					CAST((SELECT t.RegistrationTime FROM @TblRegistrationBefore t ORDER BY t.RegistrationTime
						OFFSET (@InitCountRegistrationBefore - 1) ROWS FETCH NEXT 1 ROWS ONLY) AS date), '''');

				SET @InitCountRegistrationBefore = @InitCountRegistrationBefore + 1;
				SET @CountAll = @CountAll + 1;
			END;
			DELETE @TblRegistrationBefore
		END;

		-- Registration date after
		DECLARE @InitCountRegistrationAfter INT = 1; 
		DECLARE @CountRegistrationAfter INT = 0; 
		SELECT @CountRegistrationAfter = COUNT(*) FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
			AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 1 AND sc.RegistrationDateConditionId = 3 AND sc.RegistrationTime IS NOT NULL

		IF (@CountRegistrationAfter > 0) -- RegistrationDate -> On
		BEGIN
			DECLARE @TblRegistrationAfter TABLE(RegistrationTime DATETIME);
			INSERT INTO @TblRegistrationAfter
			SELECT RegistrationTime FROM
			(
				SELECT sc.RegistrationTime FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
					AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 1 AND sc.RegistrationDateConditionId = 3 AND sc.RegistrationTime IS NOT NULL
			) tmp
			
			WHILE (@InitCountRegistrationAfter <= @CountRegistrationAfter) 
			BEGIN
				SET @SQL = CONCAT(@SQL, CASE WHEN @CountAll > 1 THEN @MatchStr ELSE '' END,' CAST(c.CreatedTime as DATE) > ''' , 
					CAST((SELECT t.RegistrationTime FROM @TblRegistrationAfter t ORDER BY t.RegistrationTime
						OFFSET (@InitCountRegistrationAfter - 1) ROWS FETCH NEXT 1 ROWS ONLY) AS date), '''');

				SET @InitCountRegistrationAfter = @InitCountRegistrationAfter + 1;
				SET @CountAll = @CountAll + 1;
			END;
			DELETE @TblRegistrationAfter
		END;
		
		-- Birthday
		DECLARE @InitCountBirthday INT = 1; 
		DECLARE @CountBirthday INT; 
		SELECT @CountBirthday = COUNT(*) FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
			AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 2 AND sc.Birthday IS NOT NULL

		IF (@CountBirthday > 0) -- Birthday
		BEGIN
			DECLARE @TblBirthday TABLE(Birthday INT);
			INSERT INTO @TblBirthday
			SELECT Birthday FROM
			(
				SELECT sc.Birthday FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
					AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 2 AND sc.Birthday IS NOT NULL
			) tmp
		

			WHILE (@InitCountBirthday <= @CountBirthday)
			BEGIN
				SET @SQL = CONCAT(@SQL, CASE WHEN @CountAll > 1 THEN @MatchStr ELSE '' END,' MONTH(c.Birthday) = ' , 
					(SELECT t.Birthday FROM @TblBirthday t ORDER BY t.Birthday 
						OFFSET (@InitCountBirthday - 1) ROWS FETCH NEXT 1 ROWS ONLY), '');

				SET @InitCountBirthday = @InitCountBirthday + 1;
				SET @CountAll = @CountAll + 1;
			END;
			DELETE @TblBirthday
		END;

		-- Gender
		DECLARE @InitCountGender INT = 1; 
		DECLARE @CountGender INT; 
		SELECT @CountGender = COUNT(*) FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
			AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 3 AND sc.IsMale IS NOT NULL

		IF (@CountGender > 0) -- Gender
		BEGIN
			DECLARE @TblGender TABLE(IsMale BIT);
			INSERT INTO @TblGender
			SELECT IsMale FROM
			(
				SELECT sc.IsMale FROM CustomerSegmentCondition sc WHERE sc.CustomerSegmentId = @CustomerSegmentId
					AND sc.ObjectiveId = 1 AND sc.CustomerDataId = 3 AND sc.IsMale IS NOT NULL
			) tmp
		

			WHILE (@InitCountGender <= @CountGender)
			BEGIN
				SET @SQL = CONCAT(@SQL, CASE WHEN @CountAll > 1 THEN @MatchStr ELSE '' END,' c.Gender = ' , 
					(SELECT t.IsMale FROM @TblGender t ORDER BY t.IsMale 
						OFFSET (@InitCountGender - 1) ROWS FETCH NEXT 1 ROWS ONLY), '');

				SET @InitCountGender = @InitCountGender + 1;
				SET @CountAll = @CountAll + 1;
			END;
			DELETE @TblGender
		END;

	SET @SQL = CONCAT(@SQL, ' )');
	--SELECT @SQL as sql_query
	EXEC sp_executesql @SQL;
END;
GO