-- Update function group name
UPDATE [dbo].[FunctionGroup] SET [Name] = N'Cửa hàng trực tuyến (Website và App cho cửa hàng)' WHERE [Name] LIKE N'Website bán hàng (Store web)'

-- Update order column data
UPDATE [dbo].[FunctionGroup]  SET [Order] = 1 WHERE [Name] LIKE N'Quản trị sản phẩm và nhà kho'
UPDATE [dbo].[FunctionGroup]  SET [Order] = 2 WHERE [Name] LIKE N'Quản trị cửa hàng'
UPDATE [dbo].[FunctionGroup]  SET [Order] = 3 WHERE [Name] LIKE N'Phân tích & Báo cáo dữ liệu kinh doanh'
UPDATE [dbo].[FunctionGroup]  SET [Order] = 4 WHERE [Name] LIKE N'Quản trị & Chăm sóc khách hàng'
UPDATE [dbo].[FunctionGroup]  SET [Order] = 5 WHERE [Name] LIKE N'Bán hàng tại quầy(POS)'
UPDATE [dbo].[FunctionGroup]  SET [Order] = 6 WHERE [Name] LIKE N'Cửa hàng trực tuyến (Website và App cho cửa hàng)'


