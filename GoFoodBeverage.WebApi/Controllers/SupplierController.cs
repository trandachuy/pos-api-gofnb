using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Suppliers.Queries;
using GoFoodBeverage.Application.Features.Suppliers.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class SupplierController : BaseApiController
    {
        public SupplierController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-supplier")]
        [HasPermission(EnumPermission.VIEW_SUPPLIER)]
        public async Task<IActionResult> GetAllSupplierAsync()
        {
            var response = await _mediator.Send(new GetAllSuppilerRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-list-supplier")]
        [HasPermission(EnumPermission.VIEW_SUPPLIER)]
        public async Task<IActionResult> GetListSupplierAsync([FromQuery] GetListSupplierRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-supplier-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_SUPPLIER)]
        public async Task<IActionResult> GetSupplierByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetSupplierByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-supplier")]
        [HasPermission(EnumPermission.CREATE_SUPPLIER)]
        public async Task<IActionResult> CreateSupplierAsync(CreateSupplierRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-supplier")]
        [HasPermission(EnumPermission.EDIT_SUPPLIER)]
        public async Task<IActionResult> UpdateSupplierAsync([FromBody] UpdateSupplierRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-supplier-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_SUPPLIER)]
        public async Task<IActionResult> DeleteSupplierByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteSupplierByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }
    }
}
