using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Customer;
using Newtonsoft.Json.Linq;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class CreateCustomerSegmentRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public bool IsAllMatch { get; set; } = false;

        public List<CustomerSegmentConditionDataModel> Conditions { get; set; }
    }

    public class CreateCustomerSegmentRequestHandler : IRequestHandler<CreateCustomerSegmentRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateCustomerSegmentRequestHandler(
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

        public async Task<bool> Handle(CreateCustomerSegmentRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            await CheckUniqueAndValidationAsync(request, loggedUser.StoreId.Value);

            var customerSegment = CreateCustomerSegment(request, loggedUser.StoreId.Value);
            await _unitOfWork.CustomerSegments.AddAsync(customerSegment);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.CustomerSegment,
                ActionType = EnumActionType.Created,
                ObjectId = customerSegment.Id,
                ObjectName = customerSegment.Name
            });

            return true;
        }

        private async Task CheckUniqueAndValidationAsync(CreateCustomerSegmentRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}", "Please enter name" },
            });

            ThrowError.Against(request.Conditions.Count == 0, "Condition cannot be empty");

            var customerSegmentNameExisted = await _unitOfWork.CustomerSegments.GetCustomerSegmentByNameInStoreAsync(request.Name, storeId);
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

        private static CustomerSegment CreateCustomerSegment(CreateCustomerSegmentRequest request, Guid storeId)
        {
            var newCustomerSegment = new CustomerSegment()
            {
                Name = request.Name,
                IsAllMatch = request.IsAllMatch,
                StoreId = storeId,
                CustomerSegmentConditions = new List<CustomerSegmentCondition>()
            };

            request.Conditions.ForEach(c =>
            {
                var condition = new CustomerSegmentCondition()
                {
                    CustomerSegmentId = newCustomerSegment.Id,
                    ObjectiveId = c.ObjectiveId,
                    CustomerDataId = c.CustomerDataId,
                    OrderDataId = c.OrderDataId,
                    RegistrationDateConditionId = c.RegistrationDateConditionId,
                    RegistrationTime = c.RegistrationTime,
                    Birthday = c.Birthday,
                    IsMale = c.IsMale,
                    TagId = c.TagId,
                    StoreId = storeId
                };
                newCustomerSegment.CustomerSegmentConditions.Add(condition);
            });

            return newCustomerSegment;
        }
    }
}
