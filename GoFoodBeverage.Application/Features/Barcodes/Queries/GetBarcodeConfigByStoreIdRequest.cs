using AutoMapper;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.BarcodeConfig;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Barcodes.Queries
{
    public class GetBarcodeConfigByStoreIdRequest : IRequest<GetBarcodeConfigByStoreIdResponse>
    {

    }

    public class GetBarcodeConfigByStoreIdResponse
    {
        public List<BarcodeType> BarcodeTypeList { get; set; }

        public List<StampType> StampTypeList { get; set; }

        public BarcodeConfigModel BarcodeConfig { get; set; }
    }

    public class GetBarcodeConfigByStoreIdRequestHandler : IRequestHandler<GetBarcodeConfigByStoreIdRequest, GetBarcodeConfigByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetBarcodeConfigByStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetBarcodeConfigByStoreIdResponse> Handle(GetBarcodeConfigByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var listBarcodeType = EnumBarcodeType.barcode.GetList();
            var listStampType = EnumStampType.mm40x25.GetList();

            var barcodeConfig = await _unitOfWork.BarcodeConfigs
                .GetBarcodeConfigByStoreIdAsync(loggedUser.StoreId);

            var barcodeConfigModel = _mapper.Map<BarcodeConfigModel>(barcodeConfig);

            return new GetBarcodeConfigByStoreIdResponse()
            {
                BarcodeTypeList = listBarcodeType,
                BarcodeConfig = barcodeConfigModel,
                StampTypeList = listStampType
            };
        }
    }
}
