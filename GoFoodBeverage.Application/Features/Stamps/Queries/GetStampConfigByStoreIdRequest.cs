using AutoMapper;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.StampConfig;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Application.Features.Stamps.Queries
{
    public class GetStampConfigByStoreIdRequest : IRequest<GetStampConfigByStoreIdResponse>
    {

    }

    public class GetStampConfigByStoreIdResponse
    {
        public List<StampType> StampTypeList { get; set; }

        public StampConfigModel StampConfig { get; set; }
    }

    public class GetStampConfigByStoreIdRequestHandler : IRequestHandler<GetStampConfigByStoreIdRequest, GetStampConfigByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetStampConfigByStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetStampConfigByStoreIdResponse> Handle(GetStampConfigByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var listStampType = EnumStampType.mm40x25.GetList();

             var stampConfig = await _unitOfWork.StampConfigs
                .GetStampConfigByStoreIdAsync(loggedUser.StoreId);

            var stampConfigModel = _mapper.Map<StampConfigModel>(stampConfig);

            return new GetStampConfigByStoreIdResponse()
            {
                StampTypeList = listStampType,
                StampConfig = stampConfigModel
            };
        }
    }
}
