--
-- Dumping data for table `Permission`
--
DELETE [dbo].[Permission]

SET IDENTITY_INSERT [dbo].[Permission] ON;
GO
INSERT INTO [dbo].[Permission] (Id, PermissionGroupId, Name, Description)
VALUES
(1,1,'View product', ''),
(2,1,'Create product', ''),
(3,1,'Edit product', ''),
(4,1,'Delete product', ''),
(5,1,'Activate product', ''),
(6,1,'Deactivate product', ''),
(7,1,'Import product', ''),
(8,1,'Export product', ''),
(9,1,'View option', ''),
(10,1,'Create option', ''),
(11,1,'Edit option', ''),
(12,1,'Delete option', ''),
(13,1,'View combo', ''),
(14,1,'Create combo', ''),
(15,1,'Edit combo', ''),
(16,1,'Delete combo', ''),

(17,2,'View material', ''),
(18,2,'Create material', ''),
(19,2,'Edit material', ''),
(20,2,'Delete material', ''),
(21,2,'Activate material', ''),
(22,2,'Deactivate material', ''),
(23,2,'Import material', ''),
(24,2,'Export material', ''),

(25,3,'View product category', ''),
(26,3,'Create product category', ''),
(27,3,'Edit product category', ''),
(28,3,'Delete product category', ''),
(29,3,'View material category', ''),
(30,3,'Create material category', ''),
(31,3,'Edit material category', ''),
(32,3,'Delete material category', ''),

(33,4,'View supplier', ''),
(34,4,'Create supplier', ''),
(35,4,'Edit supplier', ''),
(36,4,'Delete supplier', ''),

(37,5,'View purchase', ''),
(38,5,'Create purchase', ''),
(39,5,'Edit purchase', ''),
(40,5,'Cancel purchase', ''),
(41,5,'Approve purchase', ''),
(42,5,'Import goods', ''),

(43,6,'View transfer', ''),
(44,6,'Create transfer', ''),
(45,6,'Edit transfer', ''),
(46,6,'Cancel transfer', ''),
(47,6,'Ship goods', ''),
(48,6,'Receive goods', ''),

(49,7,'View customer', ''),
(50,7,'Create customer', ''),
(51,7,'Edit customer', ''),
(52,7,'Delete customer', ''),
(53,7,'View segment', ''),
(54,7,'Create segment', ''),
(55,7,'Edit segment', ''),
(56,7,'Delete segment', ''),

(57,8,'View promotion', ''),
(58,8,'Create promotion', ''),
(59,8,'Edit promotion', ''),
(60,8,'Delete promotion', ''),
(61,8,'Stop promotion', ''),

(62,9,'View area & table', ''),
(63,9,'Create area & table', ''),
(64,9,'Edit area & table', ''),
(65,9,'Delete area & table', ''),
(66,7,'View membership level:', ''),
(67,7,'Create membership level:', ''),
(68,7,'Edit membership level:', ''),
(69,7,'Delete membership level:', ''),

(70,10,'View fee', ''),
(71,10,'Create fee', ''),
(72,10,'Delete fee', ''),
(73,11,'View tax', ''),
(74,11,'Create tax', ''),
(75,11,'Delete tax', ''),
(76,12,'View shift', ''),
(77,13,'View order', ''),
(78,13,'Create order', ''),
(79,13,'Edit order', ''),
(80,13,'Delete order', ''),
(81,13,'Cancel order', ''),

(82,14,'Cashier', ''),
(83,14,'Service', ''),
(84,14,'Kitchen', ''),
(85,7,'View Loyalty Point', '')

SET IDENTITY_INSERT [dbo].[Permission] OFF;
GO
