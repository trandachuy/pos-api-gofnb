using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Suppliers.Queries
{
    public class GetSupplierByIdRequest : IRequest<GetSupplierByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetSupplierByIdResponse
    {
        public bool IsSuccess { get; set; }

        public SupplierModel Supplier { get; set; }
    }

    public class GetSupplierByIdRequestHandler : IRequestHandler<GetSupplierByIdRequest, GetSupplierByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSupplierByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetSupplierByIdResponse> Handle(GetSupplierByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var supplier = await _unitOfWork.Suppliers
                .GetSupplierByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);

            var supplierDetail = _mapper.Map<SupplierModel>(supplier);

            return new GetSupplierByIdResponse
            {
                IsSuccess = true,
                Supplier = supplierDetail
            };
        }
    }
}
