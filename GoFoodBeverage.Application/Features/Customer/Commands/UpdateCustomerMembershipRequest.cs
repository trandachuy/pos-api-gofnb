using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class UpdateCustomerMembershipRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int AccumulatedPoint { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; } = 0;

        public decimal? MaximumDiscount { get; set; }

        public string Thumbnail { get; set; }
    }

    public class UpdateCustomerMembershipHandler : IRequestHandler<UpdateCustomerMembershipRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateCustomerMembershipHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateCustomerMembershipRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            CheckUniqueAndValidation(request, loggerUser.StoreId.Value);

            var currentMembership = await _unitOfWork.CustomerMemberships
                .GetAllCustomerMembershipInStore(loggerUser.StoreId)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
            var updatedMembership = UpdateCustomerMembership(currentMembership, request, loggerUser.AccountId.Value);

            await _unitOfWork.CustomerMemberships.UpdateAsync(updatedMembership);
            return true;
        }

        private void CheckUniqueAndValidation(UpdateCustomerMembershipRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}",  "Please enter name"},
            });
            ThrowError.Against(request.AccumulatedPoint <= 0, new JObject()
            {
                { $"{nameof(request.AccumulatedPoint)}",  "Please enter accumulated Point"},
            });

            var membershipExisted = _unitOfWork.CustomerMemberships
                .Find(m => m.StoreId == storeId && m.Name.Trim().ToLower().Equals(request.Name.Trim().ToLower()) && m.Id != request.Id)
                .FirstOrDefault();
            ThrowError.Against(membershipExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}",  "This name is already existed"},
            });

            var accumulatedPointExisted = _unitOfWork.CustomerMemberships
                .Find(m => m.StoreId == storeId && m.AccumulatedPoint == request.AccumulatedPoint && m.Id != request.Id)
                .FirstOrDefault();
            ThrowError.Against(accumulatedPointExisted != null, new JObject()
            {
                { $"{nameof(request.AccumulatedPoint)}",  "This accumulated Point is already existed"},
            });
        }

        private static CustomerMembershipLevel UpdateCustomerMembership(CustomerMembershipLevel currentData, UpdateCustomerMembershipRequest changedData, Guid loggedUserId)
        {
            currentData.Name = changedData.Name.Trim();
            currentData.AccumulatedPoint = changedData.AccumulatedPoint;
            currentData.Description = changedData.Description?.Trim();
            currentData.Discount = changedData.Discount;
            currentData.MaximumDiscount = changedData.MaximumDiscount;
            currentData.Thumbnail = changedData.Thumbnail;
            currentData.LastSavedUser = loggedUserId;
            return currentData;
        }
    }
}
