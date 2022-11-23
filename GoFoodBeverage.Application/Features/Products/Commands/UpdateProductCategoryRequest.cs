using System;
using MediatR;
using MoreLinq;
using System.Linq;
using System.Threading;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class UpdateProductCategoryRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

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

    public class UpdateProductCategoryRequestHandler : IRequestHandler<UpdateProductCategoryRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateProductCategoryRequestHandler(
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

        public async Task<bool> Handle(UpdateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var productCategory = await _unitOfWork.ProductCategories.GetProductCategoryDetailByIdAsync(request.Id, loggedUser.StoreId);
            ThrowError.Against(productCategory == null, "Cannot find product category information");

            var productCategoryNameExisted = await _unitOfWork.ProductCategories.CheckExistProductCategoryNameInStoreAsync(request.Id, request.Name, loggedUser.StoreId.Value);
            ThrowError.Against(productCategoryNameExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}", "Product category name has already existed" },
            });

            RequestValidation(request);

            var modifiedProductCategory = await UpdateProductCategoryAsync(productCategory, request);
            await _unitOfWork.ProductCategories.UpdateAsync(modifiedProductCategory);

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.ProductCategory,
                ActionType = EnumActionType.Edited,
                ObjectId = modifiedProductCategory.Id,
                ObjectName = modifiedProductCategory.Name.ToString()
            });

            return true;
        }

        private static void RequestValidation(UpdateProductCategoryRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter product category name");

            ThrowError.Against(!request.IsDisplayAllBranches && (request.StoreBranchIds == null || request.StoreBranchIds.Count == 0), new JObject()
            {
                { $"{nameof(request.StoreBranchIds)}", "Please select branch" },
            });
        }

        public async Task<ProductCategory> UpdateProductCategoryAsync(ProductCategory currentProductCategory, UpdateProductCategoryRequest request)
        {
            currentProductCategory.Name = request.Name;
            currentProductCategory.Priority = request.Priority;
            currentProductCategory.IsDisplayAllBranches = request.IsDisplayAllBranches;

            #region Update store branch - product category
            var currentStoreBranchProductCategories = currentProductCategory.StoreBranchProductCategories.ToList();
            var newStoreBranchProductCategories = new List<StoreBranchProductCategory>();

            if (!request.IsDisplayAllBranches)
            {
                if (request.StoreBranchIds != null && request.StoreBranchIds.Any())
                {
                    /// Delete 
                    var deleteItems = currentStoreBranchProductCategories
                        .Where(x => !request.StoreBranchIds.Contains(x.StoreBranchId));
                    await _unitOfWork.StoreBranchProductCategories.RemoveRangeAsync(deleteItems);

                    /// Add new
                    request.StoreBranchIds.ForEach(branchId =>
                    {
                        var storeBranchProductCategory = currentStoreBranchProductCategories
                            .FirstOrDefault(x => x.StoreBranchId == branchId);
                        if (storeBranchProductCategory == null)
                        {
                            var newBranch = new StoreBranchProductCategory()
                            {
                                ProductCategoryId = currentProductCategory.Id,
                                StoreBranchId = branchId,
                                StoreId = currentProductCategory.StoreId,
                            };
                            newStoreBranchProductCategories.Add(newBranch);
                        }
                    });
                    await _unitOfWork.StoreBranchProductCategories.AddRangeAsync(newStoreBranchProductCategories);
                }
                else
                {
                    await _unitOfWork.StoreBranchProductCategories.RemoveRangeAsync(currentStoreBranchProductCategories);
                }
            }
            else
            {
                await _unitOfWork.StoreBranchProductCategories.RemoveRangeAsync(currentStoreBranchProductCategories);
            }
            #endregion

            #region Update product - product category

            /// Delete product - product category from sub-table
            var productIds = request.Products.Select(p => p.Id);
            var currentProductProductCategories = _unitOfWork.ProductProductCategories
                .Find(p => p.StoreId == currentProductCategory.StoreId && (p.ProductCategoryId == currentProductCategory.Id || productIds.Any(pid => pid == p.ProductId)));
            _unitOfWork.ProductProductCategories.RemoveRange(currentProductProductCategories);

            var newProductProductCategories = new List<ProductProductCategory>();

            if (request.Products != null && request.Products.Any())
            {
                /// Add new
                request.Products.ForEach(product =>
                {
                    var index = request.Products.IndexOf(product);
                    var newProduct = new ProductProductCategory()
                    {
                        ProductCategoryId = currentProductCategory.Id,
                        ProductId = product.Id,
                        Position = index + 1,
                        StoreId = currentProductCategory.StoreId,
                    };
                    newProductProductCategories.Add(newProduct);
                   
                });

                _unitOfWork.ProductProductCategories.AddRange(newProductProductCategories);
            }
            #endregion

            await _unitOfWork.SaveChangesAsync();

            return currentProductCategory;
        }
    }
}
