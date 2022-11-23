 -- Active WEB package
UPDATE [dbo].[Package] SET IsActive = 1, CostPerMonth = 499000 WHERE Id = '6C626154-5065-6361-6B61-676500000002'

-- Check if existed WEB function group
IF NOT EXISTS (SELECT Id FROM [FunctionGroup] WHERE Name LIKE N'Website bán hàng (Store web)') 
BEGIN 	-- Insert Online Store function group
	INSERT INTO [dbo].[FunctionGroup] (Id, Name)
	VALUES (NEWID(), 'Website bán hàng (Store web)')
END

-- Check if existed WEB function
IF NOT EXISTS (SELECT FunctionGroupId FROM [Function] f
	INNER JOIN [FunctionGroup] fg ON fg.Id = f.FunctionGroupId
	WHERE fg.Name LIKE N'Website bán hàng (Store web)'
) 
BEGIN	-- Insert Online Store Permission

	DECLARE @FunctionGroupId UNIQUEIDENTIFIER;
	SELECT @FunctionGroupId = fg.Id FROM [FunctionGroup] fg WHERE Name LIKE N'Website bán hàng (Store web)'

	INSERT INTO [dbo].[Function] (Id, Name, FunctionGroupId)
	VALUES
		(NEWID(), N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)', @FunctionGroupId),
		(NEWID(), N'Quản lý thông tin người dùng', @FunctionGroupId),
		(NEWID(), N'Tạo đơn hàng trực tuyến', @FunctionGroupId),
		(NEWID(), N'Quản lý đơn hàng trực tuyến', @FunctionGroupId),
		(NEWID(), N'Đánh giá và xếp hạng', @FunctionGroupId),
		(NEWID(), N'Quản lý đánh giá và xếp hạng', @FunctionGroupId)
END

-- Check if existed WEB package function
IF NOT EXISTS (SELECT PackageId FROM [PackageFunction] WHERE PackageId = '6C626154-5065-6361-6B61-676500000002') 
BEGIN 	-- Insert Online Store package function
	DECLARE @id1 UNIQUEIDENTIFIER;
	SELECT @id1 = f.Id FROM [Function] f WHERE Name LIKE N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)'

	DECLARE @id2 UNIQUEIDENTIFIER;
	SELECT @id2 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý thông tin người dùng'

	DECLARE @id3 UNIQUEIDENTIFIER;
	SELECT @id3 = f.Id FROM [Function] f WHERE Name LIKE N'Tạo đơn hàng trực tuyến'

	DECLARE @id4 UNIQUEIDENTIFIER;
	SELECT @id4 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý đơn hàng trực tuyến'

	DECLARE @id5 UNIQUEIDENTIFIER;
	SELECT @id5 = f.Id FROM [Function] f WHERE Name LIKE N'Đánh giá và xếp hạng'

	DECLARE @id6 UNIQUEIDENTIFIER;
	SELECT @id6 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý đánh giá và xếp hạng'


	INSERT INTO [dbo].[PackageFunction] (PackageId, FunctionId)
	VALUES ('6C626154-5065-6361-6B61-676500000002', @id1),
		('6C626154-5065-6361-6B61-676500000002', @id2),
		('6C626154-5065-6361-6B61-676500000002', @id3),
		('6C626154-5065-6361-6B61-676500000002', @id4),
		('6C626154-5065-6361-6B61-676500000002', @id5),
		('6C626154-5065-6361-6B61-676500000002', @id6)

	-- Insert package function from other tables
	DECLARE @PackageWebId UNIQUEIDENTIFIER = '6C626154-5065-6361-6B61-676500000002';
	DECLARE @FunctionGroupName NVARCHAR(MAX)
	DECLARE @TblFunctionGroup TABLE(Name NVARCHAR(MAX));
				INSERT INTO @TblFunctionGroup 
					VALUES	
					(N'Quản trị sản phẩm và nhà kho'), 
					(N'Quản trị cửa hàng'), 
					(N'Quản trị & Chăm sóc khách hàng'), 
					(N'Phân tích & Báo cáo dữ liệu kinh doanh')

	DECLARE FUNCTION_GROUP_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR

	SELECT g.Name FROM @TblFunctionGroup g
		OPEN FUNCTION_GROUP_CURSOR
		FETCH NEXT FROM FUNCTION_GROUP_CURSOR INTO @FunctionGroupName
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		INSERT INTO [dbo].[PackageFunction] (PackageId, FunctionId) 
				SELECT @PackageWebId, f.Id
				FROM [dbo].[Function] f
				INNER JOIN [dbo].[FunctionGroup] fg ON fg.Id = f.FunctionGroupId
				WHERE fg.Name LIKE @FunctionGroupName
		-- MOVE NEXT ELEMENT
		FETCH NEXT FROM FUNCTION_GROUP_CURSOR INTO @FunctionGroupName
	END
	CLOSE FUNCTION_GROUP_CURSOR
	DEALLOCATE FUNCTION_GROUP_CURSOR
END

-- Check if existed Function Permission
IF NOT EXISTS (SELECT FunctionId FROM [FunctionPermission] fp
				INNER JOIN [Function] f ON f.Id = fp.FunctionId
				WHERE f.Name LIKE N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)') 
BEGIN	-- Insert Online Function Permission
	DECLARE @FunctionId UNIQUEIDENTIFIER;
	SELECT @FunctionId = f.Id FROM [Function] f WHERE Name LIKE N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)'

	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES
			(@FunctionId, '6C626154-5065-7265-6D69-737300000058'),
			(@FunctionId, '6C626154-5065-7265-6D69-737300000059'),
			(@FunctionId, '6C626154-5065-7265-6D69-73730000005A')
END