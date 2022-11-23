using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Products.Queries
{
    public class GetAllToppingByProductIdRequest : IRequest<GetAllToppingByProductIdResponse>
    {
        public Guid ProductId { get; set; }
    }

    public class GetAllToppingByProductIdResponse
    {
        public List<ProductToppingModel> ProductToppings { get; set; }
    }

    public class GetAllToppingByProductIdRequestHandler : IRequestHandler<GetAllToppingByProductIdRequest, GetAllToppingByProductIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetAllToppingByProductIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetAllToppingByProductIdResponse> Handle(GetAllToppingByProductIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var toppingIds = _unitOfWork.ProductToppings
               .GetToppingsByProductId(request.ProductId, loggedUser.StoreId)
               .Select(x => x.ToppingId);

            var productToppings = await _unitOfWork.Products
                .GetAll()
                .Where(t => t.StoreId == loggedUser.StoreId && t.IsTopping && t.StatusId == (int)EnumStatus.Active && toppingIds.Contains(t.Id))
                .Include(t => t.ProductPrices)
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .Select(t => new ProductToppingModel()
                {
                    ToppingId = t.Id,
                    Name = t.Name,
                    PriceValue = t.ProductPrices.FirstOrDefault().PriceValue,
                })
                .ToListAsync(cancellationToken: cancellationToken);

            return new GetAllToppingByProductIdResponse()
            {
                ProductToppings = productToppings
            };
        }
    }
}
