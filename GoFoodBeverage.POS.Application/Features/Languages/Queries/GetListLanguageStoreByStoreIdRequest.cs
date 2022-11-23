using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Language;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.POS.Application.Features.Languages.Queries
{
    public class GetListLanguageStoreByStoreIdRequest : IRequest<GetListLanguageStoreByStoreIdResponse>
    {
    }

    public class GetListLanguageStoreByStoreIdResponse
    {
        public IEnumerable<LanguageStoreDtoModel> LanguageStore { get; set; }
    }

    public class GetListLanguageStoreByStoreIdRequestHandler : IRequestHandler<GetListLanguageStoreByStoreIdRequest, GetListLanguageStoreByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetListLanguageStoreByStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetListLanguageStoreByStoreIdResponse> Handle(GetListLanguageStoreByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggerUser.StoreId.HasValue, "Cannot find store information");

            var languageStore = await _unitOfWork.LanguageStores
                .GetLanguageStoreByStoreId(loggerUser.StoreId);
            var languageStoreDtoModel = _mapper.Map<List<LanguageStoreDtoModel>>(languageStore);
            var response = new GetListLanguageStoreByStoreIdResponse()
            {
                LanguageStore = languageStoreDtoModel
            };

            return response;
        }
    }
}
