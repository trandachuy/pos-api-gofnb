using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Materials.Queries;
using GoFoodBeverage.Application.Features.Materials.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class MaterialCategoryController : BaseApiController
    {
        public MaterialCategoryController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-material-categories")]
        [HasPermission(EnumPermission.VIEW_MATERIAL_CATEGORY)]
        public async Task<IActionResult> GetProductsAsync([FromQuery] GetMaterialCategoriesRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-material-categories")]
        [HasPermission(EnumPermission.VIEW_MATERIAL_CATEGORY)]
        public async Task<IActionResult> GetAllMaterialCategoriesAsync()
        {
            var response = await _mediator.Send(new GetAllMaterialCategoriesRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-material-category-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_MATERIAL_CATEGORY)]
        public async Task<IActionResult> GetMaterialCategoryByIdAsync([FromRoute] GetMaterialCategoryByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-material-category")]
        [HasPermission(EnumPermission.CREATE_MATERIAL_CATEGORY)]
        public async Task<IActionResult> CreateMaterialCategoryAsync([FromBody] CreateMaterialCategoryRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-material-category")]
        [HasPermission(EnumPermission.EDIT_MATERIAL_CATEGORY)]
        public async Task<IActionResult> UpdateMaterialCategoryByIdAsync([FromBody] UpdateMaterialCategoryRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-material-categories-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_MATERIAL_CATEGORY)]
        public async Task<IActionResult> DeleteMaterialCategoryByIdAsync([FromRoute] DeleteMaterialCategoryByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
