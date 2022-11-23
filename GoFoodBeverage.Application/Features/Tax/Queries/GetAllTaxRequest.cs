using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Tax;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Tax.Queries
{
    public class GetAllTaxRequest : IRequest<GetAllTaxResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetAllTaxResponse
    {
        public IEnumerable<TaxModel> Taxes { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetAllTaxRequestHandler : IRequestHandler<GetAllTaxRequest, GetAllTaxResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;


        public GetAllTaxRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllTaxResponse> Handle(GetAllTaxRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var taxes = await _unitOfWork.Taxes.GetAllTaxInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ProjectTo<TaxModel>(_mapperConfiguration)
                .ToPaginationAsync(request.PageNumber, request.PageSize);

            var response = new GetAllTaxResponse()
            {
                Taxes = taxes.Result,
                PageNumber = request.PageNumber,
                Total = taxes.Total,
            };
            return response;
        }
    }

}
