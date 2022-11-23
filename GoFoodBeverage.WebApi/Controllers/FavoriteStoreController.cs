using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.FavoriteStores.Queries;
using GoFoodBeverage.Application.Features.FavoriteStores.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class FavoriteStoreController : BaseApiController
    {
        public FavoriteStoreController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        #region GoFood App
        [HttpGet]
        [Route("get-favoriteStores-by-customer-id")]
        public async Task<IActionResult> GetFavoriteStoresByCustomerIdAsync([FromQuery] GetFavoriteStoresByCustomerIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("remove-store-leave-favoriteStores")]
        public async Task<IActionResult> RemoveStoreLeaveFavoriteStoresAsync([FromBody] RemoveStoreLeaveFavoriteStoresRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
       
        [HttpPost]
        [Route("add-store-on-favoriteStores")]
        public async Task<IActionResult> AddStoreOnFavoriteStoresAsync([FromBody] AddStoreFavoriteStoresRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
        #endregion
    }
}
