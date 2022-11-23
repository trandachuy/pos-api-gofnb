using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using GoFoodBeverage.POS.Application.Features.Shifts.Commands;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class ShiftController : BaseApiController
    {
        public ShiftController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpPost]
        [Route("start-shift")]
        public async Task<IActionResult> StartShiftAsync([FromBody] StartShiftRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-initial-amount-from-end-shift-by-branch-id/{branchId}")]
        public async Task<IActionResult> GetInitialAmountFromEndShiftByBranchIdAsync(Guid branchId)
        {
            var response = await _mediator.Send(new GetInitialAmountFromEndShiftByBranchIdRequest() { BranchId = branchId});
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("end-shift")]
        public async Task<IActionResult> EndShiftAsync([FromBody] EndShiftRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-info-end-shift")]
        public async Task<IActionResult> GetInfoEndShiftByBranchIdAsync()
        {
            var response = await _mediator.Send(new GetInfoEndShiftByBranchIdRequest());
            return await SafeOkAsync(response);
        }
    }
}
