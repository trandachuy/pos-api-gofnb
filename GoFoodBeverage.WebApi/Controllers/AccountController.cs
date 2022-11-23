using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Users.Queries;
using GoFoodBeverage.Application.Features.Users.Commands;
using GoFoodBeverage.Application.Features.Account.Commands;
using GoFoodBeverage.Application.Features.Account.Queries;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    [Route("/api/Account")]
    public class AccountController : BaseApiController
    {
        public AccountController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpPost]
        [Route("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("validation-password")]
        public async Task<IActionResult> ValidationPasswordAsync([FromBody] ValidationPasswordRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        /// <summary>
        /// This action is used to update the user's avatar.
        /// </summary>
        /// <param name="request">Data is mapped from the current HTTP request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload-account-avatar")]
        public async Task<IActionResult> UploadAccountAvatarAsync([FromForm] UpdateAccountAvatarRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #region GoFood App

        [HttpPost]
        [Route("create-account-address")]
        public async Task<IActionResult> CreateAccountAddressAsync([FromBody] CreateAccountAddressRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-account-addresses-by-account-id")]
        public async Task<IActionResult> GetAccountAddressesByAccountIdAsync([FromRoute] GetAccountAddressesByAccountIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-account-address-by-id/{id}")]
        public async Task<IActionResult> DeleteAccountAddressByIdAsync([FromRoute] DeleteAccountAddressByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-account-address-by-id/{id}")]
        public async Task<IActionResult> GetAccountAddressByIdAsync([FromRoute] GetAccountAddressByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-account-address-by-id")]
        public async Task<IActionResult> UpdateAccountAddressByIdAsync([FromBody] UpdateAccountAddressByIdRequest request)
        {
            var respone = await _mediator.Send(request);
            return await SafeOkAsync(respone);
        }

        [HttpPost]
        [Route("upload-account-avatar-by-id")]
        public async Task<IActionResult> UploadAccountAvatarByIdAsync([FromForm] UploadAccountAvatarRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        /// <summary>
        /// This method is used to disable account 
        /// when they click on the Delete Account button from the mobile application.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <returns>JSON object.</returns>
        [HttpPut]
        [Route("disable-account")]
        public async Task<IActionResult> DisableAccount([FromRoute] DisableAccountRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #endregion
    }
}
