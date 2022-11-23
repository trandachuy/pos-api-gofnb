using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Stores.Queries;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class AddressController : BaseApiController
    {
        public AddressController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        /// <summary>
        /// This action is used to get all countries from the database.
        /// </summary>
        /// <param name="query">The object data, it is mapped from HTTP request.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-countries")]
        public async Task<IActionResult> GetCountries([FromQuery] GetCountriesRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-cities-by-countryid")]
        public async Task<IActionResult> GetCitiesByCountryIdAsync([FromQuery] GetCitiesByCountryIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-districts-by-cityid")]
        public async Task<IActionResult> GetDistrictsByCityIdAsync([FromQuery] GetDistrictsByCityIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-wards-by-districtid")]
        public async Task<IActionResult> GetWardsByDistrictIdAsync([FromQuery] GetWardsByDistrictIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}
