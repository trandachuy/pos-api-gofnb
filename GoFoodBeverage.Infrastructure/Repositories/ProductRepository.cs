using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Models.Product;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        private IQueryable<Product> GetAllProducts()
        {
            return dbSet.Where(x => x.IsActive);
        }

        public Task<Product> GetProductByNameInStoreAsync(string productName, Guid storeId)
        {
            var product = GetAllProducts().FirstOrDefaultAsync(p => p.Name.Trim().ToLower() == productName.Trim().ToLower() && p.StoreId == storeId);

            return product;
        }

        public Task<Product> GetProductActiveByNameInStoreAsync(string productName, Guid storeId)
        {
            var product = GetAllProducts().FirstOrDefaultAsync(p => p.Name.Trim().ToLower() == productName.Trim().ToLower() && p.StoreId == storeId && p.StatusId == (int)EnumStatus.Active);

            return product;
        }

        public IQueryable<Product> GetAllProductInStore(Guid storeId)
        {
            var products = GetAllProducts().Where(s => s.StoreId == storeId);

            return products;
        }

        public IQueryable<Product> GetAllProductInStoreActive(Guid storeId)
        {
            var products = GetAllProducts().Where(s => s.StoreId == storeId && s.StatusId == (int)EnumStatus.Active);

            return products;
        }

        public IQueryable<Product> GetProductByIdInStore(Guid storeId, Guid id)
        {
            var product = GetAllProducts().Where(s => s.StoreId == storeId && s.Id == id);

            return product;
        }

        public Task<Product> GetPOSProductDetailByIdAsync(Guid id)
        {
            var product = GetAllProducts().Where(p => p.Id == id)
                .Include(x => x.ProductOptions).ThenInclude(x => x.Option).ThenInclude(x => x.OptionLevel)
                .Include(x => x.ProductPrices)
                .FirstOrDefaultAsync();
            return product;
        }

        public IQueryable<Product> GetAllToppingActivatedInStore(Guid storeId)
        {
            var toppings = GetAllProducts().Where(t => t.IsTopping && t.StatusId == (int)EnumStatus.Active && t.StoreId == storeId)
                .Include(t => t.ProductPrices);

            return toppings;
        }

        public IQueryable<Product> GetAllToppingBelongToProduct(Guid storeId, Guid productId)
        {
            var toppingIds = _dbContext.ProductToppings
                .Where(productTopping => productTopping.ProductId == productId)
                .Select(productTopping => productTopping.ToppingId);

            var toppings = GetAllProducts().Where(t => t.StoreId == storeId && t.IsTopping && t.StatusId == (int)EnumStatus.Active && toppingIds.Contains(t.Id))
                .Include(t => t.ProductPrices);

            return toppings;
        }

        public async Task<bool> CheckEditProductByNameInStoreAsync(Guid productId, string productName, Guid storeId)
        {
            var productExsited = await GetAllProducts().AnyAsync(p => p.StoreId == storeId && p.Id != productId && p.Name.Trim().ToLower() == productName.Trim().ToLower());

            return productExsited;
        }

        public async Task<Tuple<bool, int?, string>> UpdateProductAsync(Guid storeId, UpdateProductModel productEditModel)
        {
            int? productCode = null;
            string thumbnail = string.Empty;
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    //Edit Product
                    var productEdit = GetAllProducts().FirstOrDefault(x => x.Id == productEditModel.ProductId);
                    productEdit.Name = productEditModel.Name;
                    productEdit.Description = productEditModel.Description;
                    productEdit.Thumbnail = productEditModel.Image;
                    productEdit.UnitId = productEditModel.UnitId;
                    productEdit.IsTopping = productEditModel.IsTopping;
                    productEdit.TaxId = productEditModel.TaxId;
                    _dbContext.Entry(productEdit).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    productCode = productEdit.Code;
                    thumbnail = productEdit.Thumbnail;

                    //Edit Product CateGory
                    if (productEditModel.IsTopping || productEditModel.ProductCategoryId == null)
                    {
                        var productCategoryCategoryDelete = _dbContext.ProductProductCategories.Where(x => x.ProductId == productEditModel.ProductId);
                        if (productCategoryCategoryDelete.Count() > 0)
                        {
                            _dbContext.ProductProductCategories.RemoveRange(productCategoryCategoryDelete);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var productCategoryCategoryEdit = _dbContext.ProductProductCategories.Where(x => x.ProductId == productEditModel.ProductId);
                        if (productCategoryCategoryEdit.Count() > 0)
                        {
                            _dbContext.ProductProductCategories.RemoveRange(productCategoryCategoryEdit);
                            await _dbContext.SaveChangesAsync();
                        }

                        /// Get highest position of product in category
                        int positionInCategory = 0;
                        var listProductInCategory = _dbContext.ProductProductCategories
                            .Where(x => x.ProductCategoryId == productEditModel.ProductCategoryId)
                            .ToList();
                        if (listProductInCategory.Count() > 0)
                        {
                            positionInCategory = listProductInCategory.Select(x => x.Position).Max();
                        }

                        _dbContext.ProductProductCategories.Add(new ProductProductCategory { ProductId = productEditModel.ProductId, ProductCategoryId = productEditModel.ProductCategoryId.Value, Position = positionInCategory + 1, StoreId = storeId });
                        await _dbContext.SaveChangesAsync();
                    }

                    #region Handle update product prices
                    var oldProductPrices = _dbContext.ProductPrices.Where(x => x.ProductId == productEditModel.ProductId).ToList();

                    // get combo has product
                    var comboPricings = await _dbContext.ComboPricingProducts
                            .Where(c => oldProductPrices.Select(pp => pp.Id).Contains(c.ProductPriceId.Value))
                            .Include(c => c.ComboPricing).ThenInclude(cp => cp.Combo)
                            .AsNoTracking()
                            .Select(c => new
                            {
                                ComboName = c.ComboPricing.Combo.Name,
                                ProductPriceId = c.ProductPriceId.Value
                            })
                            .ToListAsync();

                    var comboProducts = await _dbContext.ComboProductPrices
                        .Where(cpp => oldProductPrices.Select(pp => pp.Id).Contains(cpp.ProductPriceId))
                        .Include(cpp => cpp.Combo)
                        .AsNoTracking()
                        .Select(c => new
                        {
                            ComboName = c.Combo.Name,
                            ProductPriceId = c.ProductPriceId
                        })
                        .ToListAsync();
                    var comboProductPrices = comboPricings.Concat(comboProducts);

                    // update multiple prices
                    if (productEditModel.Prices.Any())
                    {
                        var pricesRequestUpdate = new List<UpdateProductModel.PriceDto>();
                        var newPrices = new List<ProductPrice>();
                        var updatePrices = new List<ProductPrice>();
                        var removePrices = new List<ProductPrice>();

                        // update product price existed
                        foreach (var productPrice in oldProductPrices)
                        {
                            var newProductPrice = productEditModel.Prices.FirstOrDefault(p => p.Id == productPrice.Id);
                            if (newProductPrice == null)
                            {
                                var existedCombo = comboProductPrices.FirstOrDefault(cpp => cpp.ProductPriceId == productPrice.Id);
                                var productPriceName = string.IsNullOrEmpty(productPrice.PriceName) ?
                                productPrice.Product.Name :
                                $"{productPrice.Product.Name} ({productPrice.PriceName})";
                                ThrowError.Against(existedCombo != null, new JObject() // if the product has combo => throw message
                                {
                                    { "message", "messages.cannotEditProductHasCombo" },
                                    { "productName", productPriceName },
                                    { "comboName", existedCombo?.ComboName },
                                });

                                removePrices.Add(productPrice);
                                continue;
                            }
                            else
                            {
                                productPrice.PriceName = newProductPrice.Name;
                                productPrice.PriceValue = newProductPrice.Price;
                                pricesRequestUpdate.Add(newProductPrice);
                                updatePrices.Add(productPrice);
                            }
                        }

                        _dbContext.ProductPrices.RemoveRange(removePrices);
                        _dbContext.ProductPrices.UpdateRange(updatePrices);

                        // add new product price
                        var pricesRemaining = productEditModel.Prices.Where(p => !pricesRequestUpdate.Any(u => u.Name == p.Name && u.Price == u.Price));
                        if (pricesRemaining.Any())
                        {
                            foreach (var newPrice in pricesRemaining)
                            {
                                var newProductPrice = new ProductPrice()
                                {
                                    PriceName = newPrice.Name,
                                    PriceValue = newPrice.Price,
                                    ProductId = productEdit.Id,
                                    StoreId = storeId,
                                };

                                newPrices.Add(newProductPrice);
                            }

                            _dbContext.ProductPrices.AddRange(newPrices);
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                    // update single price
                    else
                    {
                        bool updated = false;
                        var productPricesHasCombo = new List<Tuple<string, ProductPrice>>();
                        var updatePrices = new List<ProductPrice>();
                        var removePrices = new List<ProductPrice>();
                        foreach (var productPrice in oldProductPrices)
                        {
                            var existedCombo = comboProductPrices.FirstOrDefault(cpp => cpp.ProductPriceId == productPrice.Id);
                            if (existedCombo == null)
                            {
                                if (updated == true)
                                {
                                    removePrices.Add(productPrice);
                                }
                                else
                                {
                                    productPrice.PriceName = string.Empty; // Not set price name for single price case
                                    productPrice.PriceValue = productEditModel.Price;
                                    updatePrices.Add(productPrice);
                                    updated = true;
                                }
                            }
                            else
                            {
                                productPricesHasCombo.Add(new Tuple<string, ProductPrice>(existedCombo.ComboName, productPrice));
                            }
                        }

                        if (productPricesHasCombo.Any())
                        {
                            var existedCombo = productPricesHasCombo.First();
                            var productPriceName = string.IsNullOrEmpty(existedCombo.Item2.PriceName) ?
                                existedCombo.Item2.Product.Name :
                                $"{existedCombo.Item2.Product.Name} ({existedCombo.Item2.PriceName})";
                            ThrowError.Against(existedCombo != null, new JObject() // if the product has combo => throw message
                            {
                                { "message", "messages.cannotEditProductHasCombo" },
                                { "productName", productPriceName },
                                { "comboName", existedCombo.Item1},
                            });
                        }
                        else
                        {
                            _dbContext.ProductPrices.RemoveRange(removePrices);
                            _dbContext.ProductPrices.UpdateRange(updatePrices);
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion

                    // Edit Product Option
                    var productOptions = _dbContext.ProductOptions.Where(x => x.ProductId == productEditModel.ProductId);
                    var listOptionId = productOptions.Select(x => x.OptionId);
                    var listOptionIdUpdate = new List<Guid>();
                    var listOptionIdInsert = new List<Guid>();
                    foreach (var optionId in productEditModel.OptionIds)
                    {
                        if (listOptionId.Contains(optionId))
                        {
                            listOptionIdUpdate.Add(optionId);
                        }
                        else
                        {
                            listOptionIdInsert.Add(optionId);
                        }

                    }
                    var productOptionDelete = productOptions.Where(x => !listOptionIdUpdate.Contains(x.OptionId));
                    if (productOptionDelete.Count() > 0)
                    {
                        _dbContext.ProductOptions.RemoveRange(productOptionDelete);
                        await _dbContext.SaveChangesAsync();
                    }
                    if (listOptionIdInsert.Count() > 0)
                    {
                        var listProductOptionAdd = new List<ProductOption>();
                        foreach (var optionId in listOptionIdInsert)
                        {
                            var productOption = new ProductOption
                            {
                                ProductId = productEditModel.ProductId,
                                OptionId = optionId,
                                StoreId = storeId
                            };

                            listProductOptionAdd.Add(productOption);
                        }
                        if (listProductOptionAdd.Count > 0)
                        {
                            _dbContext.ProductOptions.AddRange(listProductOptionAdd);
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    // Edit Product Inventory
                    if (productEditModel.Prices.Count == 0)
                    {
                        var productPrice = _dbContext.ProductPrices.FirstOrDefault(x => x.ProductId == productEditModel.ProductId);
                        if (productPrice != null && productEditModel.Materials.Any())
                        {
                            var listMaterialId = _dbContext.ProductPriceMaterials.Where(x => x.ProductPriceId == productPrice.Id).Select(x => x.MaterialId);
                            foreach (var material in productEditModel.Materials)
                            {
                                listMaterialId = listMaterialId.Where(x => x != material.MaterialId);
                                var checkProductPriceMateriaExist = _dbContext.ProductPriceMaterials.FirstOrDefault(x => x.ProductPriceId == productPrice.Id && x.MaterialId == material.MaterialId);
                                if (checkProductPriceMateriaExist == null)
                                {
                                    var productPriceMaterialAdd = new ProductPriceMaterial
                                    {
                                        ProductPriceId = productPrice.Id,
                                        MaterialId = material.MaterialId,
                                        Quantity = material.Quantity,
                                        StoreId = storeId
                                    };

                                    _dbContext.ProductPriceMaterials.Add(productPriceMaterialAdd);
                                    await _dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    checkProductPriceMateriaExist.Quantity = material.Quantity;

                                    _dbContext.Entry(checkProductPriceMateriaExist).State = EntityState.Modified;
                                    await _dbContext.SaveChangesAsync();
                                }
                            }
                            if (listMaterialId.Count() > 0)
                            {
                                var productPriceRemove = _dbContext.ProductPriceMaterials.Where(x => x.ProductPriceId == productPrice.Id && listMaterialId.Contains(x.MaterialId));
                                _dbContext.ProductPriceMaterials.RemoveRange(productPriceRemove);
                                await _dbContext.SaveChangesAsync();
                            }

                        }
                    }
                    else
                    {
                        var productPrices = _dbContext.ProductPrices.Where(x => x.ProductId == productEditModel.ProductId);
                        foreach (var price in productEditModel.Prices)
                        {
                            var checkProductPriceNameExist = productPrices.FirstOrDefault(x => x.PriceName == price.Name);
                            var listMaterialId = _dbContext.ProductPriceMaterials.Where(x => x.ProductPriceId == checkProductPriceNameExist.Id).Select(x => x.MaterialId);

                            if (checkProductPriceNameExist != null)
                            {
                                foreach (var material in price.Materials)
                                {
                                    listMaterialId = listMaterialId.Where(x => x != material.MaterialId);
                                    var checkProductPriceMateriaExist = _dbContext.ProductPriceMaterials.FirstOrDefault(x => x.ProductPriceId == checkProductPriceNameExist.Id && x.MaterialId == material.MaterialId);
                                    if (checkProductPriceMateriaExist == null)
                                    {
                                        var productPriceMaterialAdd = new ProductPriceMaterial
                                        {
                                            ProductPriceId = checkProductPriceNameExist.Id,
                                            MaterialId = material.MaterialId,
                                            Quantity = material.Quantity,
                                            StoreId = storeId
                                        };

                                        _dbContext.ProductPriceMaterials.Add(productPriceMaterialAdd);
                                        await _dbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        checkProductPriceMateriaExist.Quantity = material.Quantity;

                                        _dbContext.Entry(checkProductPriceMateriaExist).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();
                                    }
                                }
                            }
                            if (listMaterialId.Count() > 0)
                            {
                                var productPriceRemove = _dbContext.ProductPriceMaterials.Where(x => x.ProductPriceId == checkProductPriceNameExist.Id && listMaterialId.Contains(x.MaterialId));
                                _dbContext.ProductPriceMaterials.RemoveRange(productPriceRemove);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                    }

                    // Edit Product Platform
                    if (productEditModel.PlatformIds != null)
                    {
                        var productPlatforms = _dbContext.ProductPlatforms.Where(x => x.ProductId == productEditModel.ProductId);
                        var listPlatformId = productPlatforms.Select(x => x.PlatformId);
                        var listPlatformIdUpdate = new List<Guid>();
                        var listPlatformIdInsert = new List<Guid>();
                        foreach (var platformId in productEditModel.PlatformIds)
                        {
                            if (listPlatformId.Contains(platformId))
                            {
                                listPlatformIdUpdate.Add(platformId);
                            }
                            else
                            {
                                listPlatformIdInsert.Add(platformId);
                            }
                        }

                        var productPlatformDelete = productPlatforms.Where(x => !listPlatformIdUpdate.Contains(x.PlatformId));
                        if (productPlatformDelete.Any())
                        {
                            _dbContext.ProductPlatforms.RemoveRange(productPlatformDelete);
                            await _dbContext.SaveChangesAsync();
                        }

                        if (listPlatformIdInsert.Count > 0)
                        {
                            var listProductPlatformAdd = new List<ProductPlatform>();
                            foreach (var platformId in listPlatformIdInsert)
                            {
                                var productPlatform = new ProductPlatform
                                {
                                    ProductId = productEditModel.ProductId,
                                    PlatformId = platformId,
                                    StoreId = storeId
                                };

                                listProductPlatformAdd.Add(productPlatform);
                            }

                            if (listProductPlatformAdd.Count > 0)
                            {
                                _dbContext.ProductPlatforms.AddRange(listProductPlatformAdd);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();

                    throw;
                }
            }

            return Tuple.Create(false, productCode, thumbnail);
        }

        public IQueryable<Product> GetAllProductIncludedProductUnit(Guid storeId)
        {
            var product = GetAllProducts()
                .Where(s => s.StoreId == storeId)
                .Include(x => x.Unit);

            return product;
        }
    }
}
