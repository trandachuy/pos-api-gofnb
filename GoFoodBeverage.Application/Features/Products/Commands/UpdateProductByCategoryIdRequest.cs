using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using MediatR;
using MoreLinq;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GoFoodBeverage.Models.Product;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class UpdateProductByCategoryIdRequest : IRequest<bool>
    {
        public Guid ProductCategoryId { get; set; }

        public List<ProductProductCategoryModel> ProductByCategoryIdModel { get; set; }
    }

    public class UpdateProductByCategoryIdHandler : IRequestHandler<UpdateProductByCategoryIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateProductByCategoryIdHandler(IUnitOfWork unitOfWork, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdateProductByCategoryIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            using var productCategoryTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var productIds = request.ProductByCategoryIdModel.Select(p => p.ProductId);
                var oldProductProductCategories = _unitOfWork.ProductProductCategories
                    .Find(p => p.StoreId == loggedUser.StoreId && (p.ProductCategoryId == request.ProductCategoryId || productIds.Any(pid => pid == p.ProductId)));
                _unitOfWork.ProductProductCategories.RemoveRange(oldProductProductCategories);

                var submitProductProductCategories = MappingRequestToUpdateModel(request.ProductCategoryId, request.ProductByCategoryIdModel, loggedUser.StoreId.Value);
                _unitOfWork.ProductProductCategories.AddRange(submitProductProductCategories);

                await _unitOfWork.SaveChangesAsync();
                await productCategoryTransaction.CommitAsync(cancellationToken);
                
                return true;
            }
            catch (Exception ex)
            {
                await productCategoryTransaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        private static List<ProductProductCategory> MappingRequestToUpdateModel(Guid productCategoryId, List<ProductProductCategoryModel> products, Guid? storeId)
        {
            List<ProductProductCategory> result = new();
            products.ForEach(product =>
            {
                var index = products.IndexOf(product);
                ProductProductCategory productProductCategory = new()
                {
                    ProductCategoryId = productCategoryId,
                    ProductId = product.ProductId,
                    Position = index + 1,
                    StoreId = storeId
                };

                result.Add(productProductCategory);
            });

            return result;
        }
    }
}
