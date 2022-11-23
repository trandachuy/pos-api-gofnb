-- FIX BUG 2806: [Permission] Have Fee and Tax permission but can't see that menu
-- LINK: https://dev.azure.com/stepmedia/Go%20Food%20and%20Beverage/_workitems/edit/2806

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 21 AND PermissionId = 70 )
BEGIN
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (21, 70)-- Tạo & quản lý phụ phí - VIEW_FEE
END

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 21 AND PermissionId = 71 )
BEGIN
	INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
	VALUES (21, 71)-- Tạo & quản lý phụ phí - CREATE FEE
END

--- DELETE WRONG PERMISSIONS OF FUNCTION
DELETE [dbo].[FunctionPermission] WHERE FunctionId = 22 AND PermissionId = 76
DELETE [dbo].[FunctionPermission] WHERE FunctionId = 22 AND PermissionId = 77

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 21 AND PermissionId = 73 )
BEGIN
	UPDATE [dbo].[FunctionPermission]
	SET FunctionId = 22 -- Tạo & quản lý thuế
	WHERE PermissionId = 73 -- VIEW TAX
END

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 22 AND PermissionId = 74 )
BEGIN
	UPDATE [dbo].[FunctionPermission]
	SET FunctionId = 22 -- Tạo & quản lý thuế
	WHERE PermissionId = 74 -- CREATE TAX
END

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 30 AND PermissionId = 76 )
BEGIN
	UPDATE [dbo].[FunctionPermission]
	SET FunctionId = 30 -- Báo cáo ca làm việc
	WHERE PermissionId = 76 -- VIEW SHIFT
END

IF NOT EXISTS ( SELECT 1 FROM [dbo].[FunctionPermission] WHERE FunctionId = 35 AND PermissionId = 77 )
BEGIN
	UPDATE [dbo].[FunctionPermission]
	SET FunctionId = 35 -- Báo cáo đơn hàng
	WHERE PermissionId = 77 -- VIEW ORDER
END

