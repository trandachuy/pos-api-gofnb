using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductIncludedProductUnit : IRequest<GetAllProductIncludedProductUnitResponse>
    {
    }

    public class GetAllProductIncludedProductUnitResponse
    {
        public IEnumerable<ProductProductCategoryModel> ProductsToAddModel { get; set; }
    }

    public class GetAllProductIncludedProductUnitRequestHandle : IRequestHandler<GetAllProductIncludedProductUnit, GetAllProductIncludedProductUnitResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductIncludedProductUnitRequestHandle(IUnitOfWork unitOfWork, IMapper mapper, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<GetAllProductIncludedProductUnitResponse> Handle(GetAllProductIncludedProductUnit request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var productResponse = _unitOfWork.Products
                .GetAllProductIncludedProductUnit(loggedUser.StoreId.Value)
                .Where(x => x.StatusId == (int)EnumStatus.Active && !x.IsTopping && x.IsActive == true);

            var responseData = await productResponse
                .ToListAsync(cancellationToken: cancellationToken);

            var productsToAddModel = _mapper.Map<List<ProductProductCategoryModel>>(responseData);
            var response = new GetAllProductIncludedProductUnitResponse()
            {
                ProductsToAddModel = productsToAddModel
            };

            return response;
        }
    }
}
