UPDATE fb SET fb.StoreId = f.StoreId FROM [FeeBranch] fb --- Update FeeBranch
INNER JOIN [Fee] f ON f.Id = fb.FeeId WHERE fb.StoreId IS NULL

UPDATE fst SET fst.StoreId = f.StoreId FROM [FeeServingType] fst --- Update FeeServingType
INNER JOIN [Fee] f ON f.Id = fst.FeeId WHERE fst.StoreId IS NULL