SET IDENTITY_INSERT [dbo].[Permission] ON;
GO

INSERT INTO [dbo].[Permission] (Id, PermissionGroupId, Name, Description)
VALUES
(86,10,'Stop fee', '')

SET IDENTITY_INSERT [dbo].[Permission] OFF;
GO

-- Dumping data for table `FunctionPermission`
--
INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
VALUES
(21, 86)-- Stop Fee
GO