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
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class ProductController : BaseApiController
    {
        public ProductController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-products")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProductsAsync([FromQuery] GetProductsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
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
        [Route("get-all-products")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var response = await _mediator.Send(new GetAllProductsRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-products-active")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProductsActiveAsync()
        {
            var response = await _mediator.Send(new GetAllProductsActiveRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-products-with-category")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetAllProductsWithCategoryAsync()
        {
            var response = await _mediator.Send(new GetAllProductsWithCategoryRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-products-by-filter")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProductsByFilterAsync([FromQuery] GetProductsByFilterRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-product-by-id")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProductById([FromQuery] GetPrepareProductEditDataResquest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-products-by-category-id")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProductsByCategoryId([FromQuery] GetProductsByCategoryIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-product-included-unit")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetAllProductIncludedProductUnit([FromQuery] GetAllProductIncludedProductUnit request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-order-not-completed-by-product-id/{productId}")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetAllOrderNotCompletedByProductId(Guid productId)
        {
            var response = await _mediator.Send(new GetAllDataNotCompletedByProductIdRequest() { ProductId = productId });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-product-toppings")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetAllProductToppings()
        {
            var response = await _mediator.Send(new GetAllProductToppingsRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("download-import-product-template")]
        [HasPermission(EnumPermission.IMPORT_PRODUCT)]
        public async Task<IActionResult> DownloadImportProductTemplateAsync([FromQuery] DownloadImportProductTemplateRequest request)
        {
            var response = await _mediator.Send(request);
            if (response.Bytes == null || response.Bytes.Length == 0)
            {
                return NotFound();
            }

            await LogActivityAsync();

            return File(response.Bytes, DocumentConstants.EXCEL_FILE, response.FileName);
        }

        [HttpPost]
        [Route("create-product")]
        [HasPermission(EnumPermission.CREATE_PRODUCT)]
        public async Task<IActionResult> CreateproductManagementAsync([FromBody] CreateProductRequest request)
        {
            var response = await _mediator.Send(request);
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

        [HttpPost]
        [Route("update-product-by-category-id")]
        [HasPermission(EnumPermission.EDIT_PRODUCT_CATEGORY)]
        public async Task<IActionResult> UpdateProductByCategory(UpdateProductByCategoryIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("import")]
        [HasPermission(EnumPermission.IMPORT_PRODUCT)]
        public async Task<IActionResult> ImportProductAsync([FromForm] ImportProductRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-product")]
        [HasPermission(EnumPermission.EDIT_PRODUCT)]
        public async Task<IActionResult> EditProductCategoryAsync([FromBody] UpdateProductRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("change-status/{id}")]
        [HasPermission(EnumPermission.EDIT_PRODUCT)]
        public async Task<IActionResult> ChangeStatusAsync(Guid id)
        {
            var response = await _mediator.Send(new ChangeStatusRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-product-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_PRODUCT)]
        public async Task<IActionResult> DeleteProductByIdAsync([FromRoute] DeactivateProductByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-product-in-combo-by-product-id/{id}")]
        [HasPermission(EnumPermission.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProductInComboByProductIdAsync([FromRoute] Guid Id)
        {
            var response = await _mediator.Send(new GetProductInComboByProductlIdRequest() { ProductId = Id });
            return await SafeOkAsync(response);
        }

        #region GoFood App

        [HttpGet]
        [AllowAnonymous]
        [Route("get-product-categories-activated-by-store-id")]
        public async Task<IActionResult> GetProductCategoriesActivatedAsync([FromQuery] GetAllProductCategoryActivatedRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("calculate-product-cart-item")]
        public async Task<IActionResult> GetProductCartItemAsync([FromBody] GetProductCartItemRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-product-detail-by-id")]
        public async Task<IActionResult> GetProductDetailById([FromQuery] GetProductDetailByIdResquest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-combo-product-by-combo-id")]
        public async Task<IActionResult> GetComboDetailByIdAsync([FromQuery] GetComboProductByComboIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #endregion
    }
}