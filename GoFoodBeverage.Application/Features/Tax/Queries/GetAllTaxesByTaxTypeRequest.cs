using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Tax;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Taxes.Queries
{
    public class GetAllTaxesByTaxTypeRequest : IRequest<GetAllTaxesByTaxTypeResponse>
    {
        public int TaxType { get; set; }
    }

    public class GetAllTaxesByTaxTypeResponse
    {
        public IEnumerable<TaxTypeModel> TaxesByType { get; set; }
    }

    public class GetAllTaxTypeRequestHandler : IRequestHandler<GetAllTaxesByTaxTypeRequest, GetAllTaxesByTaxTypeResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllTaxTypeRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllTaxesByTaxTypeResponse> Handle(GetAllTaxesByTaxTypeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var listTaxByTaxType = await _unitOfWork.Taxes
                    .GetAllTaxInStore(loggedUser.StoreId.Value)
                    .Where(t => t.TaxTypeId == (int)EnumTaxType.SellingTax)
                    .ProjectTo<TaxTypeModel>(_mapperConfiguration)
                    .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllTaxesByTaxTypeResponse()
            {
                TaxesByType = listTaxByTaxType
            };

            return response;
        }
    }
}
