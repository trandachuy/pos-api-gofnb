using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Area;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Models.Tax;

namespace GoFoodBeverage.Application.Features.Taxes.Queries
{
    public class GetTaxByIdRequest : IRequest<GetTaxByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetTaxByIdResponse
    {
        public TaxModel Tax { get; set; }
    }

    public class GetTaxByIdRequestHandler : IRequestHandler<GetTaxByIdRequest, GetTaxByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetTaxByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetTaxByIdResponse> Handle(GetTaxByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var tax = await _unitOfWork.Taxes
                .GetTaxById(request.Id, loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<TaxModel>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(tax == null, "Cannot find tax information");

            var response = new GetTaxByIdResponse()
            {
                Tax = tax
            };
            return response;
        }
    }
}
