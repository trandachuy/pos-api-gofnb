using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseApiController: ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly IUserActivityService _userActivityService;

        public BaseApiController(IMediator mediator, IUserActivityService userActivityService)
        {
            _mediator = mediator;
            _userActivityService = userActivityService;
        }

        protected ActionResult OkOrNoContent(object value)
        {
            if (value == null) return NoContent();

            return Ok(value);
        }

        protected ActionResult SafeOk<T>(List<T> list)
        {
            if (list == null) return Ok(new List<T>());

            return Ok(list);
        }

        protected ActionResult SafeOk<T>(IList<T> list)
        {
            if (list == null) return Ok(new List<T>());

            return Ok(list);
        }

        protected ActionResult SafeOk(object value)
        {
            if (value == null) throw new NotFoundException();

            return Ok(value);
        }

        protected ActionResult SafeOk() => new OkResult();

        protected async Task<ActionResult> OkOrNoContentAsync(object value)
        {
            await LogActivityAsync();

            if (value == null) return NoContent();

            return Ok(value);
        }

        protected async Task<ActionResult> SafeOkAsync<T>(List<T> list)
        {
            await LogActivityAsync();

            if (list == null) return Ok(new List<T>());

            return Ok(list);
        }

        protected async Task<ActionResult> SafeOkAsync<T>(IList<T> list)
        {
            await LogActivityAsync();

            if (list == null) return Ok(new List<T>());

            return Ok(list);
        }

        protected async Task<ActionResult> SafeOkAsync(object value)
        {
            await LogActivityAsync();

            if (value == null) throw new NotFoundException();

            return Ok(value);
        }

        protected async Task<ActionResult> SafeOkAsync()
        {
            await LogActivityAsync();

            return new OkResult();
        }

        private Task LogActivityAsync()
        {
            var method = Request.Method;
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ControllerContext.ActionDescriptor.ActionName;

            return _userActivityService.LogAsync($"{method}-{controllerName}-{actionName}");
        }

    }
}
