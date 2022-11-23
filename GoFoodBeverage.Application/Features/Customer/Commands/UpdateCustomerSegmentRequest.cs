using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Customer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class UpdateCustomerSegmentRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsAllMatch { get; set; }

        public List<CustomerSegmentConditionDataModel> Conditions { get; set; }
    }

    public class UpdateCustomerSegmentRequestHandler : IRequestHandler<UpdateCustomerSegmentRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateCustomerSegmentRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateCustomerSegmentRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customerSegment = await _unitOfWork.CustomerSegments.GetCustomerSegmentDetailByIdAsync(request.Id, loggedUser.StoreId);
            ThrowError.Against(customerSegment == null, "Cannot find customer segment information");

            await RequestValidation(request, loggedUser.StoreId.Value);

            var modifiedCustomerSegment = await UpdateCustomerSegment(customerSegment, request);

            await _unitOfWork.CustomerSegments.UpdateAsync(modifiedCustomerSegment);

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.CustomerSegment,
                ActionType = EnumActionType.Edited,
                ObjectId = modifiedCustomerSegment.Id,
                ObjectName = modifiedCustomerSegment.Name
            });

            return true;
        }

        private async Task RequestValidation(UpdateCustomerSegmentRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}", "Please enter customer segment name" },
            });

            ThrowError.Against(request.Conditions.Count == 0, "Condition cannot be empty");

            var customerSegmentNameExisted = await _unitOfWork.CustomerSegments.CheckExistCustomerSegmentNameInStoreAsync(request.Id, request.Name, storeId);
            ThrowError.Against(customerSegmentNameExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}", "Customer segment name has already existed" },
            });

            ThrowError.Against(IsAnyConditionDuplicated(request.Conditions) == true, new JObject()
            {
                { $"{nameof(request.IsAllMatch)}", "The condition is existed" },
            });
        }

        private static bool IsAnyConditionDuplicated(List<CustomerSegmentConditionDataModel> conditions)
        {
            foreach (var condition in conditions)
            {
                if (IsDuplicatedCondition(conditions, condition)) return true;
            }

            return false;
        }

        private static bool IsDuplicatedCondition(List<CustomerSegmentConditionDataModel> conditions, CustomerSegmentConditionDataModel condition)
        {
            var count = conditions.Count(c => c.ObjectiveId == condition.ObjectiveId
                && c.CustomerDataId == condition.CustomerDataId
                && c.OrderDataId == condition.OrderDataId
                && c.RegistrationDateConditionId == condition.RegistrationDateConditionId
                && c.RegistrationTime?.Date == condition.RegistrationTime?.Date
                && c.Birthday == condition.Birthday
                && (c.IsMale ?? false) == (condition.IsMale ?? false)
                && c.TagId == condition.TagId
            );

            return count > 1;
        }

        public async Task<CustomerSegment> UpdateCustomerSegment(CustomerSegment currentCustomerSegment, UpdateCustomerSegmentRequest request)
        {
            currentCustomerSegment.Name = request.Name;
            currentCustomerSegment.IsAllMatch = request.IsAllMatch;

            #region Update condition
            var currentConditions = currentCustomerSegment.CustomerSegmentConditions.ToList();
            var newConditions = new List<CustomerSegmentCondition>();
            var updateConditions = new List<CustomerSegmentCondition>();

            if (request.Conditions != null && request.Conditions.Any())
            {
                var conditionDeleteItems = currentConditions
                    .Where(x => x.CustomerSegmentId == currentCustomerSegment.Id && !request.Conditions.Select(c => c.Id).Contains(x.Id));
                if (conditionDeleteItems.Any())
                {
                    _unitOfWork.CustomerSegmentConditions.RemoveRange(conditionDeleteItems);
                }

                foreach (var condition in request.Conditions)
                {
                    var conditionExisted = currentConditions.FirstOrDefault(x => x.Id == condition.Id);
                    if (conditionExisted == null)
                    {
                        var newCondition = new CustomerSegmentCondition()
                        {
                            CustomerSegmentId = currentCustomerSegment.Id,
                            ObjectiveId = condition.ObjectiveId,
                            CustomerDataId = condition.CustomerDataId,
                            OrderDataId = condition.OrderDataId,
                            RegistrationDateConditionId = condition.RegistrationDateConditionId,
                            RegistrationTime = condition.RegistrationTime,
                            Birthday = condition.Birthday,
                            IsMale = condition.IsMale,
                            TagId = condition.TagId,
                            StoreId = currentCustomerSegment.StoreId,
                        };
                        newConditions.Add(newCondition);
                    }
                    else
                    {
                        conditionExisted.ObjectiveId = condition.ObjectiveId;
                        conditionExisted.CustomerDataId = condition.CustomerDataId;
                        conditionExisted.OrderDataId = condition.OrderDataId;
                        conditionExisted.RegistrationDateConditionId = condition.RegistrationDateConditionId;
                        conditionExisted.RegistrationTime = condition.RegistrationTime;
                        conditionExisted.Birthday = condition.Birthday;
                        conditionExisted.IsMale = condition.IsMale;
                        conditionExisted.TagId = condition.TagId;

                        updateConditions.Add(conditionExisted);
                    }
                }

                _unitOfWork.CustomerSegmentConditions.AddRange(newConditions);
                _unitOfWork.CustomerSegmentConditions.UpdateRange(updateConditions);

                await _unitOfWork.SaveChangesAsync();
            }
            #endregion

            return currentCustomerSegment;
        }
    }
}
