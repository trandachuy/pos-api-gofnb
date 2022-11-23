using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
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
    public class GetProductsByCategoryIdRequest : IRequest<GetProductsByCategoryIdResponse>
    {
        public Guid CategoryId { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetProductsByCategoryIdResponse 
    {
        public IEnumerable<ProductProductCategoryModel> ProductsByCategoryId { get; set; }
    }

    public class GetProductsByCategoryIdRequestHandle : IRequestHandler<GetProductsByCategoryIdRequest, GetProductsByCategoryIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;

        public GetProductsByCategoryIdRequestHandle(IUnitOfWork unitOfWork, IMapper mapper, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<GetProductsByCategoryIdResponse> Handle(GetProductsByCategoryIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var productProductCategoryData = _unitOfWork.ProductProductCategories
                .GetAllIncludedProductUnitByProductCategoryId(request.CategoryId, loggedUser.StoreId);

            if (!string.IsNullOrEmpty(request.KeySearch))
            {
                productProductCategoryData = productProductCategoryData
                    .Where(x => x.Product.Name.Contains(request.KeySearch));
            }
               
            var responseData = await productProductCategoryData
                .OrderBy(x => x.Position)
                .ToListAsync(cancellationToken: cancellationToken);

            var productsByCategoryId = _mapper.Map<List<ProductProductCategoryModel>>(responseData);


            var response = new GetProductsByCategoryIdResponse()
            {
                ProductsByCategoryId = productsByCategoryId
            };

            return response;
        }
    }
}
