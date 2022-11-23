using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Language.Dto;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Languages.Queries
{
    public class GetListLanguageByStoreIdAndIsPublishRequest : IRequest<GetListLanguageByStoreIdAndIsPublishResponse>
    {

    }

    public class GetListLanguageByStoreIdAndIsPublishResponse
    {
        public List<LanguageStoreDto> Languages { get; set; }
    }

    public class GetListLanguageByStoreIdAndIsPublishRequestHandler : IRequestHandler<GetListLanguageByStoreIdAndIsPublishRequest, GetListLanguageByStoreIdAndIsPublishResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetListLanguageByStoreIdAndIsPublishRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetListLanguageByStoreIdAndIsPublishResponse> Handle(GetListLanguageByStoreIdAndIsPublishRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggerUser.StoreId.HasValue, "Cannot find store information");

            var language = await _unitOfWork.LanguageStores
                .GetLanguageByStoreIdAndIsPublish(loggerUser.StoreId.Value);

            var response = new GetListLanguageByStoreIdAndIsPublishResponse()
            {
                Languages = language
            };

            return response;
        }
    }
}
