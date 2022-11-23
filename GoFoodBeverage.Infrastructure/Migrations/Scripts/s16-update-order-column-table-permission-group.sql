-- Update Fee => Fee & Tax
UPDATE [dbo].[PermissionGroup] SET [Name] = N'FEE & TAX' WHERE [Id] = '6C626154-5065-7265-6D69-73730000000A'

-- Update Tax permission to Fee & Tax permission group
UPDATE [dbo].[Permission] SET [PermissionGroupId] = '6C626154-5065-7265-6D69-73730000000A' 
WHERE PermissionGroupId = '6C626154-5065-7265-6D69-73730000000B'

-- Delete Tax permission group
DELETE FROM [dbo].[PermissionGroup] WHERE [Id] = '6C626154-5065-7265-6D69-73730000000B'

-- Update order column data
UPDATE [dbo].[PermissionGroup]  SET [Order] = 1 WHERE [Id] = '6C626154-5065-7265-6D69-737300000001' -- Product
UPDATE [dbo].[PermissionGroup]  SET [Order] = 2 WHERE [Id] = '6C626154-5065-7265-6D69-737300000002' -- Material
UPDATE [dbo].[PermissionGroup]  SET [Order] = 3 WHERE [Id] = '6C626154-5065-7265-6D69-737300000003' -- Category
UPDATE [dbo].[PermissionGroup]  SET [Order] = 4 WHERE [Id] = '6C626154-5065-7265-6D69-737300000004' -- Supplier
UPDATE [dbo].[PermissionGroup]  SET [Order] = 5 WHERE [Id] = '6C626154-5065-7265-6D69-737300000005' -- Purchase order
UPDATE [dbo].[PermissionGroup]  SET [Order] = 6 WHERE [Id] = '6C626154-5065-7265-6D69-737300000006' -- Transfer
UPDATE [dbo].[PermissionGroup]  SET [Order] = 7 WHERE [Id] = '6C626154-5065-7265-6D69-737300000007' -- Customer
UPDATE [dbo].[PermissionGroup]  SET [Order] = 8 WHERE [Id] = '6C626154-5065-7265-6D69-737300000008' -- Promotion
UPDATE [dbo].[PermissionGroup]  SET [Order] = 9 WHERE [Id] = '6C626154-5065-7265-6D69-737300000009' -- Area & Table
UPDATE [dbo].[PermissionGroup]  SET [Order] = 10 WHERE [Id] = '6C626154-5065-7265-6D69-73730000000A' -- Fee & Tax
UPDATE [dbo].[PermissionGroup]  SET [Order] = 11 WHERE [Id] = '6C626154-5065-7265-6D69-73730000000C' -- Shift
UPDATE [dbo].[PermissionGroup]  SET [Order] = 12 WHERE [Id] = '6C626154-5065-7265-6D69-73730000000D' -- Order
UPDATE [dbo].[PermissionGroup]  SET [Order] = 13 WHERE [Id] = '6C626154-5065-7265-6D69-737300000010' -- Marketing
UPDATE [dbo].[PermissionGroup]  SET [Order] = 14 WHERE [Id] = '6C626154-5065-7265-6D69-73730000000E' -- POS
UPDATE [dbo].[PermissionGroup]  SET [Order] = 15 WHERE [Id] = '6C626154-5065-7265-6D69-73730000000F' -- Online store 