using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class DeleteCustomerMembershipByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCustomerMembershipRequestHandler : IRequestHandler<DeleteCustomerMembershipByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserProvider _userProvider;

        public DeleteCustomerMembershipRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(DeleteCustomerMembershipByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customerMembership = await _unitOfWork.CustomerMemberships.Find(c => c.StoreId == loggedUser.StoreId && c.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (customerMembership == null)
            {
                return false;
            }

            await _unitOfWork.CustomerMemberships.RemoveAsync(customerMembership);

            await _userActivityService.LogAsync(request);

            return true;
        }
    }
}
