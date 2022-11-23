
DELETE [dbo].AccountTransfer

SET IDENTITY_INSERT [dbo].AccountTransfer ON;
GO
INSERT INTO [dbo].AccountTransfer(Id,AccountOwner,AccountNumber,BankName,Branch,Content) VALUES ('1',N'CÔNG TY TNHH MEDIASTEP SOFTWARE VIỆT NAM','04201015009138',N'Maritime Bank', N'Đô Thành', N'Mã đơn hàng- Số điện thoại của bạn');

SET IDENTITY_INSERT [dbo].AccountTransfer OFF;
GO