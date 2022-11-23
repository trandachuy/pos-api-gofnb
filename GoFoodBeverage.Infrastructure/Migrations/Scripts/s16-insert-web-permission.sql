-- Check if existed permission group
IF NOT EXISTS (SELECT Id FROM PermissionGroup WHERE Id = '6C626154-5065-7265-6D69-73730000000F') 
BEGIN 	-- Insert Online Store PermisssionGroup
	INSERT INTO [dbo].[PermissionGroup] (Id, Name)
	VALUES ('6C626154-5065-7265-6D69-73730000000F','Online Store (Store Website & App)')
END

-- Check if existed permission
IF NOT EXISTS (SELECT PermissionGroupId FROM Permission WHERE PermissionGroupId = '6C626154-5065-7265-6D69-73730000000F') 
BEGIN	-- Insert Online Store Permission
	INSERT INTO [dbo].[Permission] (Id, Name, PermissionGroupId, Description)
	VALUES
		('6C626154-5065-7265-6D69-737300000058','View theme store', '6C626154-5065-7265-6D69-73730000000F',''),
		('6C626154-5065-7265-6D69-737300000059','Edit theme', '6C626154-5065-7265-6D69-73730000000F',''),
		('6C626154-5065-7265-6D69-73730000005A','Remove theme', '6C626154-5065-7265-6D69-73730000000F','')
END
