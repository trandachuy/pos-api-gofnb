-- FIX BUG 2807: [Permission] Full Promotion permission but always display Edit promotion page
-- LINK: https://dev.azure.com/stepmedia/Go%20Food%20and%20Beverage/_workitems/edit/2807

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 23 AND PermissionId = 57 )
BEGIN
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (23, 57)-- PROMOTION - VIEW_PROMOTION
END

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 23 AND PermissionId = 58 )
BEGIN
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (23, 58)-- PROMOTION - CREATE_PROMOTION
END

