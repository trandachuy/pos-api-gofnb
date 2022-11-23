--Table: Platform, Function, FunctionGroup, Package, Permission, PermissionGroup
GO
--Table: Platform > ProductPlatform, Order
ALTER TABLE [dbo].[Platform]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

UPDATE p
SET p.IdTemp = CONVERT(uniqueidentifier,CONVERT(VARBINARY(12), 'TablePlatform') + CONVERT(binary(4), p.Id))
FROM [dbo].[Platform] p
GO

ALTER TABLE [dbo].[Order]
DROP CONSTRAINT FK_Order_Platform_PlatformId;
GO
ALTER TABLE [dbo].[Order]
ADD PlatformIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[ProductPlatform]
DROP CONSTRAINT PK_ProductPlatform;
GO
ALTER TABLE [dbo].[ProductPlatform]
DROP CONSTRAINT FK_ProductPlatform_Platform_PlatformId;
GO
ALTER TABLE [dbo].[ProductPlatform]
ADD PlatformIdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
GO

UPDATE b
SET b.[PlatformIdTemp] = a.[IdTemp]
FROM 
[dbo].[Order] b
INNER JOIN [dbo].[Platform] a ON b.[PlatformId] = a.[Id]
GO
UPDATE b
SET b.[PlatformIdTemp] = a.[IdTemp]
FROM 
[dbo].[ProductPlatform] b
INNER JOIN [dbo].[Platform] a ON b.[PlatformId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Order_PlatformId'
    AND object_id = OBJECT_ID('[dbo].[Order]'))
BEGIN
    DROP INDEX [IX_Order_PlatformId] ON [dbo].[Order]
END
GO
ALTER TABLE [dbo].[Order]
DROP COLUMN [PlatformId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ProductPlatform_PlatformId'
    AND object_id = OBJECT_ID('[dbo].[ProductPlatform]'))
BEGIN
    DROP INDEX [IX_ProductPlatform_PlatformId] ON [dbo].[ProductPlatform]
END
GO
ALTER TABLE [dbo].[ProductPlatform]
DROP COLUMN [PlatformId]
GO

EXEC SP_RENAME '[dbo].[Order].[PlatformIdTemp]', 'PlatformId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[ProductPlatform].[PlatformIdTemp]', 'PlatformId', 'COLUMN'
GO

ALTER TABLE [dbo].[ProductPlatform]
ADD CONSTRAINT PK_ProductPlatform
PRIMARY KEY ([ProductId], [PlatformId]);
GO

ALTER TABLE [dbo].[Platform]
DROP CONSTRAINT [PK_Platform]
GO

ALTER TABLE [dbo].[Platform]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[Platform].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[Platform]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[Platform]
ADD CONSTRAINT PK_Platform PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[Order]
ADD CONSTRAINT FK_Order_Platform_PlatformId
FOREIGN KEY ([PlatformId]) REFERENCES [dbo].[Platform]([Id]);
GO
ALTER TABLE [dbo].[ProductPlatform]
ADD CONSTRAINT FK_ProductPlatform_Platform_PlatformId
FOREIGN KEY ([PlatformId]) REFERENCES [dbo].[Platform]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Order_PlatformId'
    AND object_id = OBJECT_ID('[dbo].[Order]'))
BEGIN
    CREATE INDEX [IX_Order_PlatformId] ON [dbo].[Order](PlatformId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ProductPlatform_PlatformId'
    AND object_id = OBJECT_ID('[dbo].[ProductPlatform]'))
BEGIN
    CREATE INDEX [IX_ProductPlatform_PlatformId] ON [dbo].[ProductPlatform](PlatformId)
END
GO

--Table: FunctionGroup > Function | FunctionPermission, PackageFunction
ALTER TABLE [dbo].[FunctionGroup]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO
ALTER TABLE [dbo].[Function]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[Function]
DROP CONSTRAINT FK_Function_FunctionGroup_FunctionGroupId;
GO
ALTER TABLE [dbo].[Function]
ADD FunctionGroupIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[FunctionPermission]
DROP CONSTRAINT PK_FunctionPermission;
GO
ALTER TABLE [dbo].[PackageFunction]
DROP CONSTRAINT PK_PackageFunction;
GO
ALTER TABLE [dbo].[FunctionPermission]
DROP CONSTRAINT FK_FunctionPermission_Function_FunctionId;
GO
ALTER TABLE [dbo].[PackageFunction]
DROP CONSTRAINT FK_PackageFunction_Function_FunctionId;
GO
ALTER TABLE [dbo].[FunctionPermission]
ADD FunctionIdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
GO
ALTER TABLE [dbo].[PackageFunction]
ADD FunctionIdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
GO

UPDATE b
SET b.[FunctionGroupIdTemp] = a.[IdTemp]
FROM 
[dbo].[Function] b
INNER JOIN [dbo].[FunctionGroup] a ON b.[FunctionGroupId] = a.[Id]
GO
UPDATE b
SET b.[FunctionIdTemp] = a.[IdTemp]
FROM 
[dbo].[FunctionPermission] b
INNER JOIN [dbo].[Function] a ON b.[FunctionId] = a.[Id]
GO
UPDATE b
SET b.[FunctionIdTemp] = a.[IdTemp]
FROM 
[dbo].[PackageFunction] b
INNER JOIN [dbo].[Function] a ON b.[FunctionId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Function_FunctionGroupId'
    AND object_id = OBJECT_ID('[dbo].[Function]'))
BEGIN
    DROP INDEX [IX_Function_FunctionGroupId] ON [dbo].[Function]
END
GO
ALTER TABLE [dbo].[Function]
DROP COLUMN [FunctionGroupId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_FunctionPermission_FunctionId'
    AND object_id = OBJECT_ID('[dbo].[FunctionPermission]'))
BEGIN
    DROP INDEX [IX_FunctionPermission_FunctionId] ON [dbo].[FunctionPermission]
END
GO
ALTER TABLE [dbo].[FunctionPermission]
DROP COLUMN [FunctionId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_PackageFunction_FunctionId'
    AND object_id = OBJECT_ID('[dbo].[PackageFunction]'))
BEGIN
    DROP INDEX [IX_PackageFunction_FunctionId] ON [dbo].[PackageFunction]
END
GO
ALTER TABLE [dbo].[PackageFunction]
DROP COLUMN [FunctionId]
GO

EXEC SP_RENAME '[dbo].[Function].[FunctionGroupIdTemp]', 'FunctionGroupId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[FunctionPermission].[FunctionIdTemp]', 'FunctionId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[PackageFunction].[FunctionIdTemp]', 'FunctionId', 'COLUMN'
GO

ALTER TABLE [dbo].[FunctionPermission]
ADD CONSTRAINT PK_FunctionPermission
PRIMARY KEY ([FunctionId], [PermissionId]);
GO

ALTER TABLE [dbo].[FunctionGroup]
DROP CONSTRAINT [PK_FunctionGroup]
GO
ALTER TABLE [dbo].[Function]
DROP CONSTRAINT [PK_Function]
GO

ALTER TABLE [dbo].[FunctionGroup]
DROP COLUMN [Id]
GO
ALTER TABLE [dbo].[Function]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[FunctionGroup].[IdTemp]', 'Id', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Function].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[FunctionGroup]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO
ALTER TABLE [dbo].[Function]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[FunctionGroup]
ADD CONSTRAINT PK_FunctionGroup PRIMARY KEY ([Id]);
GO
ALTER TABLE [dbo].[Function]
ADD CONSTRAINT PK_Function PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[Function]
ADD CONSTRAINT FK_Function_FunctionGroup_FunctionGroupId
FOREIGN KEY ([FunctionGroupId]) REFERENCES [dbo].[FunctionGroup]([Id]);
GO
ALTER TABLE [dbo].[FunctionPermission]
ADD CONSTRAINT FK_FunctionPermission_Function_FunctionId
FOREIGN KEY ([FunctionId]) REFERENCES [dbo].[Function]([Id]);
GO
ALTER TABLE [dbo].[PackageFunction]
ADD CONSTRAINT FK_PackageFunction_Function_FunctionId
FOREIGN KEY ([FunctionId]) REFERENCES [dbo].[Function]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Function_FunctionGroupId'
    AND object_id = OBJECT_ID('[dbo].[Function]'))
BEGIN
    CREATE INDEX [IX_Function_FunctionGroupId] ON [dbo].[Function](FunctionGroupId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_PackageFunction_FunctionId'
    AND object_id = OBJECT_ID('[dbo].[PackageFunction]'))
BEGIN
    CREATE INDEX [IX_PackageFunction_FunctionId] ON [dbo].[PackageFunction](FunctionId)
END
GO

--Table: PermissionGroup, Permission > Permission | GroupPermissionPermission
ALTER TABLE [dbo].[PermissionGroup]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO
ALTER TABLE [dbo].[Permission]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO
ALTER TABLE [dbo].[GroupPermissionPermission]
DROP CONSTRAINT PK_GroupPermissionPermission;
GO
ALTER TABLE [dbo].[GroupPermissionPermission]
DROP CONSTRAINT FK_GroupPermissionPermission_Permission_PermissionId;
GO
ALTER TABLE [dbo].[GroupPermissionPermission]
ADD PermissionIdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
GO

UPDATE p
SET p.IdTemp = CONVERT(uniqueidentifier,CONVERT(VARBINARY(12), 'TablePermissionGroup') + CONVERT(binary(4), p.Id))
FROM [dbo].[PermissionGroup] p
GO
UPDATE p
SET p.IdTemp = CONVERT(uniqueidentifier,CONVERT(VARBINARY(12), 'TablePermission') + CONVERT(binary(4), p.Id))
FROM [dbo].[Permission] p
GO
UPDATE b
SET b.[PermissionIdTemp] = a.[IdTemp]
FROM 
[dbo].[GroupPermissionPermission] b
INNER JOIN [dbo].[Permission] a ON b.[PermissionId] = a.[Id]
GO

ALTER TABLE [dbo].[Permission]
DROP CONSTRAINT FK_Permission_PermissionGroup_PermissionGroupId;
GO
ALTER TABLE [dbo].[Permission]
ADD PermissionGroupIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[FunctionPermission]
DROP CONSTRAINT PK_FunctionPermission;
GO
ALTER TABLE [dbo].[FunctionPermission]
DROP CONSTRAINT FK_FunctionPermission_Permission_PermissionId;
GO
ALTER TABLE [dbo].[FunctionPermission]
ADD PermissionIdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
GO
ALTER TABLE [dbo].[Permission]
DROP CONSTRAINT PK_Permission;
GO

UPDATE b
SET b.[PermissionGroupIdTemp] = a.[IdTemp]
FROM 
[dbo].[Permission] b
INNER JOIN [dbo].[PermissionGroup] a ON b.[PermissionGroupId] = a.[Id]
GO
UPDATE b
SET b.[PermissionIdTemp] = a.[IdTemp]
FROM 
[dbo].[FunctionPermission] b
INNER JOIN [dbo].[Permission] a ON b.[PermissionId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Permission_PermissionGroupId'
    AND object_id = OBJECT_ID('[dbo].[Permission]'))
BEGIN
    DROP INDEX [IX_Permission_PermissionGroupId] ON [dbo].[Permission]
END
GO
ALTER TABLE [dbo].[Permission]
DROP COLUMN [PermissionGroupId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_FunctionPermission_PermissionId'
    AND object_id = OBJECT_ID('[dbo].[FunctionPermission]'))
BEGIN
    DROP INDEX [IX_FunctionPermission_PermissionId] ON [dbo].[FunctionPermission]
END
GO
ALTER TABLE [dbo].[FunctionPermission]
DROP COLUMN [PermissionId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_GroupPermissionPermission_PermissionId'
    AND object_id = OBJECT_ID('[dbo].[GroupPermissionPermission]'))
BEGIN
    DROP INDEX [IX_GroupPermissionPermission_PermissionId] ON [dbo].[GroupPermissionPermission]
END
GO
ALTER TABLE [dbo].[GroupPermissionPermission]
DROP COLUMN [PermissionId]
GO

EXEC SP_RENAME '[dbo].[Permission].[PermissionGroupIdTemp]', 'PermissionGroupId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[FunctionPermission].[PermissionIdTemp]', 'PermissionId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[GroupPermissionPermission].[PermissionIdTemp]', 'PermissionId', 'COLUMN'
GO

ALTER TABLE [dbo].[FunctionPermission]
ADD CONSTRAINT PK_FunctionPermission
PRIMARY KEY ([FunctionId], [PermissionId]);
GO
ALTER TABLE [dbo].[GroupPermissionPermission]
ADD CONSTRAINT PK_GroupPermissionPermission
PRIMARY KEY ([GroupPermissionId], [PermissionId]);
GO

ALTER TABLE [dbo].[PermissionGroup]
DROP CONSTRAINT [PK_PermissionGroup]
GO

ALTER TABLE [dbo].[PermissionGroup]
DROP COLUMN [Id]
GO
ALTER TABLE [dbo].[Permission]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[PermissionGroup].[IdTemp]', 'Id', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Permission].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[PermissionGroup]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO
ALTER TABLE [dbo].[Permission]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[PermissionGroup]
ADD CONSTRAINT PK_PermissionGroup PRIMARY KEY ([Id]);
GO
ALTER TABLE [dbo].[Permission]
ADD CONSTRAINT PK_Permission PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[Permission]
ADD CONSTRAINT FK_Permission_PermissionGroup_PermissionGroupId
FOREIGN KEY ([PermissionGroupId]) REFERENCES [dbo].[PermissionGroup]([Id]);
GO
ALTER TABLE [dbo].[FunctionPermission]
ADD CONSTRAINT FK_FunctionPermission_Permission_PermissionId
FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission]([Id]);
GO
ALTER TABLE [dbo].[GroupPermissionPermission]
ADD CONSTRAINT FK_GroupPermissionPermission_Permission_PermissionId
FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Permission_PermissionGroupId'
    AND object_id = OBJECT_ID('[dbo].[Permission]'))
BEGIN
    CREATE INDEX [IX_Permission_PermissionGroupId] ON [dbo].[Permission](PermissionGroupId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_FunctionPermission_PermissionId'
    AND object_id = OBJECT_ID('[dbo].[FunctionPermission]'))
BEGIN
    CREATE INDEX [IX_FunctionPermission_PermissionId] ON [dbo].[FunctionPermission](PermissionId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_GroupPermissionPermission_PermissionId'
    AND object_id = OBJECT_ID('[dbo].[GroupPermissionPermission]'))
BEGIN
    CREATE INDEX [IX_GroupPermissionPermission_PermissionId] ON [dbo].[GroupPermissionPermission](PermissionId)
END
GO

--Table: Package > OrderPackage, PackageFunction
ALTER TABLE [dbo].[Package]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

UPDATE p
SET p.IdTemp = CONVERT(uniqueidentifier,CONVERT(VARBINARY(12), 'TablePackage') + CONVERT(binary(4), p.Id))
FROM [dbo].[Package] p
GO

ALTER TABLE [dbo].[OrderPackage]
DROP CONSTRAINT FK_OrderPackage_Package_PackageId;
GO
ALTER TABLE [dbo].[OrderPackage]
ADD PackageIdTemp UNIQUEIDENTIFIER
GO

ALTER TABLE [dbo].[PackageFunction]
DROP CONSTRAINT FK_PackageFunction_Package_PackageId;
GO
ALTER TABLE [dbo].[PackageFunction]
ADD PackageIdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
GO

UPDATE b
SET b.[PackageIdTemp] = a.[IdTemp]
FROM 
[dbo].[OrderPackage] b
INNER JOIN [dbo].[Package] a ON b.[PackageId] = a.[Id]
GO
UPDATE b
SET b.[PackageIdTemp] = a.[IdTemp]
FROM 
[dbo].[PackageFunction] b
INNER JOIN [dbo].[Package] a ON b.[PackageId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_OrderPackage_PackageId'
    AND object_id = OBJECT_ID('[dbo].[OrderPackage]'))
BEGIN
    DROP INDEX [IX_OrderPackage_PackageId] ON [dbo].[OrderPackage]
END
GO
ALTER TABLE [dbo].[OrderPackage]
DROP COLUMN [PackageId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_PackageFunction_PackageId'
    AND object_id = OBJECT_ID('[dbo].[PackageFunction]'))
BEGIN
    DROP INDEX [IX_PackageFunction_PackageId] ON [dbo].[PackageFunction]
END
GO
ALTER TABLE [dbo].[PackageFunction]
DROP COLUMN [PackageId]
GO

EXEC SP_RENAME '[dbo].[OrderPackage].[PackageIdTemp]', 'PackageId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[PackageFunction].[PackageIdTemp]', 'PackageId', 'COLUMN'
GO

ALTER TABLE [dbo].[Package]
DROP CONSTRAINT [PK_Package]
GO

ALTER TABLE [dbo].[Package]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[Package].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[Package]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[Package]
ADD CONSTRAINT PK_Package PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[OrderPackage]
ADD CONSTRAINT FK_OrderPackage_Package_PackageId
FOREIGN KEY ([PackageId]) REFERENCES [dbo].[Package]([Id]);
GO
ALTER TABLE [dbo].[PackageFunction]
ADD CONSTRAINT FK_PackageFunction_Package_PackageId
FOREIGN KEY ([PackageId]) REFERENCES [dbo].[Package]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_OrderPackage_PackageId'
    AND object_id = OBJECT_ID('[dbo].[OrderPackage]'))
BEGIN
    CREATE INDEX [IX_OrderPackage_PackageId] ON [dbo].[OrderPackage](PackageId)
END
GO

ALTER TABLE [dbo].[PackageFunction]
ADD CONSTRAINT PK_PackageFunction
PRIMARY KEY ([FunctionId], [PackageId]);
GO