-- Check if existed permission
GO
IF NOT EXISTS (SELECT Id FROM Permission WHERE Id = '6C626154-5065-7265-6D69-737300000066') 
BEGIN	-- Insert Online Store Permission
	INSERT INTO [dbo].[Permission] (Id, PermissionGroupId, Name, Description)
	VALUES
		('6C626154-5065-7265-6D69-737300000066','6C626154-5065-7265-6D69-737300000007','View customer report', '')
END
GO