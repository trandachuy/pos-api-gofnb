
--- UPDATE POS PACKAGE
UPDATE [dbo].[Package]
SET AvailableBranchNumber = 0
UPDATE [dbo].[Package]
SET AvailableBranchNumber = 1, CostPerMonth = 499000 WHERE Name = 'POS'

--- DEACTIVE STORES
UPDATE Store
Set IsActivated = 0

--- ONLY ACTIVE STORES IF HAS ORDER PACKAGE
DECLARE @StoreCodeOrderPackageId nvarchar(128)
DECLARE MY_CURSOR CURSOR
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR (SELECT CONCAT(Id,',',StoreCode) FROM [dbo].[OrderPackage] where StoreCode != 0) 

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @StoreCodeOrderPackageId
WHILE @@FETCH_STATUS = 0
BEGIN

	-- BEGIN UPDATE Store activated by order package
	UPDATE [dbo].[Store]
	SET IsActivated = 1, ActivatedByOrderPackageId = (SELECT SUBSTRING(@StoreCodeOrderPackageId, 0, 37))
	WHERE Code = (SELECT SUBSTRING(@StoreCodeOrderPackageId, 38, len(@StoreCodeOrderPackageId) + 1))
	-- END UPDATE Store activated by order package

    FETCH NEXT FROM MY_CURSOR INTO @StoreCodeOrderPackageId
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR
