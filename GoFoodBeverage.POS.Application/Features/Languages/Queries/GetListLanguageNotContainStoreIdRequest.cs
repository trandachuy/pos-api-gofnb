using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Language;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.POS.Application.Features.Languages.Queries
{
    public class GetListLanguageNotContainStoreIdRequest : IRequest<GetListLanguageNotContainStoreIdResponse>
    {

    }

    public class GetListLanguageNotContainStoreIdResponse
    {
        public IEnumerable<LanguageModel> Languages { get; set; }
    }

    public class GetListLanguageNotContainStoreIdRequestHandler : IRequestHandler<GetListLanguageNotContainStoreIdRequest, GetListLanguageNotContainStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IMapper _mapper;

        public GetListLanguageNotContainStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
            _mapper = mapper;
        }

        public async Task<GetListLanguageNotContainStoreIdResponse> Handle(GetListLanguageNotContainStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var languages = await _unitOfWork.Languages
                .GetAllLanguagesNotApplyInStore(loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<LanguageModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            var languagesModel = _mapper.Map<IEnumerable<LanguageModel>>(languages);
            var response = new GetListLanguageNotContainStoreIdResponse()
            {
                Languages = languagesModel
            };

            return response;
        }
    }
}
