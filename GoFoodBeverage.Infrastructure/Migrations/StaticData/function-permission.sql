--
-- Dumping data for table `FunctionPermission`
--
DELETE [dbo].[FunctionPermission]
GO

INSERT INTO [dbo].[FunctionPermission] (FunctionId, PermissionId)
VALUES
(1, 1),-- View product

(2, 2),-- Create product
(2, 3),-- Edit product
(2, 4),-- Delete product
(2, 5),-- Activate product
(2, 6),-- Deactivate product
(2, 7),-- Import product

(3, 8),-- Export product

(4, 25),-- View product category
(4, 26),-- Create product category
(4, 27),-- Edit product category
(4, 28),-- Delete product category

(5, 13),-- View combo
(5, 14),-- Create combo
(5, 15),-- Edit combo
(5, 16),-- Delete combo

(6, 9),-- View option
(6, 10),-- Create option
(6, 11),-- Edit option
(6, 12),-- Delete option

(8, 17),-- View Material
(8, 18),-- Create Material
(8, 19),-- Edit Material
(8, 20),-- Delete Material
(8, 21),-- Activate Material
(8, 22),-- Deactivate Material
(8, 23),-- Import Material

(9, 24),-- Export Material

(10, 29),-- Activate Material category
(10, 30),-- Deactivate Material category
(10, 31),-- Import Material category
(10, 32),-- Export Material category

(12, 33),-- View supplier
(12, 34),-- Create supplier
(12, 35),-- Edit supplier
(12, 36),-- Delete supplier

(13, 37),-- View purchase
(13, 38),-- Create purchase
(13, 39),-- Edit purchase
(13, 40),-- Cancel purchase
(13, 41),-- Approve purchase
(13, 43),-- View transfer
(13, 44),-- Create transfer
(13, 45),-- Edit transfer
(13, 46),-- Cancel transfer
(13, 42),-- Import goods
(13, 47),-- Ship goods
(13, 48),-- Receive goods

(14, 64),-- View area & table
(14, 65),-- Create area & table
(14, 66),-- Edit area & table
(14, 67),-- Delete area & table

(21, 72),-- View Fee
(21, 73),-- Create Fee
(21, 74),-- Delete Fee

(22, 75),-- View tax
(22, 76),-- Create tax
(22, 77),-- Delete tax

(23, 59),-- View promotion
(23, 60),-- Create promotion
(23, 61),-- Edit promotion
(23, 62),-- Delete promotion
(23, 63),-- Stop promotion

(25, 49),-- View customer
(25, 50),-- Create customer
(25, 51),-- Edit customer
(25, 52),-- Delete customer
(25, 85),-- View loyalty point

(25, 53),-- View segment
(26, 54),-- Create segment
(26, 55),-- Edit segment
(26, 56),-- Delete segment

(27, 66),-- View membership level
(27, 67),-- Create membership level
(27, 68),-- Edit membership level
(27, 69),-- Delete membership level

(30, 76),-- View shift

(31, 77),-- View order
(31, 78),-- Create order
(31, 79),-- Edit order
(31, 80),-- Delete order
(31, 81),-- Cancel order

(34, 82),-- Cashier
(34, 83),-- Service
(34, 84)-- Kitchen
GO