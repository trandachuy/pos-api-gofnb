using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.PurchaseOrderModel;

namespace GoFoodBeverage.Application.Features.Settings.Queries
{
    public class GetGroupPermissionByIdRequest : IRequest<GetGroupPermissionByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetGroupPermissionByIdResponse
    {
        public bool IsSuccess { get; set; }

        public GetGroupPermissionByIdModel GroupPermission { get; set; }
    }

    public class GetGroupPermissionByIdRequestHandler : IRequestHandler<GetGroupPermissionByIdRequest, GetGroupPermissionByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetGroupPermissionByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetGroupPermissionByIdResponse> Handle(GetGroupPermissionByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var groupPermissionData = await _unitOfWork.GroupPermissions.GetGroupPermissionByIdInStoreAsync(request.Id.Value, loggedUser.StoreId.Value);
            var groupPermission = _mapper.Map<GetGroupPermissionByIdModel>(groupPermissionData);
            
            return new GetGroupPermissionByIdResponse
            {
                IsSuccess = true,
                GroupPermission = groupPermission
            };
        }
    }
}
