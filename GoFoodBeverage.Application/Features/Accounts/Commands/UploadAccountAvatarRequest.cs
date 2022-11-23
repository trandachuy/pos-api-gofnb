using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Storage.Azure;
using GoFoodBeverage.Models.Common;
using GoFoodBeverage.Storage.Azure.Models;

namespace GoFoodBeverage.Application.Features.Account.Commands
{
    public class UploadAccountAvatarRequest : FileUploadRequestModel, IRequest<UploadAccountAvatarResponse> { }

    public class UploadAccountAvatarResponse : ResponseCommonModel
    {
        public string AvatarUrl { get; set; }
    }

    public class UploadAccountAvatarRequestHanlder : IRequestHandler<UploadAccountAvatarRequest, UploadAccountAvatarResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        private readonly IAzureStorageService _azureStorageService;

        public UploadAccountAvatarRequestHanlder(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IAzureStorageService azureStorageService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _azureStorageService = azureStorageService;
        }

        public async Task<UploadAccountAvatarResponse> Handle(UploadAccountAvatarRequest request, CancellationToken cancellationToken)
        {
            var response = new UploadAccountAvatarResponse();
            // Get the current user information from the user token.
            var loggedUser = _userProvider.GetLoggedCustomer();
            Guid? customerId = loggedUser.Id;
            if (!customerId.HasValue)
            {
                response.IsSuccess = false;
                response.Message = "message.uploadAvatarNotLogin";
                return response;
            }

            var customerInformation = await _unitOfWork.Accounts.GetAccountActivatedByIdAsync(customerId.Value);
            if (customerInformation == null)
            {
                response.IsSuccess = false;
                response.Message = "message.accountNotExist";
                return response;
            }

            var fileUrl = await _azureStorageService.UploadFileToStorageAsync(request);

            customerInformation.Thumbnail = fileUrl;
            await _unitOfWork.Accounts.UpdateAsync(customerInformation);
            response.IsSuccess = true;
            response.Message = "message.uploadAvatarComplete";
            response.AvatarUrl = fileUrl;

            return response;
        }
    }
}
