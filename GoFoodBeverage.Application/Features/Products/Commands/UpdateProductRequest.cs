using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Product;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class UpdateProductRequest : IRequest<bool>
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public Guid UnitId { get; set; }

        public Guid? TaxId { get; set; }

        public List<PriceDto> Prices { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public class PriceDto
        {
            public string Name { get; set; }

            public decimal Price { get; set; }

            public Guid? Id { get; set; }

            public List<MaterialDto> Materials { get; set; }
        }

        public List<MaterialDto> Materials { get; set; }

        public List<Guid> OptionIds { get; set; }

        public bool IsTopping { get; set; }

        public class MaterialDto
        {
            public Guid MaterialId { get; set; }

            public int Quantity { get; set; }

            public decimal UnitCost { get; set; }
        }

        public List<Guid> PlatformIds { get; set; }
        public List<Guid> ProductToppingIds { get; set; }
    }

    public class EditProductRequestHandler : IRequestHandler<UpdateProductRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public EditProductRequestHandler(
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

        public async Task<bool> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            // Check product name duplicate before handle update
            var productNameExisted = await _unitOfWork.Products.CheckEditProductByNameInStoreAsync(request.ProductId, request.Name, loggedUser.StoreId.Value);
            ThrowError.Against(productNameExisted, "Product name has already existed");

            // Handle update product
            var updateProductModel = MappingRequestToUpdateModel(request);
            var updateProductResult = await _unitOfWork.Products.UpdateProductAsync(loggedUser.StoreId.Value, updateProductModel);

            #region Handle update product topping
            var currentProductToppings = await _unitOfWork.ProductToppings
                .Find(x => x.StoreId == loggedUser.StoreId && x.ProductId == request.ProductId).ToListAsync(cancellationToken: cancellationToken);
            var removeProductToppings = await _unitOfWork.ProductToppings
                .Find(x => x.StoreId == loggedUser.StoreId && x.ProductId == request.ProductId).ToListAsync(cancellationToken: cancellationToken);
            if (request.IsTopping)
            {
                if (removeProductToppings != null && removeProductToppings.Any())
                {
                    _unitOfWork.ProductToppings.RemoveRange(removeProductToppings);
                }
            }
            else
            {
                if (request.ProductToppingIds != null && request.ProductToppingIds.Any())
                {
                    var newProductToppings = CreateProductTopping(request.ProductId, request.ProductToppingIds, loggedUser.StoreId.Value);
                    removeProductToppings = currentProductToppings.Where(x => !newProductToppings.Any(n => n.ToppingId == x.ToppingId)).ToList();
                    if (removeProductToppings != null && removeProductToppings.Any())
                    {
                        _unitOfWork.ProductToppings.RemoveRange(removeProductToppings);
                    }

                    newProductToppings = newProductToppings.Where(x => !removeProductToppings.Any(r => r.ToppingId == x.ToppingId) &&
                                                                       !currentProductToppings.Any(r => r.ToppingId == x.ToppingId))
                                                           .ToList();

                    if (newProductToppings != null && newProductToppings.Any())
                    {
                        _unitOfWork.ProductToppings.AddRange(newProductToppings);
                    }
                }
                else
                {
                    if (removeProductToppings != null && removeProductToppings.Any())
                    {
                        _unitOfWork.ProductToppings.RemoveRange(removeProductToppings);
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
            #endregion

            // user logging
            await _userActivityService.LogAsync(request);

            // staff logging
            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Product,
                ActionType = EnumActionType.Edited,
                ObjectId = request.ProductId,
                ObjectName = $"{updateProductResult.Item2}",
                ObjectThumbnail = updateProductResult.Item3
            }, cancellationToken);

            return true;
        }

        private static void RequestValidation(UpdateProductRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter product name");
            ThrowError.Against(request.Price < 0 && !request.Prices.Any(), "Please enter product price");
        }

        private static UpdateProductModel MappingRequestToUpdateModel(UpdateProductRequest request)
        {
            var updateProductModel = new UpdateProductModel()
            {
                ProductId = request.ProductId,
                Name = request.Name,
                Description = request.Description,
                Image = request.Image,
                Price = request.Price,
                UnitId = request.UnitId,
                TaxId = request.TaxId,
                Prices = new List<UpdateProductModel.PriceDto>(),
                ProductCategoryId = request.ProductCategoryId,
                Materials = new List<UpdateProductModel.MaterialDto>(),
                OptionIds = request.OptionIds,
                IsTopping = request.IsTopping,
                PlatformIds = request.PlatformIds,
            };

            if (request.Prices != null)
            {
                foreach (var price in request.Prices)
                {
                    var updatePrice = new UpdateProductModel.PriceDto()
                    {
                        Id = price.Id,
                        Name = price.Name,
                        Price = price.Price,
                        Materials = new List<UpdateProductModel.MaterialDto>()
                    };

                    foreach (var material in price.Materials)
                    {
                        updatePrice.Materials.Add(new UpdateProductModel.MaterialDto()
                        {
                            MaterialId = material.MaterialId,
                            Quantity = material.Quantity,
                            UnitCost = material.UnitCost,
                        });
                    }

                    updateProductModel.Prices.Add(updatePrice);
                }
            }

            if (request.Materials != null)
            {
                foreach (var material in request.Materials)
                {
                    updateProductModel.Materials.Add(new UpdateProductModel.MaterialDto()
                    {
                        MaterialId = material.MaterialId,
                        Quantity = material.Quantity,
                        UnitCost = material.UnitCost,
                    });
                }
            }

            return updateProductModel;
        }

        private static List<ProductTopping> CreateProductTopping(Guid productId, IEnumerable<Guid> productToppingIds, Guid? storeId)
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
