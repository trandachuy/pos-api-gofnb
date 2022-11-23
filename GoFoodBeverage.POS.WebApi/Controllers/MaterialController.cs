using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using GoFoodBeverage.POS.Application.Features.Materials.Queries;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class MaterialController : BaseApiController
    {
        public MaterialController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-material-management")]
        public async Task<IActionResult> GetAllMaterialsAsync()
        {
            var response = await _mediator.Send(new GetAllMaterialRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-materials-from-orders-current-shift")]
        public async Task<IActionResult> GetMaterialsFromOrdersCurrentShiftAsync()
        {
            var response = await _mediator.Send(new GetMaterialsFromOrdersCurrentShiftRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-materials-by-branch-id")]
        public async Task<IActionResult> GetMaterialsByBranchIdAsync([FromQuery] GetMaterialsByBranchIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
