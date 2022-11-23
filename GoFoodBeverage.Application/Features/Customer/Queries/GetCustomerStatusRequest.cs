using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerStatusRequest : IRequest<GetCustomerStatusResponse>
    {
    }

    public class GetCustomerStatusResponse
    {
        public bool IsSuccess { get; set; }

        public bool Status { get; set; }
    }

    public class GetCustomerStatusRequestHandler : IRequestHandler<GetCustomerStatusRequest, GetCustomerStatusResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        public GetCustomerStatusRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        /// <summary>
        /// This method is used to handle the current HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns>GetCustomerStatusResponse</returns>
        public async Task<GetCustomerStatusResponse> Handle(GetCustomerStatusRequest request, CancellationToken cancellationToken)
        {
            var dataToResponse = new GetCustomerStatusResponse();
            try
            {
                // Get customer information from token.
                var loggedUser = _userProvider.GetLoggedCustomer();

                // Get the current customer status from the database by the customer information.
                var customerStatus = await _unitOfWork.Accounts.GetAccountStatusByIdAsync(loggedUser.Id.Value);
                dataToResponse.IsSuccess = true;
                dataToResponse.Status = customerStatus;
            }
            catch { }

            return dataToResponse;
        }
    }
}
