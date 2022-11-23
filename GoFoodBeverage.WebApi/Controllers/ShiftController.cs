using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Shift.Queries;
using GoFoodBeverage.Application.Features.Shifts.Queries;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class ShiftController : BaseApiController
    {
        public ShiftController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-shifts")]
        [HasPermission(EnumPermission.VIEW_SHIFT)]
        public async Task<IActionResult> GetShiftsAsync([FromQuery] GetShiftsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-shift-detail-order")]
        [HasPermission(EnumPermission.VIEW_SHIFT)]
        public async Task<IActionResult> GetShiftDetailOrderAsync([FromQuery] GetShiftsDetailOrderRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-shift-detail-selling-product")]
        [HasPermission(EnumPermission.VIEW_SHIFT)]
        public async Task<IActionResult> GetShiftDetailSellingProductAsync([FromQuery] GetShiftsDetailSellingProductRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-shift-info-by-id")]
        [HasPermission(EnumPermission.VIEW_SHIFT)]
        public async Task<IActionResult> GetInfoShiftByIdRequesAsync([FromQuery] GetInfoShiftByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
