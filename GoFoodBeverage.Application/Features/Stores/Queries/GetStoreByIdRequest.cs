using AutoMapper;
using GoFoodBeverage.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Staff;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetStoreByIdRequest : IRequest<GetStoreByIdResponse>
    {
    }

    public class GetStoreByIdResponse
    {
        public StoreModel Store { get; set; }

        public StaffModel Staff { get; set; }

        public bool IsDefaultCountry { get; set; }
    }

    public class GetStoreByIdRequestHandler : IRequestHandler<GetStoreByIdRequest, GetStoreByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;

        public GetStoreByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<GetStoreByIdResponse> Handle(GetStoreByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var store = await _unitOfWork.Stores
                .GetStoreInformationByIdAsync(loggedUser.StoreId);

            var staff = await _unitOfWork.Staffs
                .GetStaffByAccountIdAsync(store.InitialStoreAccountId);

            var storeDetail = _mapper.Map<StoreModel>(store);
            var staffDetail = _mapper.Map<StaffModel>(staff);
            var defaultCountry = store?.Address?.Country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO ? true : false;

            return new GetStoreByIdResponse
            {
                Store = storeDetail,
                Staff = staffDetail,
                IsDefaultCountry = defaultCountry
            };

        }
    }
}
