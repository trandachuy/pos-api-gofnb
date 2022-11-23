using GoFoodBeverage.Application.Features.EmailCampaigns.Commands;
using GoFoodBeverage.Application.Features.EmailCampaigns.Queries;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class EmailCampaignController : BaseApiController
    {
        public EmailCampaignController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-email-campaign")]
        [HasPermission(EnumPermission.VIEW_EMAIL_CAMPAIGN)]
        public async Task<IActionResult> GetAllEmailCampaignAsync([FromQuery] GetAllEmailCampaignRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-email-campaign")]
        [HasPermission(EnumPermission.CREATE_EMAIL_CAMPAIGN)]
        public async Task<IActionResult> CreateEmailCampaignAsync([FromBody] CreateEmailCampaignRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("trigger-send-email-campaign")]
        [HasPermission(EnumPermission.INTERNAL_TOOL)]
        public async Task<IActionResult> TriggerSendEmailCampaign([FromBody] TriggerSendEmailCampaignRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}