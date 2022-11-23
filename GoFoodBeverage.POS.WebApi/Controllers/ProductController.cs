using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Products.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [Authorize]
    public class ProductController : BaseApiController
    {
        public ProductController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-product-categories-activated")]
        public async Task<IActionResult> GetProductCategoriesActivatedAsync()
        {
            var response = await _mediator.Send(new GetAllProductCategoryActivatedRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-products-in-platform-by-product-category-id")]
        public async Task<IActionResult> GetProductsInPlatformByCategoryIdAsync([FromQuery] GetProductsInPlatformByCatgoryIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-product-detail-by-id/{id}")]
        public async Task<IActionResult> GetProductDetailByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetProductDetailByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-product-toppings")]
        public async Task<IActionResult> GetProductToppingsAsync()
        {
            var response = await _mediator.Send(new GetAllProductToppingRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-toppings-by-product-id/{id}")]
        public async Task<IActionResult> GetAllToppingByProductIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetAllToppingByProductIdRequest() { ProductId = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("calculate-product-cart-item")]
        public async Task<IActionResult> GetProductCartItemAsync([FromBody] GetProductCartItemRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
