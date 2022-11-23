-- Check if existed permission group
IF NOT EXISTS (SELECT Id FROM PermissionGroup WHERE Id = '6C626154-5065-7265-6D69-737300000011') 
BEGIN 	-- Insert Online Store PermisssionGroup
	INSERT INTO [dbo].[PermissionGroup] (Id, Name)
	VALUES ('6C626154-5065-7265-6D69-737300000011','INVENTORY HISTORY')
END

-- Check if existed permission
IF NOT EXISTS (SELECT PermissionGroupId FROM Permission WHERE PermissionGroupId = '6C626154-5065-7265-6D69-737300000011') 
BEGIN	-- Insert Online Store Permission
	INSERT INTO [dbo].[Permission] (Id, Name, PermissionGroupId, Description)
	VALUES
		('6C626154-5065-7265-6D69-737300000065','View Inventory History', '6C626154-5065-7265-6D69-737300000011','')	
END

-- Get Id of Name=N'Marketing' in Function table
DECLARE @function_id UNIQUEIDENTIFIER;
SELECT @function_id = Id FROM [dbo].[Function] WHERE Name = N'Quản lý kho hàng'

-- Check if existed (FunctionId, PermissionId) in FunctionPermission table
IF NOT EXISTS (SELECT PermissionId FROM FunctionPermission WHERE PermissionId = '6C626154-5065-7265-6D69-737300000065')
BEGIN     -- View email campaign
    INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
    VALUES (@function_id,'6C626154-5065-7265-6D69-737300000065')
END
