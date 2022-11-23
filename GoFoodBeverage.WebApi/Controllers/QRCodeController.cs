using GoFoodBeverage.Application.Features.QRCodes.Commands;
using GoFoodBeverage.Application.Features.QRCodes.Queries;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class QRCodeController : BaseApiController
    {
        public QRCodeController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-qr-code")]
        [HasPermission(EnumPermission.VIEW_QR_CODE)]
        public async Task<IActionResult> GetAllQRCodeAsync([FromQuery] GetAllQRCodeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-qr-code-by-id/{id}")]
        public async Task<IActionResult> GetQRCodeByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetQRCodeDetailByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-create-prepare-data")]
        [HasPermission(EnumPermission.CREATE_QR_CODE)]
        public async Task<IActionResult> GetCreateQRCodePrepareDataAsync([FromQuery] GetCreateQRCodePrepareDataRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-edit-prepare-data/{id}")]
        [HasPermission(EnumPermission.EDIT_QR_CODE)]
        public async Task<IActionResult> GetEditQRCodePrepareDataAsync(Guid id)
        {
            var response = await _mediator.Send(new GetEditQRCodePrepareDataRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-qr-code")]
        [HasPermission(EnumPermission.CREATE_QR_CODE)]
        public async Task<IActionResult> CreateQrCodeAsync([FromBody] CreateQRCodeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-qr-code")]
        [HasPermission(EnumPermission.EDIT_QR_CODE)]
        public async Task<IActionResult> UpdateQrCodeAsync([FromBody] UpdateQrCodeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("stop-qr-code-by-id/{id}")]
        [HasPermission(EnumPermission.STOP_QR_CODE)]
        public async Task<IActionResult> StopComboByIdAsync([FromRoute] StopQrCodeByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-qr-code-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_QR_CODE)]
        public async Task<IActionResult> DeleteComboByIdAsync([FromRoute] DeleteQrCodeByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}