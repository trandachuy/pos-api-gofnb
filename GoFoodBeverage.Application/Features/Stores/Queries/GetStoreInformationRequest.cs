using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using System.Linq;
using GoFoodBeverage.Common.Exceptions;
using System;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetStoreInformationRequest : IRequest<GetStoreInformationResponse>
    {
    }

    public class GetStoreInformationResponse
    {
        public string StoreName { get; set; }

        public string Logo { get; set; }

        public Guid CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencySymbol { get; set; }

    }

    public class GetStoreInformationRequestHandler : IRequestHandler<GetStoreInformationRequest, GetStoreInformationResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetStoreInformationRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetStoreInformationResponse> Handle(GetStoreInformationRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var storeInfo = await _unitOfWork.Stores
                .Find(store => store.Id == loggedUser.StoreId)
                .Include(store => store.Currency)
                .AsNoTracking()
                .Select(store => new GetStoreInformationResponse() {
                    StoreName = store.Title,
                    Logo = store.Logo,
                    CurrencyId = store.CurrencyId,
                    CurrencyName = store.Currency.CurrencyName,
                    CurrencyCode = store.Currency.Code,
                    CurrencySymbol = store.Currency.Symbol,
                })
                .FirstOrDefaultAsync();

            ThrowError.BadRequestAgainstNull(storeInfo, "Can not find store information.");

            return storeInfo;
        }
    }
}
