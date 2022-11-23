using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Storage.Azure;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Storage.Azure.Models;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class UpdateStoreLogoRequest : FileUploadRequestModel, IRequest<string> { }

    public class UpdateStoreLogoRequestHandler : IRequestHandler<UpdateStoreLogoRequest, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IAzureStorageService _azureStorageService;

        public UpdateStoreLogoRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IAzureStorageService azureStorageService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _azureStorageService = azureStorageService;
        }

        public async Task<string> Handle(UpdateStoreLogoRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            string fileUrl = string.Empty;

            if (request.File != null)
                fileUrl = await _azureStorageService.UploadFileToStorageAsync(request);
            Store storeEntity = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            storeEntity.Logo = fileUrl;
            await _unitOfWork.Stores.UpdateAsync(storeEntity);

            return fileUrl;
        }
    }
}
