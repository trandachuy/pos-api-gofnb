
--
-- Dumping data for table `PackagePermissionGroup`
--
DELETE [dbo].[PackagePermissionMapping]

SET IDENTITY_INSERT [dbo].[PackagePermissionMapping] ON;
GO
INSERT INTO [dbo].[PackagePermissionMapping] (Id, PermissionId, PackageId)
VALUES
---------------- Product permission -----------------------

(1, 1, 1),-- View product
(2, 2, 1),-- Create product
(3, 3, 1),-- Edit product
(4, 4, 1),-- Delete product
(5, 5, 1),-- Activate product
(6, 6, 1),-- Deactivate product
(7, 7, 1),-- Import product
(8, 8, 1),-- Export product

-------------------- End Product permission ----------------

---------------- Material Permission ------------------

(9, 17, 1),-- View Material
(10, 18, 1),-- Create Material
(11, 19, 1),-- Edit Material
(12, 20, 1),-- Delete Material
(13, 21, 1),-- Activate Material
(14, 22, 1),-- Deactivate Material
(15, 23, 1),-- Import Material
(16, 24, 1),-- Export Material

-------------- End Material Permission ------------------

----------------- Category permission -------------------

(17, 25, 1),-- View product category
(18, 26, 1),-- Create product category
(19, 27, 1),-- Edit product category
(20, 28, 1),-- Delete product category

(21, 29, 1),-- Activate Material category
(22, 30, 1),-- Deactivate Material category
(23, 31, 1),-- Import Material category
(24, 32, 1),-- Export Material category

--------------- End Category permission ------------------

---------------- Supplier Permission ---------------------

(25, 33, 1),-- View supplier
(26, 34, 1),-- Create supplier
(27, 35, 1),-- Edit supplier
(28, 36, 1),-- Delete supplier

--------------- End Supplier Permission ------------------

-------------- Purchase Permission -----------------------

(29, 37, 1),-- View purchase
(30, 38, 1),-- Create purchase
(31, 39, 1),-- Edit purchase
(32, 40, 1),-- Cancel purchase
(33, 41, 1),-- Approve purchase

------------- End Purchase Permission -------------------

-------------- Transfer Permission -----------------------

(34, 43, 1),-- View transfer
(35, 44, 1),-- Create transfer
(36, 45, 1),-- Edit transfer
(37, 46, 1),-- Cancel transfer

------------- End Transfer Permission -------------------

-------------- CRM Permission -----------------------

(38, 49, 1),-- View customer
(39, 50, 1),-- Create customer
(40, 51, 1),-- Edit customer
(41, 52, 1),-- Delete customer
(42, 53, 1),-- Cancel customer
(43, 54, 1),-- Ship customer
(44, 55, 1),-- Receive customer

(45, 56, 1),-- Create segment
(46, 57, 1),-- Edit segment
(47, 58, 1),-- Delete segment

(48, 68, 1),-- View membership level
(49, 69, 1),-- Create membership level
(50, 70, 1),-- Edit membership level
(51, 71, 1),-- Delete membership level

------------- End CRM Permission -------------------

-------------- Promotion Permission -----------------------

(52, 59, 1),-- View promotion
(53, 60, 1),-- Create promotion
(54, 61, 1),-- Edit promotion
(55, 62, 1),-- Delete promotion
(56, 63, 1),-- Stop promotion

------------- End Promotion Permission -------------------

-------------- Area & Table Permission -----------------------

(57, 64, 1),-- View area & table
(58, 65, 1),-- Create area & table
(59, 66, 1),-- Edit area & table
(60, 67, 1),-- Delete area & table

------------- End Area & Table Permission -------------------

-------------- Fee Permission -----------------------

(61, 72, 1),-- View Fee
(62, 73, 1),-- Create Fee
(63, 74, 1),-- Delete Fee

------------- End Fee Permission -------------------

-------------- Tax Permission -----------------------

(64, 75, 1),-- View tax
(65, 76, 1),-- Create tax
(66, 77, 1),-- Delete tax

------------- End Tax Permission -------------------

-------------- Report Permission -----------------------

(67, 78, 1),-- View shift

------------- End Report Permission -------------------

-------------- POS Permission -----------------------

(68, 84, 1),-- Cashier
(69, 85, 1),-- Service
(70, 86, 1),-- Kitchen

------------- End POS Permission -------------------

-------------- More Permission -----------------------

(71, 9, 1),-- View option
(72, 10, 1),-- Create option
(73, 11, 1),-- Edit option
(74, 12, 1),-- Delete option

(75, 13, 1),-- View combo
(76, 14, 1),-- Create combo
(77, 15, 1),-- Edit combo
(78, 16, 1),-- Delete combo

(79, 42, 1),-- Import goods
(80, 47, 1),-- Ship goods
(81, 48, 1),-- Receive goods

(82, 79, 1),-- View order
(83, 80, 1),-- Create order
(84, 81, 1),-- Edit order
(85, 82, 1),-- Delete order
(86, 83, 1),-- Cancel order

------------- End More Permission -------------------


----------------------------------------------------------------------------------------------------------------------------


---------------- Product permission -----------------------

(87, 1, 2),-- View product
(88, 2, 2),-- Create product
(89, 3, 2),-- Edit product
(90, 4, 2),-- Delete product
(91, 5, 2),-- Activate product
(92, 6, 2),-- Deactivate product
(93, 7, 2),-- Import product
(94, 8, 2),-- Export product

-------------------- End Product permission ----------------

---------------- Material Permission ------------------

(95, 17, 2),-- View Material
(96, 18, 2),-- Create Material
(97, 19, 2),-- Edit Material
(98, 20, 2),-- Delete Material
(99, 21, 2),-- Activate Material
(100, 22, 2),-- Deactivate Material
(101, 23, 2),-- Import Material
(102, 24, 2),-- Export Material

-------------- End Material Permission ------------------

----------------- Category permission -------------------

(103, 25, 2),-- View product category
(104, 26, 2),-- Create product category
(105, 27, 2),-- Edit product category
(106, 28, 2),-- Delete product category

(107, 29, 2),-- Activate Material category
(108, 30, 2),-- Deactivate Material category
(109, 31, 2),-- Import Material category
(110, 32, 2),-- Export Material category

--------------- End Category permission ------------------

---------------- Supplier Permission ---------------------

(111, 33, 2),-- View supplier
(112, 34, 2),-- Create supplier
(113, 35, 2),-- Edit supplier
(114, 36, 2),-- Delete supplier

--------------- End Supplier Permission ------------------

-------------- Purchase Permission -----------------------

(115, 37, 2),-- View purchase
(116, 38, 2),-- Create purchase
(117, 39, 2),-- Edit purchase
(118, 40, 2),-- Cancel purchase
(119, 41, 2),-- Approve purchase

------------- End Purchase Permission -------------------

-------------- Transfer Permission -----------------------

(120, 43, 2),-- View transfer
(121, 44, 2),-- Create transfer
(122, 45, 2),-- Edit transfer
(123, 46, 2),-- Cancel transfer

------------- End Transfer Permission -------------------

-------------- CRM Permission -----------------------

(124, 49, 2),-- View customer
(125, 50, 2),-- Create customer
(126, 51, 2),-- Edit customer
(127, 52, 2),-- Delete customer
(128, 53, 2),-- Cancel customer
(129, 54, 2),-- Ship customer
(130, 55, 2),-- Receive customer

(131, 56, 2),-- Create segment
(132, 57, 2),-- Edit segment
(133, 58, 2),-- Delete segment

(134, 68, 2),-- View membership level
(135, 69, 2),-- Create membership level
(136, 70, 2),-- Edit membership level
(137, 71, 2),-- Delete membership level

------------- End CRM Permission -------------------

-------------- Promotion Permission -----------------------

(138, 59, 2),-- View promotion
(139, 60, 2),-- Create promotion
(140, 61, 2),-- Edit promotion
(141, 62, 2),-- Delete promotion
(142, 63, 2),-- Stop promotion

------------- End Promotion Permission -------------------

-------------- Area & Table Permission -----------------------

(143, 64, 2),-- View area & table
(144, 65, 2),-- Create area & table
(145, 66, 2),-- Edit area & table
(146, 67, 2),-- Delete area & table

------------- End Area & Table Permission -------------------

-------------- Fee Permission -----------------------

(147, 72, 2),-- View Fee
(148, 73, 2),-- Create Fee
(149, 74, 2),-- Delete Fee

------------- End Fee Permission -------------------

-------------- Tax Permission -----------------------

(150, 75, 2),-- View tax
(151, 76, 2),-- Create tax
(152, 77, 2),-- Delete tax

------------- End Tax Permission -------------------

-------------- Report Permission -----------------------

(153, 78, 2),-- View shift

------------- End Report Permission -------------------

-------------- More Permission -----------------------

(154, 9, 2),-- View option
(155, 10, 2),-- Create option
(156, 11, 2),-- Edit option
(157, 12, 2),-- Delete option

(158, 13, 2),-- View combo
(159, 14, 2),-- Create combo
(160, 15, 2),-- Edit combo
(161, 16, 2),-- Delete combo

(162, 42, 2),-- Import goods
(163, 47, 2),-- Ship goods
(164, 48, 2),-- Receive goods

(165, 79, 2),-- View order
(166, 80, 2),-- Create order
(167, 81, 2),-- Edit order
(168, 82, 2),-- Delete order
(169, 83, 2)-- Cancel order

------------- End More Permission -------------------
--- Continue Id = 171

SET IDENTITY_INSERT [dbo].[PackagePermissionMapping] OFF;
GO