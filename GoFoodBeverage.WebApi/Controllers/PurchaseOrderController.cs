using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.PurchaseOrders.Queries;
using GoFoodBeverage.Application.Features.PurchaseOrders.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class PurchaseOrderController : BaseApiController
    {
        public PurchaseOrderController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-purchase-order")]
        [HasPermission(EnumPermission.VIEW_PURCHASE)]
        public async Task<IActionResult> GetAllPurchaseOrderAsync([FromQuery] GetPurchaseOrderRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-purchase-order-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_PURCHASE)]
        public async Task<IActionResult> GetPurchaseOrderByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetPurchaseOrderByIdRequest() { Id = id});
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-purchase-order-by-material-id/{materialId}")]
        [HasPermission(EnumPermission.VIEW_PURCHASE)]
        public async Task<IActionResult> GetPurchaseOrderByMaterialIdAsync(Guid materialId)
        {
            var response = await _mediator.Send(new GetPurchaseOrdersByMaterialIdRequest() { MaterialId = materialId });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-purchase-order-by-branch-id/{branchId}")]
        [HasPermission(EnumPermission.ADMIN)]
        public async Task<IActionResult> GetPurchaseOrdersByBranchIdAsync(Guid branchId)
        {
            var response = await _mediator.Send(new GetPurchaseOrderByBranchIdRequest() { BranchId = branchId });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [HasPermission(EnumPermission.VIEW_PURCHASE)]
        [Route("get-purchase-order-by-supplier-id/{supplierId}")]
        public async Task<IActionResult> GetPurchaseOrderBySupplierIdAsync(Guid supplierId)
        {
            var response = await _mediator.Send(new GetPurchaseOrderBySupplierIdRequest() { SupplierId = supplierId });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [HasPermission(EnumPermission.CREATE_PURCHASE)]
        [Route("get-purchase-order-prepare-data")]
        public async Task<IActionResult> GetPurchaseOrderPrepareDataAsync()
        {
            var response = await _mediator.Send(new GetPurchaseOrderPrepareDataRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-purchase-order")]
        [HasPermission(EnumPermission.CREATE_PURCHASE)]
        public async Task<IActionResult> CreatePurchaseOrderAsync([FromBody] CreatePurchaseOrderRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-purchase-order-by-id")]
        [HasPermission(EnumPermission.EDIT_PURCHASE)]
        public async Task<IActionResult> UpdatePurchaseOrderByIdAsync([FromBody] UpdatePurchaseOrderByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("cancel-purchase-order-by-id")]
        [HasPermission(EnumPermission.CANCEL_PURCHASE)]
        public async Task<IActionResult> CancelPurchaseOrderByIdAsync([FromBody] CancelPurchaseOrderStatusByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("approve-purchase-order-by-id")]
        [HasPermission(EnumPermission.APPROVE_PURCHASE)]
        public async Task<IActionResult> ApprovePurchaseOrderByIdAsync([FromBody] ApprovePurchaseOrderStatusByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("complete-purchase-order-by-id")]
        [HasPermission(EnumPermission.IMPORT_GOODS)]
        public async Task<IActionResult> CompletePurchaseOrderByIdAsync([FromBody] CompletePurchaseOrderStatusByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
