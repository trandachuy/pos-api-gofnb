using GoFoodBeverage.Application.Features.Customer.Commands;
using GoFoodBeverage.Application.Features.Customer.Queries;
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
    public class CustomerController : BaseApiController
    {
        public CustomerController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-customers")]
        [HasPermission(EnumPermission.VIEW_CUSTOMER)]
        public async Task<IActionResult> GetCustomersAsync([FromQuery] GetCustomersRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customers-by-segment")]
        [HasPermission(EnumPermission.VIEW_CUSTOMER)]
        public async Task<IActionResult> GetCustomersBySegmentAsync([FromQuery] GetListCustomersBySegmentRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customer-by-id")]
        [HasPermission(EnumPermission.VIEW_CUSTOMER)]
        public async Task<IActionResult> GetCustomerByIdAsync([FromQuery] GetCustomerByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customer-by-accumulatedPoint")]
        [HasPermission(EnumPermission.VIEW_CUSTOMER)]
        public async Task<IActionResult> GetCustomerByAccumulatedPointAsync([FromQuery] GetCustomerbyAccumulatedPointRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-loyalty-by-store-id")]
        public async Task<IActionResult> GetLoyaltyPointByStoreIdAsync()
        {
            var response = await _mediator.Send(new GetLoyaltyPointByStoreIdRequest());
            return await SafeOkAsync(response);
        }

        /// <summary>
        /// This method is used to check if the customer already exists in the system for the mobile app.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>The JSON object.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("check-account-already-exists-in-system")]
        public async Task<IActionResult> CheckAccountAlreadyExistsInSystem([FromQuery] CheckAccountAlreadyExistsInSystemRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customers-by-date-range")]
        [HasPermission(EnumPermission.VIEW_CUSTOMER)]
        public async Task<IActionResult> GetCustomersByDateRangeAsync([FromQuery] GetCustomersByDateRangeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-customer")]
        [HasPermission(EnumPermission.CREATE_CUSTOMER)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CreateCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("modify-loyalty-point")]
        public async Task<IActionResult> ModifyLoyaltyPointAsync([FromBody] ModifyLoyaltyPointRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-customer")]
        [HasPermission(EnumPermission.EDIT_CUSTOMER)]
        public async Task<IActionResult> UpdateCustomerAsync([FromBody] EditCustomerByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-customer-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_CUSTOMER)]
        public async Task<IActionResult> DeleteCustomerByIdAsync([FromRoute] DeleteCustomerByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customer-report")]
        public async Task<IActionResult> GetCustomerReportAsync([FromQuery] GetCustomerReportWithPlatformRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("reset-accumulated-point")]
        [HasPermission(EnumPermission.INTERNAL_TOOL)]
        public async Task<IActionResult> ResetAccumulatedPoinAsync([FromBody] ResetAccumulatedPointRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #region GoFood App

        /// <summary>
        /// This method is used to create a new customer for the mobile app.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>The JSON object.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("quick-create-customer")]
        public async Task<IActionResult> QuickCreateCustomerAsync([FromBody] QuickCreateCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-current-customer")]
        public async Task<IActionResult> GetCurrentCustomerAsync([FromRoute] GetCurrentCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("update-customer-profile")]
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCustomerProfileRequest request)
        {
            var respone = await _mediator.Send(request);
            return await SafeOkAsync(respone);
        }

        /// <summary>
        /// This method is used to get the customer's order list on the Order List screen on the mobile application.
        /// It can be called to check some information to delete the account on the mobile application.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>JSON object.</returns>
        [HttpGet]
        [Route("get-customer-order-list")]
        public async Task<IActionResult> GetCustomerOrderList([FromQuery] GetCustomerOrderListRequest request)
        {
            var respone = await _mediator.Send(request);
            return await SafeOkAsync(respone);
        }

        /// <summary>
        /// This method is used to check the customer status 
        /// when the user returns from the background or opens the mobile application.
        /// This action is only called when the user is logged in.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>JSON object.</returns>
        [HttpGet]
        [Route("get-customer-status")]
        public async Task<IActionResult> GetCustomerStatus([FromQuery] GetCustomerStatusRequest request)
        {
            var respone = await _mediator.Send(request);
            return await SafeOkAsync(respone);
        }

        [HttpPost]
        [Route("update-customer-password")]
        public async Task<IActionResult> UpdateCustomerPasswordAsync([FromBody] UpdateCustomerPassword request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #endregion

    }
}
