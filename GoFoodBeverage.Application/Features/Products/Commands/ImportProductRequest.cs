using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Product;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class ImportProductRequest : IRequest<ImportProductResponse>
    {
        public IFormFile File { get; set; }
    }

    public class ImportProductResponse
    {
        public bool Success { get; set; }

        public List<ImportMessageModel> Messages { get; set; }

        public class ImportMessageModel
        {
            public string Row { get; set; }

            public string Cell { get; set; }

            public string Message { get; set; }
        }
    }

    public class ImportProductRequestHandler : IRequestHandler<ImportProductRequest, ImportProductResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImportProductRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ImportProductResponse> Handle(ImportProductRequest request, CancellationToken cancellationToken)
        {
            // get lang code from request header
            var langCode = _httpContextAccessor.GetValueFromRequestHeader(RequestHeaderConstants.LANG_CODE);

            // set lang for response message
            var importProductMessage = new ImportProductMessage(langCode);

            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            // validate request
            ThrowError.ArgumentIsNull(request.File, nameof(request.File));

            var response = new ImportProductResponse()
            {
                Success = true,
                Messages = new List<ImportProductResponse.ImportMessageModel>()
            };

            // get excel records as list
            int productSheet = ImportProductFileConstants.PRODUCT_SHEET_INDEX;
            int productSheetHeadingIndex = ImportProductFileConstants.PRODUCT_SHEET_HEADING_INDEX;
            using var stream = request.File.OpenReadStream();
            var importProducts = stream.ReadRows<ImportProductExcelRecordModel>(
                productSheet,
                productSheetHeadingIndex,
                importProductMessage.InvalidFileFormat)
                .ToList();

            var isEmptyList = importProducts.Count() == 0 || importProducts.Count(i => string.IsNullOrWhiteSpace(i.ProductName)) == importProducts.Count();
            ThrowError.BadRequestAgainst(isEmptyList, "Please insert data in the file");

            // insert product list to database
            using var importProductTransaction = await _unitOfWork.BeginTransactionAsync();
            try

            {
                // get product entity model from list
               await GetProductEntityModelListAsync(
                importProducts,
                loggedUser.StoreId.Value,
                productSheetHeadingIndex + 2,
                response,
                importProductMessage);

                if (response.Success == false)
                {
                    return response;
                }
                else
                {
                    await _unitOfWork.SaveChangesAsync();
                    await importProductTransaction.CommitAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await importProductTransaction.RollbackAsync(cancellationToken);

                throw;
            }

            return response;
        }

        public async Task GetProductEntityModelListAsync(
            List<ImportProductExcelRecordModel> importRecords,
            Guid storeId,
            int startRowNumber,
            ImportProductResponse response,
            ImportProductMessage importProductMessage)
        {
            // get all product
            var allProducts = await _unitOfWork.Products
                .GetAllProductInStoreActive(storeId)
                .AsNoTracking()
                .Select(p => new Product()
                {
                    Name = p.Name,
                })
                .ToListAsync();

            // get all product category in store
            var allProductCategories = await _unitOfWork.ProductCategories
                .GetAllProductCategoriesInStore(storeId)
                .Select(pc => new ProductCategory()
                {
                    Id = pc.Id,
                    Code = pc.Code,
                    Name = pc.Name
                })
                .ToListAsync();

            // get all option in store
            var importOptionNames = importRecords.Where(io => string.IsNullOrWhiteSpace(io.Option) == false).SelectMany(io => io.Option.Trim().Split(','));
            var allOptions = await _unitOfWork.Options
                .GetAllOptionsInStore(storeId)
                .Select(o => new Option()
                {
                    Id = o.Id,
                    Code = o.Code,
                    Name = o.Name
                })
                .ToListAsync();

            // get all toppings in store
            var toppingNames = importRecords.Where(io => string.IsNullOrWhiteSpace(io.Topping) == false).SelectMany(io => io.Topping.Trim().Split(','));
            var allToppings = await _unitOfWork.Products
               .GetAllToppingActivatedInStore(storeId)
               .Select(o => new Product()
               {
                   Id = o.Id,
                   Code = o.Code,
                   Name = o.Name
               })
               .ToListAsync();

            // get materials by name
            var materialNames = importRecords.Where(io => string.IsNullOrWhiteSpace(io.Material) == false).Select(io => io.Material.Trim());
            var allMaterials = await _unitOfWork.Materials
               .GetAllMaterialsActivatedInStore(storeId)
               .Where(o => materialNames.Contains(o.Name))
               .Select(o => new Material()
               {
                   Id = o.Id,
                   Code = o.Code,
                   Name = o.Name
               })
               .ToListAsync();

            var unitNames = importRecords.Where(io => string.IsNullOrWhiteSpace(io.Unit) == false).Select(io => io.Unit.Trim());
            var allUnits = await _unitOfWork.Units
                .GetAllUnitsInStore(storeId)
                .Select(u => new Domain.Entities.Unit()
                {
                    Id = u.Id,
                    Name = u.Name
                })
                .ToListAsync();

            var importProducts = new List<Product>();
            var currentImportProduct = new Product();
            var importProductPrices = new List<ProductPrice>();
            var importProductPriceMaterials = new List<ProductPriceMaterial>();
            var importProductToppings = new List<ProductTopping>();
            foreach (var import in importRecords)
            {
                var currentIndex = importRecords.IndexOf(import);

                if (HasError(string.IsNullOrWhiteSpace(import.Quantity), nameof(import.Quantity), startRowNumber + currentIndex, importProductMessage.MaterialQuantityEmpty, response)) continue;
                if (HasError(string.IsNullOrWhiteSpace(import.Material), nameof(import.Material), startRowNumber + currentIndex, importProductMessage.MaterialEmpty, response)) continue;
                if (HasError(import.QuantityValue <= 0, nameof(import.Quantity), startRowNumber + currentIndex, importProductMessage.InvalidQuantity, response)) continue;

                // add new product and set current product
                if (string.IsNullOrWhiteSpace(import.ProductName) == false && import.ProductName != currentImportProduct.Name)
                {
                    if (HasError(string.IsNullOrWhiteSpace(import.ProductName), nameof(import.ProductName), startRowNumber + currentIndex, importProductMessage.ProductNameEmpty, response)) continue;
                    if (HasError(string.IsNullOrWhiteSpace(import.Unit), nameof(import.Unit), startRowNumber + currentIndex, importProductMessage.ProductUnitEmpty, response)) continue;
                    if (HasError(string.IsNullOrWhiteSpace(import.Price), nameof(import.Price), startRowNumber + currentIndex, importProductMessage.ProductPriceEmpty, response)) continue;

                    var productName = import.ProductName.Trim();
                    var existedProduct = allProducts.FirstOrDefault(product => product.Name == productName);
                    if (HasError(existedProduct != null, nameof(import.ProductName), startRowNumber + currentIndex, string.Format(importProductMessage.ExistedProduct, productName), response)) continue;

                    var productCategory = allProductCategories.FirstOrDefault(pc => pc.Name.ToLower().Trim() == import.Category?.ToLower().Trim());

                    var unitName = import.Unit?.ToLower().Trim();
                    var existedUnit = allUnits.FirstOrDefault(unit => unit.Name.ToLower().Trim() == unitName);
                    if (HasError(existedUnit == null, nameof(import.ProductName), startRowNumber + currentIndex, string.Format(importProductMessage.UnitNotExist, import.Unit), response)) continue;

                    var importProduct = new Product()
                    {
                        StoreId = storeId,
                        UnitId = existedUnit.Id,
                        TaxId = null,
                        StatusId = (int)EnumStatus.Active,
                        Name = import.ProductName,
                        Description = import.Description,
                        IsTopping = string.IsNullOrWhiteSpace(import.IsTopping) == false,
                        IsActive = true,
                        ProductOptions = new List<ProductOption>(),
                        ProductPlatforms = new List<ProductPlatform>(),
                    };

                    #region Add product category
                    if (productCategory != null)
                    {
                        var productProductCategory = new List<ProductProductCategory>
                        {
                            new ProductProductCategory()
                            {
                               ProductCategoryId = productCategory.Id,
                               ProductId = importProduct.Id,
                               Position = 1,
                               StoreId = storeId,
                            }
                        };

                        importProduct.ProductProductCategories = productProductCategory;
                    }
                    #endregion

                    #region Add product to channel
                    var productChannels = new List<ProductChannel>
                    {
                        new ProductChannel()
                        {
                            ChannelId = EnumChannel.InStore.ToGuid(),
                            ProductId = importProduct.Id,
                            StoreId = storeId,
                        }
                    };

                    importProduct.ProductChannels = productChannels;
                    #endregion

                    #region Add product to platform
                    var productPlatformPos = new ProductPlatform()
                    {
                        PlatformId = EnumPlatform.POS.ToGuid(),
                        ProductId = importProduct.Id,
                        StoreId = storeId,
                    };

                    var productPlatformApp = new ProductPlatform()
                    {
                        PlatformId = EnumPlatform.GoFnBApp.ToGuid(),
                        ProductId = importProduct.Id,
                        StoreId = storeId,
                    };

                    importProduct.ProductPlatforms.Add(productPlatformPos);
                    importProduct.ProductPlatforms.Add(productPlatformApp);
                    #endregion

                    #region Add option to product
                    if (string.IsNullOrWhiteSpace(import.Option) == false)
                    {
                        var options = import.Option.Trim().Split(',');
                        foreach (var option in options)
                        {
                            var storeOption = allOptions.FirstOrDefault(o => o.Code.ToLower().Trim() == option.ToLower().Trim());
                            if (HasError(storeOption == null, nameof(import.Option), startRowNumber + currentIndex, string.Format(importProductMessage.InvalidOption, option), response)) continue;

                            var productOption = new ProductOption()
                            {
                                ProductId = importProduct.Id,
                                OptionId = storeOption.Id,
                                StoreId = storeId,
                            };

                            importProduct.ProductOptions.Add(productOption);
                        }
                    }
                    #endregion

                    #region Add topping to product
                    if (string.IsNullOrWhiteSpace(import.Topping) == false)
                    {
                        var toppings = import.Topping.Trim().Split(',');
                        foreach (var topping in toppings)
                        {
                            var storeTopping = allToppings.FirstOrDefault(o => o.Code.ToString() == topping.ToLower().Trim());
                            if (HasError(storeTopping == null, nameof(import.Topping), startRowNumber + currentIndex, string.Format(importProductMessage.InvalidTopping, topping), response)) continue;

                            var productTopping = new ProductTopping()
                            {
                                ProductId = importProduct.Id,
                                ToppingId = storeTopping.Id,
                                StoreId = storeId,
                            };

                            importProductToppings.Add(productTopping);
                        }
                    }
                    #endregion

                    #region Add product price and material
                    if (HasError(import.PriceValue == 0, nameof(import.Price), startRowNumber + currentIndex, importProductMessage.InvalidPriceValue, response)) continue;

                    var material = allMaterials.FirstOrDefault(m => m.Name.ToLower() == import.Material.ToLower().Trim());
                    if (HasError(material == null, nameof(import.Material), startRowNumber + currentIndex, string.Format(importProductMessage.InvalidMaterial, import.Material), response)) continue;

                    var productPrice = new ProductPrice()
                    {
                        ProductId = importProduct.Id,
                        PriceName = import.PriceName,
                        PriceValue = import.PriceValue,
                        StoreId = storeId,
                    };

                    var productPriceMaterial = new ProductPriceMaterial()
                    {
                        ProductPriceId = productPrice.Id,
                        MaterialId = material.Id,
                        Quantity = import.QuantityValue,
                        StoreId = storeId,
                    };

                    importProductPriceMaterials.Add(productPriceMaterial);
                    importProductPrices.Add(productPrice);
                    #endregion

                    // add product
                    importProducts.Add(importProduct);

                    // set current product
                    currentImportProduct = importProduct;
                }
                // add product price or material for this current product
                else
                {
                    // get product from current
                    var product = importProducts.FirstOrDefault(p => p.Id == currentImportProduct.Id);
                    if (product == null) continue;

                    #region Add product price and material
                    if (string.IsNullOrWhiteSpace(import.Material) == false && string.IsNullOrWhiteSpace(import.Price) == false)
                    {
                        #region Add product price and material
                        if (HasError(import.PriceValue == 0, nameof(import.Price), startRowNumber + currentIndex, importProductMessage.InvalidPriceValue, response)) continue;

                        var material = allMaterials.FirstOrDefault(m => m.Name.ToLower() == import.Material.ToLower().Trim());
                        if (HasError(material == null, nameof(import.Material), startRowNumber + currentIndex, string.Format(importProductMessage.InvalidMaterial, import.Material), response)) continue;

                        var productPrice = new ProductPrice()
                        {
                            ProductId = product.Id,
                            PriceName = import.PriceName,
                            PriceValue = import.PriceValue,
                            StoreId = storeId,
                            ProductPriceMaterials = new List<ProductPriceMaterial>()
                        };

                        var productPriceMaterial = new ProductPriceMaterial()
                        {
                            ProductPriceId = productPrice.Id,
                            MaterialId = material.Id,
                            Quantity = import.QuantityValue,
                            StoreId = storeId,
                        };

                        importProductPriceMaterials.Add(productPriceMaterial);
                        importProductPrices.Add(productPrice);
                        #endregion

                        continue;
                    }
                    #endregion

                    #region Add material
                    if (string.IsNullOrWhiteSpace(import.Material) == false)
                    {
                        var material = allMaterials.FirstOrDefault(m => m.Name.ToLower() == import.Material.ToLower().Trim());
                        if (HasError(material == null, nameof(import.Material), startRowNumber + currentIndex, string.Format(importProductMessage.InvalidMaterial, import.Material), response)) continue;

                        var productPrice = importProductPrices.Last();
                        var importProductPriceMaterial = importProductPriceMaterials.FirstOrDefault(i => i.ProductPriceId == productPrice.Id);
                        if (HasError(importProductPriceMaterial.MaterialId == material.Id, nameof(import.Material), startRowNumber + currentIndex, string.Format(importProductMessage.DuplicatedMaterial, import.Material), response)) continue;

                        var productPriceMaterial = new ProductPriceMaterial()
                        {
                            ProductPriceId = productPrice.Id,
                            MaterialId = material.Id,
                            Quantity = import.QuantityValue,
                            StoreId = storeId,
                        };

                        importProductPriceMaterials.Add(productPriceMaterial);

                        continue;
                    }
                    #endregion
                }
            };

            await _unitOfWork.Products.AddRangeAsync(importProducts);
            await _unitOfWork.ProductPrices.AddRangeAsync(importProductPrices);
            await _unitOfWork.ProductPriceMaterials.AddRangeAsync(importProductPriceMaterials);
        }

        /// <summary>
        /// Check record error and collect errors
        /// </summary>
        /// <param name="conditionPassed"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="message"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static bool HasError(bool conditionPassed, string column, int row, string message, ImportProductResponse response)
        {
            if (conditionPassed)
            {
                response.Success = false;
                var columnLabel = typeof(ImportProductExcelRecordModel).GetColunmLabel(column);
                response.Messages.Add(new ImportProductResponse.ImportMessageModel()
                {
                    Cell = $"{columnLabel}{row}",
                    Row = $"{row}",
                    Message = message
                });
            }

            return conditionPassed;
        }
    }
}
