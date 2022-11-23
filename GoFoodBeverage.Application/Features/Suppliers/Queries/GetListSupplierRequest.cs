using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Suppliers.Queries
{
    public class GetListSupplierRequest : IRequest<GetListSupplierResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetListSupplierResponse
    {
        public IEnumerable<SupplierModel> Suppliers { get; set; }

        public int Total { get; set; }
    }

    public class GetListSupplierRequestHandler : IRequestHandler<GetListSupplierRequest, GetListSupplierResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IMapper _mapper;

        public GetListSupplierRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
            _mapper = mapper;
        }

        public async Task<GetListSupplierResponse> Handle(GetListSupplierRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            
            var listSupplier = _unitOfWork.Suppliers
                .GetAllSuppliersInStore(loggedUser.StoreId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(request.KeySearch) && listSupplier != null)
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                listSupplier = listSupplier.Where(s => s.Code.ToLower().Contains(keySearch)
                                                || s.Name.ToLower().Contains(keySearch)
                                                || s.PhoneNumber.ToLower().Contains(keySearch)
                                                || s.Email.ToLower().Contains(keySearch));
            }

            var listSupplierOrdered = listSupplier.OrderByDescending(s => s.CreatedTime);
            var listSupplierByPaging = await listSupplierOrdered.ToPaginationAsync(request.PageNumber, request.PageSize);
            var listSupplierModels = _mapper.Map<IEnumerable<SupplierModel>>(listSupplierByPaging.Result);

            var response = new GetListSupplierResponse()
            {
                Suppliers = listSupplierModels,
                Total = listSupplierByPaging.Total
            };

            return response;
        }
    }
}
