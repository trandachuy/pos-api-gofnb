using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class OrderSessionController : BaseApiController
    {
        public OrderSessionController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }
    }
}
