
--
-- Dumping data for table `[Package]`
--
DELETE[dbo].[Package]

SET IDENTITY_INSERT [dbo].[Package] ON;
GO
INSERT INTO [dbo].[Package] (Id, Name, CostPerMonth, IsActive, Tax)
VALUES
(1,'POS', '250000', 1, 10),
(2,'WEB','250000', 0, 10),
(3,'APP','250000', 0, 10)
SET IDENTITY_INSERT [dbo].[Package] OFF;
GO

--
-- Dumping data for table `[PackageDurationByMonth]`
--
DELETE[dbo].[PackageDurationByMonth]

SET IDENTITY_INSERT [dbo].[PackageDurationByMonth] ON;
GO
INSERT INTO [dbo].[PackageDurationByMonth] (Id, Period )
VALUES
(1,120),
(2,60),
(3,24),
(4,12)
SET IDENTITY_INSERT [dbo].[PackageDurationByMonth] OFF;
GO

--
-- Dumping data for table `[FunctionGroup]`
--
DELETE[dbo].[Function]
DELETE [dbo].[FunctionGroup]

SET IDENTITY_INSERT [dbo].[FunctionGroup] ON;
GO
INSERT INTO [dbo].[FunctionGroup] (Id, Name)
VALUES
(1,N'Quản trị sản phẩm và nhà kho'),
(2,N'Quản trị cửa hàng'),
(3,N'Quản trị & Chăm sóc khách hàng'),
(4,N'Phân tích & Báo cáo dữ liệu kinh doanh'),
(5,N'Bán hàng tại quầy(POS)')
SET IDENTITY_INSERT [dbo].[FunctionGroup] OFF;
GO

--
-- Dumping data for table `[Function]`
--
SET IDENTITY_INSERT [dbo].[Function] ON;
GO
INSERT INTO [dbo].[Function] (Id, FunctionGroupId, Name)
VALUES
(1,1,N'Quản lý danh sách sản phẩm'),
(2,1,N'Nhập sản phẩm'),
(3,1,N'Xuất danh sách sản phẩm'),
(4,1,N'Tạo & quản lý danh mục sản phẩm'),
(5,1,N'Tạo & quản lý combo'),
(6,1,N'Tạo & quản lý các tùy chọn món ăn'),
(7,1,N'Quản lý kho hàng'),
(8,1,N'Nhập nguyên vật liệu'),
(9,1,N'Xuất danh sách nguyên vật liệu'),
(10,1,N'Tạo & quản lý danh mục nguyên vật liệu'),
(11,1,N'In mã vạch nguyên vật liệu'),
(12,1,N'Tạo & quản lý nhà cung cấp'),
(13,1,N'Tạo & quản lý đơn nhập hàng'),

(14,2,N'Tạo & quản lý chi nhánh'),
(15,2,N'Quản lý nhóm tài khoản'),
(16,2,N'Quản lý nhân viên'),
(17,2,N'Cài đặt phương thức vận chuyển'),
(18,2,N'Cài đặt phương thức thanh toán'),
(19,2,N'Cài đặt hóa đơn/tem'),
(20,2,N'Tạo & quản lý điểm thành viên'),
(21,2,N'Tạo & quản lý phụ phí'),
(22,2,N'Tạo & quản lý thuế'),
(23,2,N'Tạo & quản lý chương trình khuyến mãi'),
(24,2,N'Quản lý địa hóa'),

(25,3,N'Tạo & quản lý khách hàng'),
(26,3,N'Tạo & quản lý phân nhóm khách hàng'),
(27,3,N'Tạo & quản lý cấp bậc thành viên'),
(28,3,N'Tạo & quản lý điểm thành viên'),
(29,3,N'Marketing'),

(30,4,N'Báo cáo ca làm việc'),
(31,4,N'Báo cáo đơn hàng'),
(32,4,N'Báo cáo doanh thu/ chi phí'),
(33,4,N'Tạo & quản lý điểm thành viên'),

(34,5,N'Báo cáo ca làm việc'),
(35,5,N'Báo cáo đơn hàng'),
(36,5,N'Báo cáo doanh thu/ chi phí'),
(37,5,N'Tạo & quản lý điểm thành viên')

SET IDENTITY_INSERT [dbo].[Function] OFF;
GO

--
-- Dumping data for table `[PackageFunction]`
-- POS - Functions
DELETE[dbo].[PackageFunction]
GO
INSERT INTO [dbo].[PackageFunction] (PackageId, FunctionId)
VALUES
(1,1),
(1,2),
(1,3),
(1,4),
(1,5),
(1,6),
(1,7),
(1,8),
(1,9),
(1,10),
(1,11),
(1,12),
(1,13),
(1,14),
(1,15),
(1,16),
(1,17),
(1,18),
(1,19),
(1,20),
(1,21),
(1,22),
(1,23),
(1,24),
(1,25),
(1,26),
(1,27),
(1,28),
(1,29),
(1,30),
(1,31),
(1,32),
(1,33),
(1,34),
(1,35),
(1,36),
(1,37)
GO
