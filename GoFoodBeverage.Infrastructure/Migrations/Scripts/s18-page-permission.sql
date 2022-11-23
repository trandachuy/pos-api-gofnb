

-- Check if existed permission
IF NOT EXISTS (SELECT PermissionGroupId FROM Permission WHERE Id = '6C626154-5065-7265-6D69-737300000067') 
BEGIN	-- Insert Online Store Permission
	INSERT INTO [dbo].[Permission] (Id, PermissionGroupId, Name, Description)
	VALUES
		('6C626154-5065-7265-6D69-737300000067','6C626154-5065-7265-6D69-73730000000F','View page', ''),
		('6C626154-5065-7265-6D69-737300000068','6C626154-5065-7265-6D69-73730000000F','Create page', ''),
		('6C626154-5065-7265-6D69-737300000069','6C626154-5065-7265-6D69-73730000000F','Edit page', ''),
		('6C626154-5065-7265-6D69-73730000006A','6C626154-5065-7265-6D69-73730000000F','Delete page', '')
END
GO
-- Get Id of Name=N'Online Store' in Function table
DECLARE @function_id UNIQUEIDENTIFIER;
SELECT @function_id = Id FROM [dbo].[Function] WHERE Name = N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)'

-- Check if existed (FunctionId, PermissionId) in FunctionPermission table
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000067') 
BEGIN 	-- View page
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000067')
END
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000068') 
BEGIN 	-- Create page
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000068')
END
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000069') 
BEGIN 	-- Edit page
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000069')
END
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000006A') 
BEGIN 	-- Delete page
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000006A')
END