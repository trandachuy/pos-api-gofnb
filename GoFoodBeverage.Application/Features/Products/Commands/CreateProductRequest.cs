using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class CreateProductRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public List<MaterialModel> Materials { get; set; }

        public Guid UnitId { get; set; }

        public Guid? TaxId { get; set; }

        public bool IsTopping { get; set; }

        public List<PriceDto> Prices { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public class PriceDto
        {
            public string Name { get; set; }

            public decimal Price { get; set; }

            public List<MaterialModel> Materials { get; set; }

            public class MaterialModel
            {
                public Guid MaterialId { get; set; }

                public int Quantity { get; set; }

                public decimal UnitCost { get; set; }
            }
        }

        public List<Guid> OptionIds { get; set; }

        public class MaterialModel
        {
            public Guid MaterialId { get; set; }

            public int Quantity { get; set; }

            public decimal UnitCost { get; set; }
        }

        public List<Guid> PlatformIds { get; set; }

        public List<Guid> ProductToppingIds { get; set; }
    }
    public class CreateMaterialRequestHandler : IRequestHandler<CreateProductRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateMaterialRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            var productNameExisted = await _unitOfWork.Products.GetProductActiveByNameInStoreAsync(request.Name.ToLower(), loggedUser.StoreId.Value);
            ThrowError.Against(productNameExisted != null, "Product name has already existed");
            var productCategoryIdExisted = await _unitOfWork.ProductCategories.CheckExistProductCategoryAsync(request.ProductCategoryId, loggedUser.StoreId);

            var product = CreateProduct(request, loggedUser.StoreId.Value, loggedUser.AccountId.Value, productCategoryIdExisted);

            await _unitOfWork.Products.AddAsync(product);

            if (request.IsTopping == false && request.ProductToppingIds != null && request.ProductToppingIds.Any())
            {
                var productToppings = CreateProductTopping(product.Id, request.ProductToppingIds, loggedUser.StoreId.Value);
                await _unitOfWork.ProductToppings.AddRangeAsync(productToppings);
            }

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Product,
                ActionType = EnumActionType.Created,
                ObjectId = product.Id,
                ObjectName = product.Code.ToString(),
                ObjectThumbnail = product.Thumbnail
            }, cancellationToken);

            return true;
        }

        private static void RequestValidation(CreateProductRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter product name");
            ThrowError.Against(request.Price < 0 && !request.Prices.Any(), "Please enter product price");
        }

        private static Product CreateProduct(CreateProductRequest request, Guid storeId, Guid accountId, bool productCategoryIdExisted)
        {
            var prices = new List<ProductPrice>();
            if (request.Prices != null && request.Prices.Any())
            {
                foreach (var price in request.Prices)
                {
                    var productPrice = new ProductPrice()
                    {
                        PriceName = price.Name,
                        PriceValue = price.Price,
                        StoreId = storeId,
                        ProductPriceMaterials = new List<ProductPriceMaterial>()
                    };

                    foreach (var material in price.Materials)
                    {
                        var productPriceMaterial = new ProductPriceMaterial()
                        {
                            ProductPriceId = productPrice.Id,
                            MaterialId = material.MaterialId,
                            StoreId = storeId,
                            Quantity = material.Quantity,
                        };

                        productPrice.ProductPriceMaterials.Add(productPriceMaterial);
                    }

                    prices.Add(productPrice);
                }
            }
            else
            {
                var productPrice = new ProductPrice()
                {
                    PriceValue = request.Price,
                    StoreId = storeId,
                    ProductPriceMaterials = new List<ProductPriceMaterial>()
                };

                foreach (var material in request.Materials)
                {
                    var productPriceMaterial = new ProductPriceMaterial()
                    {
                        ProductPriceId = productPrice.Id,
                        MaterialId = material.MaterialId,
                        Quantity = material.Quantity,
                        StoreId = storeId,
                    };

                    productPrice.ProductPriceMaterials.Add(productPriceMaterial);
                }

                prices.Add(productPrice);
            }

            var newProduct = new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Thumbnail = request.Image,
                StoreId = storeId,
                ProductPrices = prices,
                UnitId = request.UnitId,
                TaxId = request.TaxId,
                StatusId = (int)EnumStatus.Active,
                CreatedUser = accountId,
                ProductOptions = new List<ProductOption>(),
                IsTopping = request.IsTopping,
                ProductPlatforms = new List<ProductPlatform>(),
                IsActive = true
            };

            if (!request.IsTopping)
            {
                if (request.OptionIds.Any())
                {
                    request.OptionIds.ForEach(optionId =>
                    {
                        var productOption = new ProductOption()
                        {
                            ProductId = newProduct.Id,
                            OptionId = optionId,
                            StoreId = storeId,
                        };
                        newProduct.ProductOptions.Add(productOption);
                    });
                }

                if (productCategoryIdExisted)
                {
                    var productProductCategory = new List<ProductProductCategory>
                    {
                        new ProductProductCategory()
                        {
                           ProductCategoryId = request.ProductCategoryId.Value,
                           ProductId = newProduct.Id,
                           StoreId = storeId,
                           Position = 1
                        }
                    };
                    newProduct.ProductProductCategories = productProductCategory;
                }
            }

            var productChannels = new List<ProductChannel>
            {
                new ProductChannel()
                {
                    ChannelId = EnumChannel.InStore.ToGuid(),
                    ProductId = newProduct.Id,
                    StoreId = storeId,
                }
            };

            newProduct.ProductChannels = productChannels;

            /// Save product platforms
            if (request.PlatformIds != null && request.PlatformIds.Any())
            {
                request.PlatformIds.ForEach(platformId =>
                {
                    var productPlatform = new ProductPlatform()
                    {
                        PlatformId = platformId,
                        ProductId = newProduct.Id,
                        StoreId = storeId,
                    };
                    newProduct.ProductPlatforms.Add(productPlatform);
                });
            }

            return newProduct;
        }

        private List<ProductTopping> CreateProductTopping(Guid productId, List<Guid> productToppingIds, Guid? storeId)
        {
            List<ProductTopping> productToppings = new List<ProductTopping>();
            foreach (var productToppingId in productToppingIds)
            {
                ProductTopping productTopping = new ProductTopping
                {
                    ToppingId = productToppingId,
                    ProductId = productId,
                    StoreId = storeId
                };

                productToppings.Add(productTopping);
            }

            return productToppings;
        }
    }
}
