using GoFoodBeverage.Application.Features.Barcodes.Commands;
using GoFoodBeverage.Application.Features.Barcodes.Queries;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class BarcodeController : BaseApiController
    {
        public BarcodeController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-barcode-config-by-store-id")]
        public async Task<IActionResult> GetBarcodeConfigByStoreIdAsync([FromRoute] GetBarcodeConfigByStoreIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-barcode-config")]
        public async Task<IActionResult> UpdateBarcodeConfigAsync([FromBody] UpdateBarcodeConfigRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
