using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using GoFoodBeverage.POS.Application.Features.Combos.Queries;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [Authorize]
    public class ComboController : BaseApiController
    {
        public ComboController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-combos-activated")]
        public async Task<IActionResult> GetCombosActivatedAsync()
        {
            var response = await _mediator.Send(new GetAllComboActivatedRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-combo-by-id/{id}")]
        public async Task<IActionResult> GetComboByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetComboByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }
    }
}
