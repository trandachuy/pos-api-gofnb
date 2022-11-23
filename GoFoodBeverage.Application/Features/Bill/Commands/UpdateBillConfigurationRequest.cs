using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Bill;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Bill.Commands
{
    public class UpdateBillConfigurationRequest : IRequest<bool>
    {
        public BillModel BillConfiguration { get; set; }
    }

    public class UpdateBillConfigurationRequestHandler : IRequestHandler<UpdateBillConfigurationRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public UpdateBillConfigurationRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<bool> Handle(UpdateBillConfigurationRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var billConfiguration = await _unitOfWork.Bills
                .Find(x => x.BillFrameSize == request.BillConfiguration.BillFrameSize && x.StoreId == loggedUser.StoreId.Value)
                .FirstOrDefaultAsync();

            // Update
            var updatedBill = UpdateBillConfiguration(billConfiguration, request, loggedUser.StoreId.Value, loggedUser.AccountId.Value);

            var removeDefaultBill = await _unitOfWork.Bills
                .Find(x => x.Id != billConfiguration.Id && x.StoreId == loggedUser.StoreId.Value && x.IsDefault == true)
                .FirstOrDefaultAsync();

            if(removeDefaultBill != null)
            {
                removeDefaultBill.IsDefault = false;
                await _unitOfWork.Bills.UpdateAsync(removeDefaultBill);
            }

            await _unitOfWork.Bills.UpdateAsync(updatedBill);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static Domain.Entities.BillConfiguration UpdateBillConfiguration(Domain.Entities.BillConfiguration data, UpdateBillConfigurationRequest request, Guid storeId, Guid accountId)
        {
            data.BillFrameSize = request.BillConfiguration.BillFrameSize;
            data.IsShowLogo = request.BillConfiguration.IsShowLogo;
            data.LogoData = request.BillConfiguration.LogoData;
            data.IsShowAddress = request.BillConfiguration.IsShowAddress;
            data.IsShowOrderTime = request.BillConfiguration.IsShowOrderTime;
            data.IsShowCashierName = request.BillConfiguration.IsShowCashierName;
            data.IsShowCustomerName = request.BillConfiguration.IsShowCustomerName;
            data.IsShowToping = request.BillConfiguration.IsShowToping;
            data.IsShowOption = request.BillConfiguration.IsShowOption;
            data.IsShowThanksMessage = request.BillConfiguration.IsShowThanksMessage;
            data.ThanksMessageData = request.BillConfiguration.ThanksMessageData;
            data.IsShowWifiAndPassword = request.BillConfiguration.IsShowWifiAndPassword;
            data.WifiData = request.BillConfiguration.WifiData;
            data.PasswordData = request.BillConfiguration.PasswordData;
            data.IsShowQRCode = request.BillConfiguration.IsShowQRCode;
            data.QRCodeData = request.BillConfiguration.QRCodeData;
            data.QRCodeThumbnail = request.BillConfiguration.QRCodeThumbnail;
            data.LastSavedUser = accountId;
            data.IsDefault = true;
            return data;
        }
    }
}
