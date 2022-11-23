using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Products.Queries;
using GoFoodBeverage.Application.Features.Products.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class ProductCategoryController : BaseApiController
    {
        public ProductCategoryController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-product-categories")]
        [HasPermission(EnumPermission.VIEW_PRODUCT_CATEGORY)]
        public async Task<IActionResult> GetProductCategoriesAsync([FromQuery] GetProductCategoriesRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-product-categories")]
        [HasPermission(EnumPermission.VIEW_PRODUCT_CATEGORY)]
        public async Task<IActionResult> GetAllProductCategoriesAsync()
        {
            var response = await _mediator.Send(new GetAllProductCategoriesRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-product-category-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_PRODUCT_CATEGORY)]
        public async Task<IActionResult> GetProductCategoryByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetProductCategoryByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-product-category")]
        [HasPermission(EnumPermission.CREATE_PRODUCT_CATEGORY)]
        public async Task<IActionResult> CreateProductCategoryAsync([FromBody] CreateProductCategoryRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-product-category")]
        [HasPermission(EnumPermission.EDIT_PRODUCT_CATEGORY)]
        public async Task<IActionResult> UpdateProductCategoryAsync([FromBody] UpdateProductCategoryRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-product-category-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_PRODUCT_CATEGORY)]
        public async Task<IActionResult> DeleteProductCategoryByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteProductCategoryByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }
    }
}
