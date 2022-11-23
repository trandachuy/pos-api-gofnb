using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Orders.Commands
{
    public class CheckAddProductForOrderRequest : IRequest<CheckAddProductForOrderResponse>
    {
        public List<ProductCartItemModel> CartItems { get; set; }

        public List<ProductAddIntoOrderModel> ProductList { get; set; }
    }

    public class CheckAddProductForOrderResponse
    {
        public CheckAddProductForOrderResponse()
        {
            ProductInformationListResponse = new List<ProductInformationResponse>();
        }

        public bool? IsAllowOutOfMaterial { get; set; }

        public List<ProductInformationResponse> ProductInformationListResponse { get; set; }

        public class ProductInformationResponse
        {
            public Guid? ProductId { get; set; }

            public string ProductName { get; set; }
        }
    }

    public class CheckAddProductForOrderReStructureRequestHandler : IRequestHandler<CheckAddProductForOrderRequest, CheckAddProductForOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public CheckAddProductForOrderReStructureRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<CheckAddProductForOrderResponse> Handle(CheckAddProductForOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            Guid branchId = loggedUser.BranchId.Value;
            CheckAddProductForOrderResponse response = new CheckAddProductForOrderResponse();
            List<ProductAddIntoOrderModel> productList = MergeProductFromCartItems(request.CartItems);
            List<Guid?> productIds = productList.Where(a => a.ProductId.HasValue).Select(a => a.ProductId).Distinct().ToList();
            List<MaterialInventoryBranch> materialInventoryList = await _unitOfWork.MaterialInventoryBranches
                .GetAll()
                .Where(materialInventoryBranch => materialInventoryBranch.StoreId == loggedUser.StoreId && materialInventoryBranch.BranchId == branchId && materialInventoryBranch.Material.IsActive == true)
                .Select(materialInventoryBranch => new MaterialInventoryBranch()
                {
                    BranchId = materialInventoryBranch.BranchId,
                    MaterialId = materialInventoryBranch.MaterialId,
                    Material = materialInventoryBranch.Material,
                    Quantity = materialInventoryBranch.Quantity
                })
                .AsNoTracking()
                .ToListAsync();
            List<MaterialInformationNeededForProductModel> materialNeededForProductList = await _unitOfWork.Products
                .GetAll()
                .Where(product => product.StoreId == loggedUser.StoreId)
                .Select(product => new MaterialInformationNeededForProductModel()
                {
                    IsAllowOutOfMaterial = product.Store.IsCheckProductSell,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    MaterialInformationList = product.ProductPrices
                    .SelectMany(e => e.ProductPriceMaterials)
                    .Select(s => new MaterialInformationNeededForProductModel.MaterialInformation()
                    {
                        MaterialId = s.MaterialId,
                        QuantityNeeded = s.Quantity
                    }).ToList(),
                    OptionLevelInformationList = product.ProductOptions
                    .SelectMany(s => s.Option.OptionLevel)
                    .Select(s => new MaterialInformationNeededForProductModel.OptionLevelInformation()
                    {
                        OptionId = s.OptionId,
                        OptionLevelId = s.Id,
                        Quota = s.Quota,
                        MaterialId = s.Option.MaterialId,
                        QuantityNeeded = s.Option.Material != null ? s.Option.Material.Quantity : 0
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();
            response.IsAllowOutOfMaterial = materialNeededForProductList.FirstOrDefault()?.IsAllowOutOfMaterial;
            List<MaterialInformationCheckModel> materialInformationChecks = new List<MaterialInformationCheckModel>();

            foreach (Guid? productId in productIds)
            {
                if (!productId.HasValue || productId == Guid.Empty)
                    continue;
                var productListQuery = productList.Where(a => a.ProductId == productId).ToList();
                foreach (var product in productListQuery)
                {
                    var materialList = materialNeededForProductList.Where(a => a.ProductId == product.ProductId).FirstOrDefault();
                    var materialInformationList = materialList.MaterialInformationList;
                    foreach (var material in materialInformationList)
                    {
                        int? quanityInStock = materialInventoryList.Where(a => a.MaterialId == material.MaterialId).FirstOrDefault()?.Quantity;
                        var materialInformationCheck = materialInformationChecks.FirstOrDefault(a => a.MaterialId == material.MaterialId);
                        decimal? materialQuantityAddMore = 0;
                        List<OptionInformationOfProduct> optionsOfProduct = new List<OptionInformationOfProduct>();

                        bool isMaterialOption = materialList.OptionLevelInformationList.Any(a => a.MaterialId == material.MaterialId &&
                        product.OptionInformationOfProducts.Select(e => e.OptionId).ToList().Contains(a.OptionId));
                        if (isMaterialOption)
                        {
                            optionsOfProduct = product.OptionInformationOfProducts;
                            decimal? quotaOfOption = 0;
                            decimal? optionQuantity = 0;
                            foreach (var option in optionsOfProduct)
                            {
                                var optionLevel = materialList.OptionLevelInformationList
                                    .Where(a => a.OptionId == option.OptionId && a.OptionLevelId == option.OptionLevelId && a.MaterialId == material.MaterialId).FirstOrDefault();
                                if (optionLevel != null)
                                {
                                    quotaOfOption = optionLevel.Quota;
                                    optionQuantity = option.Quantity;
                                    break;
                                }
                            }
                            materialQuantityAddMore = ((material.QuantityNeeded * quotaOfOption) / 100) * optionQuantity;
                            if (materialInformationCheck != null)
                                materialQuantityAddMore += materialInformationCheck.TotalAmountNeeded;

                            if (materialQuantityAddMore > quanityInStock)
                            {
                                bool productExist = request.ProductList.Any(a => a.ProductId == product.ProductId);
                                bool productExistFromResponse = response.ProductInformationListResponse.Any(a => a.ProductId == product.ProductId);
                                if (productExist && !productExistFromResponse)
                                {
                                    response.ProductInformationListResponse.Add(new CheckAddProductForOrderResponse.ProductInformationResponse()
                                    {
                                        ProductId = product.ProductId,
                                        ProductName = materialList.ProductName
                                    });
                                }
                            }
                        }
                        else
                        {
                            materialQuantityAddMore = material.QuantityNeeded * product.Quantity;
                            if (materialInformationCheck != null)
                                materialQuantityAddMore += materialInformationCheck.TotalAmountNeeded;

                            if (materialQuantityAddMore > quanityInStock)
                            {
                                bool productExist = request.ProductList.Any(a => a.ProductId == product.ProductId);
                                bool productExistFromResponse = response.ProductInformationListResponse.Any(a => a.ProductId == product.ProductId);
                                if (productExist && !productExistFromResponse)
                                {
                                    response.ProductInformationListResponse.Add(new CheckAddProductForOrderResponse.ProductInformationResponse()
                                    {
                                        ProductId = product.ProductId,
                                        ProductName = materialList.ProductName
                                    });
                                }
                            }
                        }

                        if (materialInformationCheck != null)
                            materialInformationCheck.TotalAmountNeeded = materialQuantityAddMore;
                        else
                        {
                            materialInformationChecks.Add(new MaterialInformationCheckModel()
                            {
                                MaterialId = material.MaterialId,
                                TotalAmountNeeded = materialQuantityAddMore
                            });
                        }
                    }
                }
            }

            return response;
        }

        private List<ProductAddIntoOrderModel> MergeProductFromCartItems(List<ProductCartItemModel> cartItems)
        {
            List<ProductAddIntoOrderModel> productList = new List<ProductAddIntoOrderModel>();
            foreach (ProductCartItemModel cartItem in cartItems)
            {
                if (cartItem.IsCombo)
                {
                    var comboItems = cartItem.Combo.ComboItems;
                    foreach (var comboItem in comboItems)
                    {
                        ProductAddIntoOrderModel productCombo = new ProductAddIntoOrderModel();
                        productCombo.ProductId = comboItem.ProductId;
                        productCombo.ProductPriceId = comboItem.ProductPriceId;
                        productCombo.Quantity = cartItem.Quantity;
                        productCombo.OptionInformationOfProducts = new List<OptionInformationOfProduct>();
                        var optionOfProductComboList = comboItem.Options;
                        foreach (var option in optionOfProductComboList)
                        {
                            OptionInformationOfProduct optionItem = new OptionInformationOfProduct()
                            {
                                OptionId = option.OptionId,
                                OptionLevelId = option.OptionLevelId,
                                Quantity = productCombo.Quantity
                            };

                            productCombo.OptionInformationOfProducts.Add(optionItem);
                        }
                        productList.Add(productCombo);

                        var toppingOfProductList = cartItem.Toppings;
                        if (toppingOfProductList != null && toppingOfProductList.Count() > 0)
                        {
                            foreach (var topping in toppingOfProductList)
                            {
                                ProductAddIntoOrderModel productComboIsTopping = new ProductAddIntoOrderModel();
                                productComboIsTopping.ProductId = topping.ToppingId;
                                productComboIsTopping.Quantity = topping.Quantity;

                                productList.Add(productComboIsTopping);
                            }
                        }
                    }
                }
                else
                {
                    ProductAddIntoOrderModel product = new ProductAddIntoOrderModel();
                    product.ProductId = cartItem.ProductId;
                    product.ProductPriceId = cartItem.ProductPriceId;
                    product.Quantity = cartItem.Quantity;
                    product.OrderId = cartItem.OrderId;
                    product.OrderItemId = cartItem.OrderItemId;
                    product.OptionInformationOfProducts = new List<OptionInformationOfProduct>();
                    var optionOfProductList = cartItem.Options;
                    if (optionOfProductList != null && optionOfProductList.Count() > 0)
                    {
                        foreach (var option in optionOfProductList)
                        {
                            OptionInformationOfProduct optionItem = new OptionInformationOfProduct()
                            {
                                OptionId = option.OptionId,
                                OptionLevelId = option.OptionLevelId,
                                Quantity = product.Quantity
                            };

                            product.OptionInformationOfProducts.Add(optionItem);
                        }
                    }
                    productList.Add(product);

                    var toppingOfProductList = cartItem.Toppings;
                    if (toppingOfProductList != null && toppingOfProductList.Count() > 0)
                    {
                        foreach (var topping in toppingOfProductList)
                        {
                            ProductAddIntoOrderModel productIsTopping = new ProductAddIntoOrderModel();
                            productIsTopping.ProductId = topping.ToppingId;
                            productIsTopping.Quantity = topping.Quantity;

                            productList.Add(productIsTopping);
                        }
                    }
                }
            }

            return productList;
        }
    }
}