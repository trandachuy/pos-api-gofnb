using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Fee;
using GoFoodBeverage.Common.Exceptions;
using System.Linq;
using static GoFoodBeverage.Models.Fee.FeeDetailModel;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Fees.Queries
{
    public class GetFeeDetailByIdRequest : IRequest<GetFeeDetailByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetFeeDetailByIdResponse
    {
        public FeeDetailModel FeeDetail { get; set; }
    }

    public class GetFeeDetailByIdRequestHandler : IRequestHandler<GetFeeDetailByIdRequest, GetFeeDetailByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFeeDetailByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetFeeDetailByIdResponse> Handle(GetFeeDetailByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var fee = await _unitOfWork.Fees.GetFeeByIdInStoreAsync(request.Id.Value, loggedUser.StoreId.Value);
            ThrowError.Against(fee == null, "Cannot find fee information");

            var feeDetail = new FeeDetailModel
            {
                Id = fee.Id,
                Name = fee.Name,
                Value = fee.Value,
                IsPercentage =fee.IsPercentage,
                StartDate = fee.StartDate,
                EndDate = fee.EndDate,
                IsAutoApplied = fee.IsAutoApplied,
                IsShowAllBranches=fee.IsShowAllBranches,
                FeeBranches = fee.FeeBranches.Select(fb=>new StoreBranchDto { Code=fb.Branch.Code, Name=fb.Branch.Name}),
                ServingTypes = fee.FeeServingTypes.Select(st=> new ServingTypeDto
                {
                    Code = st.OrderServingType,
                    Name = Enum.GetName(typeof(EnumOrderType), st.OrderServingType)
                }),
                Description = fee.Description
            };

            return new GetFeeDetailByIdResponse
            {
                FeeDetail = feeDetail
            };
        }
    }
}
