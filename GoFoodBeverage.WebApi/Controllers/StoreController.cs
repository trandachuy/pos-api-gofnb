using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Stores.Queries;
using GoFoodBeverage.Application.Features.Stores.Commands;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Application.Features.Package.Queries;
using GoFoodBeverage.Application.Features.Package.Commands;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class StoreController : BaseApiController
    {
        public StoreController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-prepare-register-new-store-data")]
        public async Task<IActionResult> GetPrepareRegisterNewStoreData()
        {
            var response = await _mediator.Send(new GetPrepareRegisterNewStoreDataRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-currency-by-store-id")]
        public async Task<IActionResult> IsCurrencyVND()
        {
            var response = await _mediator.Send(new GetCurrencyRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-prepare-address-data")]
        public async Task<IActionResult> GetPrepareAddressData()
        {
            var response = await _mediator.Send(new GetPrepareAddressDataRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-by-id")]
        public async Task<IActionResult> GetStoreByIdAsync([FromRoute] GetStoreByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-bank-account-by-store-id")]
        public async Task<IActionResult> GetStoreBankAccountByStoreIdAsync([FromRoute] GetStoreBankAccountByStoreIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-platform-activated")]
        public async Task<IActionResult> GetAllPlatformActivatedAsync()
        {
            var response = await _mediator.Send(new GetAllActivePlatformRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-kitchen-config-by-store-id")]
        public async Task<IActionResult> GetStoreKitchenConfigByStoreIdAsync()
        {
            var response = await _mediator.Send(new GetStoreKitchenConfigByStoreIdRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-available-branch-quantity")]
        public async Task<IActionResult> GetAvailableBranchQuantityAsync()
        {
            var response = await _mediator.Send(new GetAvailableBranchQuantityRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-current-package-info")]
        public async Task<IActionResult> GetCurrentOrderPackageInfoAsync()
        {
            var response = await _mediator.Send(new GetCurrentOrderPackageInfoRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [HasPermission(EnumPermission.INTERNAL_TOOL)]
        [Route("check-branch-expiration")]
        public async Task<IActionResult> CheckBranchExpirationAsync()
        {
            var response = await _mediator.Send(new CheckBranchExpirationRequest());
            return Ok(response);
        }

        [HttpGet]
        [Route("information")]
        public async Task<IActionResult> GetStoreInformationAsync()
        {
            var response = await _mediator.Send(new GetStoreInformationRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("register-new-store-account")]
        public async Task<IActionResult> RegisterNewStoreAccount([FromBody] RegisterNewStoreAccountRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-branch-purchase-order-package")]
        public async Task<IActionResult> CreateBranchPurchaseOrderPackageAsync([FromBody] CreateBranchPurchaseOrderPackageRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-store")]
        public async Task<IActionResult> UpdateStoreManagementAsync([FromBody] UpdateStoreManagementRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("activate-account-store")]
        public async Task<IActionResult> ActivateAccountStoreAsync([FromHeader] ActivateAccountStoreRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-slider")]
        public async Task<IActionResult> CreateSliderAsync([FromForm] CreateSliderRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-banners/{bannerType}")]
        public async Task<IActionResult> GetStoreBannersAsync([FromRoute] GetStoreBannersAsyncRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("update-store-banners")]
        public async Task<IActionResult> UpdateStoreBannerAsync([FromBody] UpdateStoreBannerAsyncRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("update-store-logo")]
        public async Task<IActionResult> UpdateStoreLogoAsync([FromForm] UpdateStoreLogoRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }


        [HttpGet]
        [Route("get-themes")]
        public async Task<IActionResult> GetThemesAsync()
        {
            var response = await _mediator.Send(new GetThemesRequest());
            return await SafeOkAsync(response);
        }

        #region GoFood App

        [HttpGet]
        [Route("get-stores-by-address")]
        public async Task<IActionResult> GetStoresByAddressAsync([FromQuery] GetStoresByAddressRequest query)
        {
            var response = await _mediator.Send(query);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("get-all-branches-by-store-id-or-branch-id")]
        public async Task<IActionResult> GetAllBranchByStoreIdOrBranchId([FromBody] GetAllBranchInStoreByStoreIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("search-product-by-name-or-store-name")]
        public async Task<IActionResult> Search([FromBody] SearchProductByNameOrStoreNameRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #endregion
    }
}
