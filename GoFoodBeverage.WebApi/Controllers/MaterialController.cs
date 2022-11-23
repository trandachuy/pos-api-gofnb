using MediatR;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Materials.Queries;
using GoFoodBeverage.Application.Features.Materials.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class MaterialController : BaseApiController
    {
        public MaterialController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-material-management")]
        [HasPermission(EnumPermission.VIEW_MATERIAL)]
        public async Task<IActionResult> GetAllMaterialsAsync()
        {
            var response = await _mediator.Send(new GetAllMaterialRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-materials")]
        [HasPermission(EnumPermission.VIEW_MATERIAL)]
        public async Task<IActionResult> GetMaterialsAsync([FromQuery] GetMaterialsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-materials-by-filter")]
        [HasPermission(EnumPermission.VIEW_MATERIAL)]
        public async Task<IActionResult> GetMaterialsByFilterAsync([FromQuery] GetMaterialsByFilterRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-material-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_MATERIAL)]
        public async Task<IActionResult> GetMaterialByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetMaterialByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-material-prepare-edit-data/{id}")]
        public async Task<IActionResult> GetPrepareMaterialEditDataAsync([FromRoute] Guid id)
        {
            var response = await _mediator.Send(new GetMaterialPrepareEditDataRequest() { Materiald = id });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("download-template-material")]
        public async Task<IActionResult> DownloadTemplateMaterialAsync([FromQuery] DownloadTemplateMaterialRequest request)
        {
            var response = await _mediator.Send(request);
            string outputFile = response.Result;
            if (string.IsNullOrEmpty(outputFile))
            {
                return BadRequest("Cannot download template!");
            }

            var fileName = Path.GetFileName(outputFile);
            byte[] fileBytes = System.IO.File.ReadAllBytes(outputFile);

            if (fileBytes == null)
            {
                return NotFound();
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("export-material")]
        public async Task<IActionResult> ExportMaterialsAsync([FromQuery] ExportMaterialRequest request)
        {
            var response = await _mediator.Send(request);
            string outputFile = response.Result;
            if (string.IsNullOrEmpty(outputFile))
            {
                return BadRequest("Cannot download template!");
            }

            var fileName = Path.GetFileName(outputFile);
            byte[] fileBytes = System.IO.File.ReadAllBytes(outputFile);

            if (fileBytes == null)
            {
                return NotFound();
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        [Route("create-material-management")]
        [HasPermission(EnumPermission.CREATE_MATERIAL)]
        public async Task<IActionResult> CreateMaterialManagementAsync([FromBody] CreateMaterialRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [Route("import-materials")]
        [HttpPost]
        [HasPermission(EnumPermission.IMPORT_MATERIAL)]
        public async Task<IActionResult> ImportMaterials(IFormFile file)
        {
            var response = await _mediator.Send(new ImportMaterialsRequest() { File = file });
            return Ok(response);
        }

        [HttpPut]
        [Route("activate-material/{id}")]
        [HasPermission(EnumPermission.ACTIVATE_MATERIAL)]
        public async Task<IActionResult> ActivateMaterialByIdAsync([FromRoute] Guid Id)
        {
            var response = await _mediator.Send(new UpdateMaterialStatusRequest() { Id = Id, IsActive = true});
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("deactivate-material/{id}")]
        [HasPermission(EnumPermission.DEACTIVATE_MATERIAL)]
        public async Task<IActionResult> DeActivateMaterialByIdAsync([FromRoute] Guid Id)
        {
            var response = await _mediator.Send(new UpdateMaterialStatusRequest() { Id = Id, IsActive = false });
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-material-management")]
        [HasPermission(EnumPermission.EDIT_MATERIAL)]
        public async Task<IActionResult> UpdateMaterialManagementAsync([FromBody] UpdateMaterialRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-material-management/{id}")]
        [HasPermission(EnumPermission.DELETE_MATERIAL)]
        public async Task<IActionResult> DeleteMaterialManagementAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteMaterialManagementRequest() { Id = id});
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-product-price-material-by-material-id/{materialId}")]
        [HasPermission(EnumPermission.DELETE_MATERIAL)]
        public async Task<IActionResult> DeleteProductPriceMaterialByMaterialIddAsync([FromRoute] DeleteProductPriceMaterialByMaterialIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-material-filter")]
        [HasPermission(EnumPermission.VIEW_MATERIAL)]
        public async Task<IActionResult> GetAllMaterialsFilterAsync()
        {
            var response = await _mediator.Send(new GetAllMaterialFilterRequest());
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-cost-per-unit-by-material-id")]
        [HasPermission(EnumPermission.EDIT_MATERIAL)]
        public async Task<IActionResult> UpdateCostPerUnitByMaterialIdAsync([FromBody] UpdateCostPerUnitByMaterialIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
