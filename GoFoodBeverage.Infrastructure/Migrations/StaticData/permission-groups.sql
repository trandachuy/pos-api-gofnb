--
-- Dumping data for table `PermissionGroup`
--
DELETE [dbo].[PermissionGroup]

SET IDENTITY_INSERT [dbo].[PermissionGroup] ON;
GO
INSERT INTO [dbo].[PermissionGroup] (Id, Name)
VALUES
(1,'PRODUCT'),
(2,'MATERIAL'),
(3,'CATEGORY'),
(4,'SUPPLIER'),
(5,'PURCHASE ORDER'),
(6,'TRANSFER'),
(7,'CUSTOMER'),
(8,'PROMOTION'),
(9,'AREA & TABLE'),
(10,'FEE'),
(11,'TAX'),
(12,'SHIFT'),
(13,'ORDER'),
(14,'POS')

SET IDENTITY_INSERT [dbo].[PermissionGroup] OFF;
GO
