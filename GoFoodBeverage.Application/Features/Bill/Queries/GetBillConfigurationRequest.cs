using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Bill;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Bill.Queries
{
    public class GetBillConfigurationRequest : IRequest<GetBillConfigurationResponse>
    {
    }

    public class GetBillConfigurationResponse
    {
        public List<BillModel> BillConfigurations { get; set; }
    }

    public class GetBillConfigurationRequestHandler : IRequestHandler<GetBillConfigurationRequest, GetBillConfigurationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper; 
        private readonly MapperConfiguration _mapperConfiguration;

        public GetBillConfigurationRequestHandler(
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

        public async Task<GetBillConfigurationResponse> Handle(GetBillConfigurationRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var logo = await _unitOfWork.Stores.Find(x => x.Id == loggedUser.StoreId.Value).Select(x => x.Logo).FirstOrDefaultAsync();

            if (await CheckExistedData(loggedUser.StoreId.Value))
            {
                var bills = await _unitOfWork.Bills.GetDefaultBillConfigurationByStore(loggedUser.StoreId.Value).ToListAsync();
                var response = _mapper.Map<List<BillModel>>(bills);
                response.ForEach(x =>
                {
                    x.Logo = logo;
                });

                return new GetBillConfigurationResponse { BillConfigurations = response };
            }
            else
            {
                var newBills = CreateBillConfiguration(loggedUser.StoreId.Value, loggedUser.AccountId.Value);
                _unitOfWork.Bills.AddRange(newBills);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<List<BillModel>>(newBills);
                response.ForEach(x =>
                {
                    x.Logo = logo;
                });

                return new GetBillConfigurationResponse { BillConfigurations = response };
            }
        }

        private async Task<bool> CheckExistedData(Guid storeId)
        {
            var data = await _unitOfWork.Bills.Find(m => m.StoreId.Value == storeId).ToListAsync();
            return data.Count() > 0;
        }

        private List<Domain.Entities.BillConfiguration> CreateBillConfiguration(Guid storeId, Guid accountId)
        {
            List<Domain.Entities.BillConfiguration> billConfigurations = new List<Domain.Entities.BillConfiguration>();

            foreach(int frameSize in Enum.GetValues(typeof(EnumBillFrameSize)))
            {
                var bill = new Domain.Entities.BillConfiguration()
                {
                    StoreId = storeId,
                    BillFrameSize = (EnumBillFrameSize)frameSize,
                    CreatedUser = accountId,
                    IsShowAddress = true,
                    IsShowOrderTime = true,
                    IsShowCashierName = true,
                    IsShowCustomerName = true,
                    IsShowToping = true,
                    IsShowOption = true,
                    IsShowThanksMessage = true,
                    ThanksMessageData = ThanksMessageConstants.ThanksMessage
                };

                if((EnumBillFrameSize)frameSize == EnumBillFrameSize.Medium)
                {
                    bill.IsDefault = true;
                }

                billConfigurations.Add(bill);
            }
            
            return billConfigurations;
        }
    }
}
