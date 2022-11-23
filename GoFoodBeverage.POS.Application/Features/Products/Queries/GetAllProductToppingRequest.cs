using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Products.Queries
{
    public class GetAllProductToppingRequest : IRequest<GetAllProductToppingResponse>
    {
    }

    public class GetAllProductToppingResponse
    {
        public List<ProductToppingModel> ProductToppings { get; set; }
    }

    public class GetAllProductToppingRequestHandler : IRequestHandler<GetAllProductToppingRequest, GetAllProductToppingResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetAllProductToppingRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetAllProductToppingResponse> Handle(GetAllProductToppingRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var listProductToppings = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var productToppingResponse = new List<ProductToppingModel>();

            listProductToppings.ForEach(t =>
            {
                var productTopping = new ProductToppingModel
                {
                    ToppingId = t.Id,
                    Name = t.Name,
                    PriceValue = t.ProductPrices.FirstOrDefault().PriceValue,
                };

                productToppingResponse.Add(productTopping);
            });

            return new GetAllProductToppingResponse()
            {
                ProductToppings = productToppingResponse
            };
        }
    }
}
