using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCurrentCustomerRequest : IRequest<GetCurrentCustomerResponse> { }

    public class GetCurrentCustomerResponse
    {
        public string Fullname { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class GetCurrentCustomerRequestHandler : IRequestHandler<GetCurrentCustomerRequest, GetCurrentCustomerResponse>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetCurrentCustomerRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetCurrentCustomerResponse> Handle(GetCurrentCustomerRequest request, CancellationToken cancellationToken)
        {
            var response = new GetCurrentCustomerResponse();
            var currentCustomer = _userProvider.GetLoggedCustomer();

            var customerInformation = await _unitOfWork.Customers.GetCustomerByIdAsync(currentCustomer.Id ?? Guid.Empty);
            if (customerInformation == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "The account doesn't exist in the system";
                return response;
            }

            response.PhoneNumber = customerInformation.PhoneNumber;
            response.Fullname = string.IsNullOrEmpty(customerInformation.FullName) ? $"{customerInformation.FirstName} {customerInformation.LastName}" : customerInformation.FullName;
            response.Email = customerInformation.Email;
            response.AvatarUrl = customerInformation.Thumbnail;
            response.IsSuccess = true;

            return response;
        }
    }
}
