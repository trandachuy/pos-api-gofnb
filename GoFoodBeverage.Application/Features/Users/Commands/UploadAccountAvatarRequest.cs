using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Storage.Azure;
using GoFoodBeverage.Storage.Azure.Models;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class UpdateAccountAvatarRequest : IRequest<string>
    {
        public IFormFile File { get; set; }

        public string FileName { get; set; }
    }

    public class UpdateAccountAvatarRequestHandler : IRequestHandler<UpdateAccountAvatarRequest, string>
    {
        private IUnitOfWork _unitOfWork;

        private IUserProvider _userProvider;

        private IAzureStorageService _azureStorageService;


        public UpdateAccountAvatarRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IAzureStorageService azureStorageService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _azureStorageService = azureStorageService;
        }

        /// <summary>
        /// This method is used to handle data from the current HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the current HTTP request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<string> Handle(UpdateAccountAvatarRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new modal and set modal data.
                var updateModal = new FileUploadRequestModel();
                updateModal.File = request.File;
                updateModal.FileName = request.FileName;

                // Send file to the Azure server.
                var filePath = await _azureStorageService.UploadFileToStorageAsync(updateModal);

                // If the resource has been saved successfully, return the current path for the client.
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    // Get account information from the token.
                    var loggedUser = _userProvider.Provide();

                    // Get account information from the database by the token and set the user avatar.
                    var staff = await _unitOfWork.Staffs.GetStaffByAccountIdAsync(loggedUser.AccountId.Value);
                    staff.Thumbnail = filePath;

                    // Update data and return data to the logic method.
                    await _unitOfWork.SaveChangesAsync();
                    return filePath;
                }

                return string.Empty;
            }
            catch
            {

                return string.Empty;
            }
        }
    }
}
