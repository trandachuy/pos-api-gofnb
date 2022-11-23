-- Update all Order status from New to Draft
UPDATE [dbo].[Order]
SET StatusId = 7
WHERE StatusId = 0