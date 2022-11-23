DECLARE @TblOldFunction TABLE(Id UNIQUEIDENTIFIER, Name NVARCHAR(MAX));
				INSERT INTO @TblOldFunction 
					SELECT f.Id, f.Name FROM [Function] f 
					INNER JOIN [FunctionGroup] fg ON fg.Id = f.FunctionGroupId
					WHERE fg.[Name] LIKE N'Bán hàng tại quầy(POS)'

DELETE FROM [PackageFunction] WHERE FunctionId IN (SELECT o.Id FROM @TblOldFunction o) -- Delete old PackageFunction

DELETE FROM [FunctionPermission] WHERE FunctionId IN (SELECT o.Id FROM @TblOldFunction o) -- Delete old FunctionPermission

DELETE FROM [Function] WHERE Id IN (SELECT o.Id FROM @TblOldFunction o) -- Delete old Function

-- Insert new POS Function
	DECLARE @FunctionGroupId UNIQUEIDENTIFIER;
	SELECT @FunctionGroupId = fg.Id FROM [FunctionGroup] fg WHERE Name LIKE N'Bán hàng tại quầy(POS)'

	INSERT INTO [dbo].[Function] (Id, Name, FunctionGroupId)
	VALUES
		(NEWID(), N'Quản lý ca làm việc', @FunctionGroupId),
		(NEWID(), N'Quản lý đơn hàng', @FunctionGroupId),
		(NEWID(), N'Quản lý khu vực/ bàn ăn', @FunctionGroupId),
		(NEWID(), N'Kiểm kê hàng tồn kho mỗi ca', @FunctionGroupId),
		(NEWID(), N'Tạo & quản lý điểm thành viên', @FunctionGroupId),
		(NEWID(), N'Tạo đơn hàng tại bàn', @FunctionGroupId),
		(NEWID(), N'Tạo đơn hàng mang đi', @FunctionGroupId),
		(NEWID(), N'Chỉnh sửa và thanh toán đơn hàng', @FunctionGroupId),
		(NEWID(), N'Quản lý bill in, tem in', @FunctionGroupId),
		(NEWID(), N'Quản lý nhà bếp', @FunctionGroupId)

-- Insert new POS PackageFunction
	DECLARE @PackagePOSId UNIQUEIDENTIFIER = '6C626154-5065-6361-6B61-676500000001';

	DECLARE @id1 UNIQUEIDENTIFIER;
	SELECT @id1 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý ca làm việc'

	DECLARE @id2 UNIQUEIDENTIFIER;
	SELECT @id2 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý đơn hàng'

	DECLARE @id3 UNIQUEIDENTIFIER;
	SELECT @id3 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý khu vực/ bàn ăn'

	DECLARE @id4 UNIQUEIDENTIFIER;
	SELECT @id4 = f.Id FROM [Function] f WHERE Name LIKE N'Kiểm kê hàng tồn kho mỗi ca'

	DECLARE @id5 UNIQUEIDENTIFIER;
	SELECT @id5 = f.Id FROM [Function] f  
		INNER JOIN [FunctionGroup] fg ON fg.Id = f.FunctionGroupId
		WHERE f.[Name] LIKE N'Tạo & quản lý điểm thành viên' AND fg.[Name] LIKE N'Bán hàng tại quầy(POS)' 

	DECLARE @id6 UNIQUEIDENTIFIER;
	SELECT @id6 = f.Id FROM [Function] f WHERE Name LIKE N'Tạo đơn hàng tại bàn'
	
	DECLARE @id7 UNIQUEIDENTIFIER;
	SELECT @id7 = f.Id FROM [Function] f WHERE Name LIKE N'Tạo đơn hàng mang đi'

	DECLARE @id8 UNIQUEIDENTIFIER;
	SELECT @id8 = f.Id FROM [Function] f WHERE Name LIKE N'Chỉnh sửa và thanh toán đơn hàng'

	DECLARE @id9 UNIQUEIDENTIFIER;
	SELECT @id9 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý bill in, tem in'

	DECLARE @id10 UNIQUEIDENTIFIER;
	SELECT @id10 = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý nhà bếp'

	INSERT INTO [dbo].[PackageFunction] (PackageId, FunctionId)
	VALUES 
		(@PackagePOSId, @id1), 
		(@PackagePOSId, @id2), 
		(@PackagePOSId, @id3),
		(@PackagePOSId, @id4), 
		(@PackagePOSId, @id5),
		(@PackagePOSId, @id6),
		(@PackagePOSId, @id7),
		(@PackagePOSId, @id8), 
		(@PackagePOSId, @id9), 
		(@PackagePOSId, @id10)

-- Check if existed Function Permission
IF NOT EXISTS (SELECT FunctionId FROM [FunctionPermission] fp
				INNER JOIN [Function] f ON f.Id = fp.FunctionId
				WHERE f.Name LIKE N'Quản lý nhà bếp') 
BEGIN	-- Insert Online Function Permission
	DECLARE @FunctionId UNIQUEIDENTIFIER;
	SELECT @FunctionId = f.Id FROM [Function] f WHERE Name LIKE N'Quản lý nhà bếp'

	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES
			(@FunctionId, '6C626154-5065-7265-6D69-737300000052'),
			(@FunctionId, '6C626154-5065-7265-6D69-737300000053'),
			(@FunctionId, '6C626154-5065-7265-6D69-737300000054')
END