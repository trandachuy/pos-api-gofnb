using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Staffs.Queries;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class StaffController : BaseApiController
    {
        public StaffController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-data-staff-management")]
        public async Task<IActionResult> GetDataStaffManagementAsync([FromQuery] GetDataStaffManagementRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-staffs")]
        public async Task<IActionResult> GetStaffs([FromQuery] GetStaffsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-prepare-create-new-staff-data")]
        public async Task<IActionResult> GetPrepareCreateNewStaffDataAsync()
        {
            var response = await _mediator.Send(new GetPrepareCreateNewStaffDataRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-staff-by-id/{id}")]
        public async Task<IActionResult> GetStaffByIdAsync([FromRoute] GetStaffByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-staff")]
        public async Task<IActionResult> CreateNewStaffAsync([FromBody] CreateNewStaffRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-current-staff")]
        public async Task<IActionResult> GetCurrentStaffAsync([FromRoute] GetCurrentStaffRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-staff")]
        public async Task<IActionResult> UpdateStaffAsync([FromBody] UpdateStaffRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateStaffProfileRequest request)
        {
            var respone = await _mediator.Send(request);
            return await SafeOkAsync(respone);
        }

        [HttpDelete]
        [Route("delete-staff-by-id/{id}")]
        public async Task<IActionResult> DeleteStaffByIdAsync([FromRoute] DeleteStaffByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-staff-activities")]
        public async Task<IActionResult> GetStaffActivities([FromQuery] GetStaffActivitiesRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
