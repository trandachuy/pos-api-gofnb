using System;
using MediatR;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class CreateProductCategoryRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public int Priority { get; set; }

        public bool IsDisplayAllBranches { get; set; } = true;

        public List<ProductSelectedModel> Products { get; set; }

        public List<Guid> StoreBranchIds { get; set; }

        public class ProductSelectedModel
        {
            public Guid Id { get; set; }

            public int Position { get; set; }
        }
    }

    public class CreateProductCategoryRequestHandler : IRequestHandler<CreateProductCategoryRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateProductCategoryRequestHandler(
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

        public async Task<bool> Handle(CreateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            var productCategoryNameExisted = await _unitOfWork.ProductCategories.GetProductCategoryByNameInStoreAsync(request.Name, loggedUser.StoreId.Value);
            ThrowError.Against(productCategoryNameExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}", "Product category name has already existed" },
            });

            var storeConfig = await _unitOfWork.StoreConfigs.GetStoreConfigByStoreIdAsync(loggedUser.StoreId.Value);

            var productCategory = CreateProductCategory(
                request,
                storeConfig.CurrentMaxProductCategoryCode.GetCode(StoreConfigConstants.PRODUCT_CATEGORY_CODE), 
                loggedUser.StoreId.Value);

            _unitOfWork.ProductCategories.Add(productCategory);
            await _unitOfWork.SaveChangesAsync();

            // update store configure
            await _unitOfWork.StoreConfigs.UpdateStoreConfigAsync(storeConfig, StoreConfigConstants.PRODUCT_CATEGORY_CODE);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.ProductCategory,
                ActionType = EnumActionType.Created,
                ObjectId = productCategory.Id,
                ObjectName = productCategory.Name.ToString()
            });

            return true;
        }

        private static void RequestValidation(CreateProductCategoryRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter product category name");

            ThrowError.Against(!request.IsDisplayAllBranches && (request.StoreBranchIds == null || request.StoreBranchIds.Count == 0), new JObject()
            {
                { $"{nameof(request.StoreBranchIds)}", "Please select branch" },
            });
        }

        private ProductCategory CreateProductCategory(CreateProductCategoryRequest request, string code, Guid storeId)
        {
            var newProductCategory = new ProductCategory()
            {
                Code = code,
                Name = request.Name,
                Priority = request.Priority,
                StoreId = storeId,
                IsDisplayAllBranches = request.IsDisplayAllBranches,
                ProductProductCategories = new List<ProductProductCategory>(),
                StoreBranchProductCategories = new List<StoreBranchProductCategory>(),
            };

            /// Remove all product - category old data from sub-table ProductProductCategory
            var productIds = request.Products.Select(p => p.Id);
            var productProductCategories = _unitOfWork.ProductProductCategories.Find(p => p.StoreId == storeId && productIds.Any(pid => pid == p.ProductId));
            _unitOfWork.ProductProductCategories.RemoveRange(productProductCategories);

            /// Save new product - product category to sub-table
            if (request.Products != null && request.Products.Any())
            {
                request.Products.ForEach(product =>
                {
                    var index = request.Products.IndexOf(product);
                    var productProductCategory = new ProductProductCategory()
                    {
                        ProductId = product.Id,
                        ProductCategoryId = newProductCategory.Id,
                        Position = index + 1,
                        StoreId = storeId,
                    };
                    newProductCategory.ProductProductCategories.Add(productProductCategory);
                });
            }

            /// Save branches if select display to specific branches
            if (request.StoreBranchIds != null && request.StoreBranchIds.Any() && !request.IsDisplayAllBranches)
            {
                request.StoreBranchIds.ForEach(storeBranchId =>
                {
                    var storeBranchProductCategory = new StoreBranchProductCategory()
                    {
                        StoreBranchId = storeBranchId,
                        ProductCategoryId = newProductCategory.Id,
                        StoreId = storeId,
                    };
                    newProductCategory.StoreBranchProductCategories.Add(storeBranchProductCategory);
                });
            }

            return newProductCategory;
        }
    }
}
