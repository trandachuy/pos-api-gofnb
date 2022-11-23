using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Storage.Azure;
using GoFoodBeverage.Storage.Azure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class CreateSliderRequest : IRequest<SliderInfoResponse>
    {
        public IFormFile File { get; set; }

        public EnumSliderType ScreenType { get; set; }

        public string FileName { get; set; }
    }

    public class SliderInfoResponse
    {
        public string FileUrl { get; set; }

        public string Name { get; set; }

        public bool Success { get; set; }
    }

    public class CreateSliderRequestHandler : IRequestHandler<CreateSliderRequest, SliderInfoResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IUserProvider _userProvider;

        public CreateSliderRequestHandler(IUnitOfWork unitOfWork, IAzureStorageService azureStorageService, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _azureStorageService = azureStorageService;
            _userProvider = userProvider;
        }

        public async Task<SliderInfoResponse> Handle(CreateSliderRequest request, CancellationToken cancellationToken)
        {
            ThrowError.Against(request.File == null, "File is null or empty");

            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var fileUploadRequestModel = new FileUploadRequestModel()
            {
                File = request.File,
                FileName = request.FileName
            };

            var fileUrl = await _azureStorageService.UploadFileToStorageAsync(fileUploadRequestModel);
            var fileUpload = new FileUpload
            {
                Name = fileUploadRequestModel.FileName,
                NameAlias = "",
                IsActivated = true,
                FileUrl = fileUrl,
                UsingById = EnumFileUsingBy.Slider,
                Type = (int)request.ScreenType,
                StoreId = loggedUser.StoreId
            };

            await _unitOfWork.FileUpload.AddAsync(fileUpload);

            var response = new SliderInfoResponse()
            {
                FileUrl = fileUpload.FileUrl,
                Name = fileUpload.Name,
                Success = true
            };

            return response;
        }
    }
}
