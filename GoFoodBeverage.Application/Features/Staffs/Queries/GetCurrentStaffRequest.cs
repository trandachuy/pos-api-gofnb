using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;


namespace GoFoodBeverage.Application.Features.Staffs.Queries
{

    public class GetCurrentStaffRequest : IRequest<GetCurrentStaffResponse> { }

    public class GetCurrentStaffResponse
    {
        public Guid StaffId { get; set; }

        public Guid AccountId { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string StoreName { get; set; }

        public string Thumbnail { get; set; }
    }

    public class GetStaffByAccountIdRequestHandler : IRequestHandler<GetCurrentStaffRequest, GetCurrentStaffResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;


        public GetStaffByAccountIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }


        public async Task<GetCurrentStaffResponse> Handle(GetCurrentStaffRequest request, CancellationToken cancellationToken)
        {            
            var currentUser = await _userProvider.ProvideAsync(cancellationToken);
            var userInformation = await _unitOfWork.Staffs.GetStaffByIdAsync(currentUser.Id ?? Guid.Empty);
            ThrowError.Against(userInformation == null, "myAccount.notExist");

            var response = new GetCurrentStaffResponse()
            {
                StaffId = userInformation.Id,
                AccountId = userInformation.AccountId,
                FullName = userInformation.FullName,
                PhoneNumber = userInformation.PhoneNumber,
                Email = userInformation?.Account?.Username,
                StoreName = userInformation.Store?.Title,
                Thumbnail = userInformation.Thumbnail
            };

            return response;
        }
    }
}
