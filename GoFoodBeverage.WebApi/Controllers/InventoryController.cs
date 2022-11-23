using MediatR;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Materials.Queries;
using GoFoodBeverage.Application.Features.Materials.Commands;
using GoFoodBeverage.Application.Features.Inventory.Queries;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class InventoryController : BaseApiController
    {
        public InventoryController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-inventory-history")]
        [HasPermission(EnumPermission.VIEW_INVENTORY_HISTORY)]
        public async Task<IActionResult> GetAllInventoryHistory([FromQuery] GetAllInventoryHistoryRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
