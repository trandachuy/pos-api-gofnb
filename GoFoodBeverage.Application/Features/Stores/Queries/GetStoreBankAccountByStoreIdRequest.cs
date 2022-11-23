using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetStoreBankAccountByStoreIdRequest : IRequest<GetStoreBankAccountByStoreIdResponse>
    {
    }

    public class GetStoreBankAccountByStoreIdResponse
    {
        public StoreBankAccountModel StoreBankAccount { get; set; }
    }

    public class GetStoreBankAccountByStoreIdRequestHandler : IRequestHandler<GetStoreBankAccountByStoreIdRequest, GetStoreBankAccountByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;

        public GetStoreBankAccountByStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<GetStoreBankAccountByStoreIdResponse> Handle(GetStoreBankAccountByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var storeBankAccount = await _unitOfWork.StoreBankAccounts
                .GetStoreBankAccountByStoreIdAsync(loggedUser.StoreId);

            var storeBankAccountModel = _mapper.Map<StoreBankAccountModel>(storeBankAccount);

            return new GetStoreBankAccountByStoreIdResponse
            {
                StoreBankAccount = storeBankAccountModel
            };
        }
    }
}
