using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Stores.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class StoreController : BaseApiController
    {
        public StoreController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-prepare-address-data")]
        public async Task<IActionResult> GetPrepareAddressData()
        {
            var response = await _mediator.Send(new GetPrepareAddressDataRequest());

            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-kitchen-config-by-store-id")]
        public async Task<IActionResult> GetStoreKitchenConfigByStoreIdAsync()
        {
            var response = await _mediator.Send(new GetStoreKitchenConfigByStoreIdRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-prepare-store-data-for-order-delivery")]
        public async Task<IActionResult> GetPrepareStoreDataForOrderDeliveryAsync()
        {
            var response = await _mediator.Send(new GetPrepareStoreDataForOrderDeliveryRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-info")]
        public async Task<IActionResult> GetStoreInformationAsync()
        {
            var response = await _mediator.Send(new GetPrepareStoreDataForOrderDeliveryRequest());
            return await SafeOkAsync(response);
        }
    }
}
