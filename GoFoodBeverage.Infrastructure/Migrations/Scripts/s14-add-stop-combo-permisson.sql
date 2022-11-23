IF NOT EXISTS ( SELECT 1 FROM [dbo].[Permission] WHERE Id = 87 AND PermissionGroupId = 1 )
BEGIN
SET IDENTITY_INSERT [dbo].[Permission] ON;
INSERT INTO [dbo].[Permission] (Id, PermissionGroupId, Name, Description)
VALUES
(87,1,'Stop Combo', '')

SET IDENTITY_INSERT [dbo].[Permission] OFF;
END

-- Dumping data for table `FunctionPermission`
--
IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 5 AND PermissionId = 87 )
BEGIN
INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
VALUES
(5, 87)-- Stop Combo
END