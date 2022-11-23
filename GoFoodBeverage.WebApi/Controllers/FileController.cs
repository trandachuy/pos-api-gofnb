using GoFoodBeverage.Common.Models.Base;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Storage.Azure;
using GoFoodBeverage.Storage.Azure.Models;
using GoFoodBeverage.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class FileController : BaseApiController
    {
        private readonly IAzureStorageService _azureStorageService;

        public FileController(IMediator mediator, IUserActivityService userActivityService, IAzureStorageService azureStorageService) : base(mediator, userActivityService)
        {
            _azureStorageService = azureStorageService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadAsync([FromForm] FileUploadRequestModel request)
        {
            try
            {
                var fileUrl = await _azureStorageService.UploadFileToStorageAsync(request);
                var response = new ResponseModel()
                {
                    Success = true,
                    Data = fileUrl
                };

                return SafeOk(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel()
                {
                    Success = false,
                    Data = ex.Message
                };

                return SafeOk(response);
            }
        }
    }
}
