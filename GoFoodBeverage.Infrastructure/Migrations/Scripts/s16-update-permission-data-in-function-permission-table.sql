-- Get Id of Name=N'Marketing' in Function table
DECLARE @function_id UNIQUEIDENTIFIER;
SELECT @function_id = Id FROM [dbo].[Function] WHERE Name = N'Marketing'

-- Check if existed (FunctionId, PermissionId) in FunctionPermission table
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000005B') 
BEGIN 	-- View email campaign
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000005B')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000005C') 
BEGIN 	-- Create email campaign
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000005C')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000005D') 
BEGIN 	-- Edit email campaign
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000005D')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000005E') 
BEGIN 	-- Delete email campaign
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000005E')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-73730000005F') 
BEGIN 	-- Stop email campaign
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-73730000005F')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000060') 
BEGIN 	-- View QR Code
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000060')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000061') 
BEGIN 	-- Create QR Code
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000061')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000062') 
BEGIN 	-- Edit QR Code
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000062')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000063') 
BEGIN 	-- Delete QR Code
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000063')
END

IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000064') 
BEGIN 	-- Stop QR Code
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (@function_id,'6C626154-5065-7265-6D69-737300000064')
END
GO