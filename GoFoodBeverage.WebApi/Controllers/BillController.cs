using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Application.Features.Bill.Queries;
using GoFoodBeverage.Application.Features.Bill.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class BillController : BaseApiController
    {
        public BillController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-bill-configuration")]
        public async Task<IActionResult> GetBillConfigurationAsync([FromRoute] GetBillConfigurationRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-bill-configuration")]
        public async Task<IActionResult> UpdateBillConfigurationAsync([FromBody] UpdateBillConfigurationRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
