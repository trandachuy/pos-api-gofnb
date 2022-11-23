
-- Check if existed permission
IF NOT EXISTS (SELECT PermissionGroupId FROM Permission WHERE Id = '6C626154-5065-7265-6D69-73730000006B') 
BEGIN	-- Insert Online Store Permission
	INSERT INTO [dbo].[Permission] (Id, PermissionGroupId, Name, Description)
	VALUES
		('6C626154-5065-7265-6D69-73730000006B','6C626154-5065-7265-6D69-73730000000F','View Menu Management', ''),
		('6C626154-5065-7265-6D69-73730000006C','6C626154-5065-7265-6D69-73730000000F','Create Menu Management', ''),
		('6C626154-5065-7265-6D69-73730000006D','6C626154-5065-7265-6D69-73730000000F','Edit Menu Management', ''),
		('6C626154-5065-7265-6D69-73730000006E','6C626154-5065-7265-6D69-73730000000F','Delete Menu Management', '')

END
GO

-- Get Id of Name=N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)' in Function table
DECLARE @function_id UNIQUEIDENTIFIER;
SELECT @function_id = Id FROM [dbo].[Function] WHERE Name = N'Trang web Thực phẩm & Đồ uống (Cửa hàng trực tuyến)'

-- Check if existed (FunctionId, PermissionId) in FunctionPermission table
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000006B') 
BEGIN 	
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000006B')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000006C') 
BEGIN 	
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000006C')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000006D') 
BEGIN 	
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000006D')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000006E') 
BEGIN 	
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000006E')
END