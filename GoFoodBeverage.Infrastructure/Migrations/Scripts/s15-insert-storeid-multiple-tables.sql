UPDATE art SET art.StoreId = a.StoreId FROM [AreaTable] art --- Update AreaTable
INNER JOIN [Area] a ON art.AreaId = a.Id WHERE art.StoreId IS NULL

UPDATE cp SET cp.StoreId = c.StoreId FROM [ComboPricing] cp --- Update ComboPricing
INNER JOIN [Combo] c ON cp.ComboId = c.Id WHERE cp.StoreId IS NULL

UPDATE cppp SET cppp.StoreId = c.StoreId FROM [ComboPricingProductPrice] cppp --- Update ComboPricingProductPrice
INNER JOIN [ComboPricing] cp ON cppp.ComboPricingId = cp.Id
INNER JOIN [Combo] c ON cp.ComboId = c.Id WHERE cppp.StoreId IS NULL

UPDATE cpg SET cpg.StoreId = c.StoreId FROM [ComboProductGroup] cpg --- Update ComboProductGroup
INNER JOIN [Combo] c ON cpg.ComboId = c.Id WHERE cpg.StoreId IS NULL

UPDATE cpgpp SET cpgpp.StoreId = c.StoreId FROM [ComboProductGroupProductPrice] cpgpp --- Update ComboProductGroupProductPrice
INNER JOIN [ComboProductGroup] cpg ON cpgpp.ComboProductGroupId = cpg.Id 
INNER JOIN [Combo] c ON cpg.ComboId = c.Id WHERE cpgpp.StoreId IS NULL

UPDATE cpp SET cpp.StoreId = c.StoreId FROM [ComboProductPrice] cpp --- Update ComboProductPrice
INNER JOIN [Combo] c ON cpp.ComboId = c.Id WHERE cpp.StoreId IS NULL

UPDATE csb SET csb.StoreId = c.StoreId FROM [ComboStoreBranch] csb --- Update ComboStoreBranch
INNER JOIN [Combo] c ON csb.ComboId = c.Id WHERE csb.StoreId IS NULL

UPDATE cp SET cp.StoreId = c.StoreId FROM [CustomerPoint] cp --- Update CustomerPoint
INNER JOIN [Customer] c ON cp.CustomerId = c.Id WHERE cp.StoreId IS NULL

UPDATE csc SET csc.StoreId = cs.StoreId FROM [CustomerSegmentCondition] csc --- Update CustomerSegmentCondition
INNER JOIN [CustomerSegment] cs ON csc.CustomerSegmentId = cs.Id WHERE csc.StoreId IS NULL

UPDATE dcp SET dcp.StoreId = dc.StoreId FROM [DeliveryConfigPricing] dcp --- Update DeliveryConfigPricing
INNER JOIN [DeliveryConfig] dc ON dcp.DeliveryConfigId = dc.Id WHERE dcp.StoreId IS NULL

UPDATE ol SET ol.StoreId = o.StoreId FROM [OptionLevel] ol --- Update OptionLevel
INNER JOIN [Option] o ON ol.OptionId = o.Id WHERE ol.StoreId IS NULL

UPDATE pc SET pc.StoreId = p.StoreId FROM [ProductChannel] pc --- Update ProductChannel
INNER JOIN [Product] p ON pc.ProductId = p.Id WHERE pc.StoreId IS NULL

UPDATE po SET po.StoreId = p.StoreId FROM [ProductOption] po --- Update ProductOption
INNER JOIN [Product] p ON po.ProductId = p.Id WHERE po.StoreId IS NULL

UPDATE pp SET pp.StoreId = p.StoreId FROM [ProductPlatform] pp --- Update ProductPlatform
INNER JOIN [Product] p ON pp.ProductId = p.Id WHERE pp.StoreId IS NULL

UPDATE pp SET pp.StoreId = p.StoreId FROM [ProductPrice] pp --- Update ProductPrice
INNER JOIN [Product] p ON pp.ProductId = p.Id WHERE pp.StoreId IS NULL

UPDATE ppm SET ppm.StoreId = p.StoreId FROM [ProductPriceMaterial] ppm --- Update ProductPriceMaterial
INNER JOIN [ProductPrice] pp ON ppm.ProductPriceId = pp.Id 
INNER JOIN [Product] p ON pp.ProductId = p.Id WHERE ppm.StoreId IS NULL

UPDATE ppc SET ppc.StoreId = p.StoreId FROM [ProductProductCategory] ppc --- Update ProductProductCategory
INNER JOIN [Product] p ON ppc.ProductId = p.Id WHERE ppc.StoreId IS NULL

UPDATE pt SET pt.StoreId = p.StoreId FROM [ProductTopping] pt --- Update ProductTopping
INNER JOIN [Product] p ON pt.ProductId = p.Id WHERE pt.StoreId IS NULL

UPDATE pb SET pb.StoreId = p.StoreId FROM [PromotionBranch] pb --- Update PromotionBranch
INNER JOIN [Promotion] p ON pb.PromotionId = p.Id WHERE pb.StoreId IS NULL

UPDATE pp SET pp.StoreId = p.StoreId FROM [PromotionProduct] pp --- Update PromotionProduct
INNER JOIN [Promotion] p ON pp.PromotionId = p.Id WHERE pp.StoreId IS NULL

UPDATE ppc SET ppc.StoreId = p.StoreId FROM [PromotionProductCategory] ppc --- Update PromotionProductCategory
INNER JOIN [Promotion] p ON ppc.PromotionId = p.Id WHERE ppc.StoreId IS NULL

UPDATE pom SET pom.StoreId = po.StoreId FROM [PurchaseOrderMaterial] pom --- Update PurchaseOrderMaterial
INNER JOIN [PurchaseOrder] po ON pom.PurchaseOrderId = po.Id WHERE pom.StoreId IS NULL

UPDATE sbpc SET sbpc.StoreId = sb.StoreId FROM [StoreBranchProductCategory] sbpc --- Update StoreBranchProductCategory
INNER JOIN [StoreBranch] sb ON sbpc.StoreBranchId = sb.Id WHERE sbpc.StoreId IS NULL

UPDATE oi SET oi.StoreId = o.StoreId FROM [OrderItem] oi --- Update OrderItem
INNER JOIN [Order] o ON oi.OrderId = o.Id WHERE oi.StoreId IS NULL

UPDATE oci SET oci.StoreId = o.StoreId FROM [OrderComboItem] oci --- Update OrderComboItem
INNER JOIN [OrderItem] oi ON oci.OrderItemId = oi.Id 
INNER JOIN [Order] o ON oi.OrderId = o.Id WHERE oci.StoreId IS NULL

UPDATE ocppi SET ocppi.StoreId = o.StoreId FROM [OrderComboProductPriceItem] ocppi --- Update OrderComboProductPriceItem
INNER JOIN [OrderComboItem] oci ON ocppi.OrderComboItemId = oci.Id 
INNER JOIN [OrderItem] oi ON oci.OrderItemId = oi.Id 
INNER JOIN [Order] o ON oi.OrderId = o.Id WHERE ocppi.StoreId IS NULL

UPDATE odf SET odf.StoreId = o.StoreId FROM [OrderFee] odf --- Update OrderFee
INNER JOIN [Order] o ON odf.OrderId = o.Id WHERE odf.StoreId IS NULL

UPDATE oh SET oh.StoreId = o.StoreId FROM [OrderHistory] oh --- Update OrderHistory
INNER JOIN [Order] o ON oh.OrderId = o.Id WHERE oh.StoreId IS NULL

UPDATE oio SET oio.StoreId = o.StoreId FROM [OrderItemOption] oio --- Update OrderItemOption
INNER JOIN [OrderItem] oi ON oio.OrderItemId = oi.Id 
INNER JOIN [Order] o ON oi.OrderId = o.Id WHERE oio.StoreId IS NULL

UPDATE oit SET oit.StoreId = o.StoreId FROM [OrderItemTopping] oit --- Update OrderItemTopping
INNER JOIN [OrderItem] oi ON oit.OrderItemId = oi.Id 
INNER JOIN [Order] o ON oi.OrderId = o.Id WHERE oit.StoreId IS NULL

UPDATE gpb SET gpb.StoreId = gp.StoreId FROM [GroupPermissionBranches] gpb --- Update GroupPermissionBranches
INNER JOIN [GroupPermission] gp ON gpb.GroupPermissionId = gp.Id WHERE gpb.StoreId IS NULL

UPDATE sgpb SET sgpb.StoreId = s.StoreId FROM [StaffGroupPermissionBranch] sgpb --- Update StaffGroupPermissionBranch
INNER JOIN [Staff] s ON sgpb.StaffId = s.Id WHERE sgpb.StoreId IS NULL

UPDATE od SET od.StoreId = o.StoreId FROM [OrderDelivery] od --- Update OrderDelivery
INNER JOIN [Order] o ON o.OrderDeliveryId = od.Id WHERE od.StoreId IS NULL

UPDATE oir SET oir.StoreId = o.StoreId FROM [OrderItemRestore] oir --- Update OrderItemRestore
INNER JOIN [Order] o ON oir.OrderId = o.Id WHERE oir.StoreId IS NULL

UPDATE os SET os.StoreId = o.StoreId FROM [OrderSession] os --- Update OrderSession
INNER JOIN [Order] o ON os.OrderId = o.Id WHERE os.StoreId IS NULL