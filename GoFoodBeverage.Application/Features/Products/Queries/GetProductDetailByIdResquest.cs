using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Product;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using System;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetProductDetailByIdResquest : IRequest<GetProductDetailByIdResponse>
    {
        public Guid StoreId { get; set; }

        public Guid ProductId { get; set; }
    }

    public class GetProductDetailByIdResponse
    {
        public ProductDetailAppOrderModel Product { get; set; }
    }

    public class GetProductDetailByIdResquestHandler : IRequestHandler<GetProductDetailByIdResquest, GetProductDetailByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductDetailByIdResquestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task<GetProductDetailByIdResponse> Handle(GetProductDetailByIdResquest request, CancellationToken cancellationToken)
        {
            var product = _unitOfWork.Products
                                   .GetProductByIdInStore(request.StoreId, request.ProductId)
                                   .Where(p => p.IsActive == true)
                                   .Include(p => p.ProductPrices.OrderBy(x => x.CreatedTime))
                                   .Include(p => p.ProductOptions).ThenInclude(o => o.Option).ThenInclude(ol => ol.OptionLevel)
                                   .FirstOrDefault();

            var productDetail = _mapper.Map<ProductDetailAppOrderModel>(product);

            var allProductToppingInStore = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(request.StoreId)
                .Select(p => new ProductToppingModel { Id = p.Id, Name = p.Name, Price = p.ProductPrices.FirstOrDefault().PriceValue, Thumbnail = p.Thumbnail })
                .ToListAsync();

            var productToppingIds = await _unitOfWork.ProductToppings.GetAll().Where(pt => pt.ProductId == request.ProductId)
                       .Select(pt => pt.ToppingId)
                       .ToListAsync();
            var productToppingsModel = allProductToppingInStore.Where(p => productToppingIds.Contains(p.Id)).ToList();

            productDetail.ProductToppings = productToppingsModel;

            var response = new GetProductDetailByIdResponse()
            {
                Product = productDetail
            };

            return response;
        }
    }
}
