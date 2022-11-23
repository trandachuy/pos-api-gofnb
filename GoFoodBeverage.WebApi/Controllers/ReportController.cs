using GoFoodBeverage.Application.Features.Report.Queries;
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
    public class ReportController : BaseApiController
    {
        public ReportController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-top-customer-report")]
        [HasPermission(EnumPermission.VIEW_CUSTOMER_REPORT)]
        public async Task<IActionResult> GetTopCustomerReportAsync([FromQuery] GetTopCustomerReportRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
