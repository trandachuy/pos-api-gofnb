using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class CreateCustomerMembershipRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public int AccumulatedPoint { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; } = 0;

        public decimal? MaximumDiscount { get; set; }

        public string Thumbnail { get; set; }
    }

    public class CreateCustomerMembershipHandler : IRequestHandler<CreateCustomerMembershipRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateCustomerMembershipHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateCustomerMembershipRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);

            CheckUniqueAndValidation(request, loggerUser.StoreId.Value);

            var newCustomerMembership = new CustomerMembershipLevel
            {
                Name = request.Name.Trim(),
                AccumulatedPoint = request.AccumulatedPoint,
                Description = request.Description?.Trim(),
                Discount = request.Discount,
                MaximumDiscount = request.MaximumDiscount,
                Thumbnail = request.Thumbnail,
                StoreId = loggerUser.StoreId.Value,
                CreatedUser = loggerUser.AccountId.Value,
            };

            await _unitOfWork.CustomerMemberships.AddAsync(newCustomerMembership);

            return true;
        }

        private void CheckUniqueAndValidation(CreateCustomerMembershipRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}",  "Please enter name"},
            });
            ThrowError.Against(request.AccumulatedPoint <= 0, new JObject()
            {
                { $"{nameof(request.AccumulatedPoint)}",  "Please enter accumulated Point"},
            });

            var membershipbyNameExisted = _unitOfWork.CustomerMemberships.CheckCustomerMemberShipByNameInStore(request.Name, storeId);
            ThrowError.Against(membershipbyNameExisted == true, new JObject()
            {
                { $"{nameof(request.Name)}",  "This name is already existed"},
            });

            var membershipbyAccumulatedPointExisted = _unitOfWork.CustomerMemberships.CheckCustomerMemberShipByAccumulatedPointInStore(request.AccumulatedPoint, storeId);
            ThrowError.Against(membershipbyAccumulatedPointExisted == true, new JObject()
            {
                { $"{nameof(request.AccumulatedPoint)}",  "This accumulated Point is already existed"},
            });
        }
    }
}
