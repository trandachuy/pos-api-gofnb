using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using Newtonsoft.Json.Linq;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Suppliers.Commands
{
    public class CreateSupplierRequest : IRequest<bool>
    {
        public SupplierModel Supplier { get; set; }
    }

    public class CreateSupplierRequestHandler : IRequestHandler<CreateSupplierRequest, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateSupplierRequestHandler(
            IMapper mapper,
            IMediator mediator,
            IUnitOfWork unitOfWork, 
            IUserProvider userProvider, 
            IUserActivityService userActivityService            
        )
        {
            _mapper = mapper;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateSupplierRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggerUser.StoreId.HasValue, "Cannot find store information");
            await CheckUniqueAndValidation(request, loggerUser.StoreId.Value);
            var newAddress = CreateSupplierAddress(request);
            var newSupplier = CreateSupplier(request, loggerUser.StoreId.Value, newAddress);
            newSupplier.CreatedUser = loggerUser.AccountId;
            await _unitOfWork.Suppliers.AddAsync(newSupplier);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Supplier,
                ActionType = EnumActionType.Created,
                ObjectId = newSupplier.Id,
                ObjectName = newSupplier.Name
            });

            return true;
        }

        private async Task CheckUniqueAndValidation(CreateSupplierRequest request, Guid storeId)
        {
            var supplierNameExisted = await _unitOfWork.Suppliers
                .GetAllSuppliersInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Name.ToLower() == request.Supplier.Name.ToLower());
            ThrowError.Against(supplierNameExisted != null, new JObject()
            {
                { $"{nameof(request.Supplier.Name)}", "This supplier name is already existed" },
            });

            var supplierCodeExisted = await _unitOfWork.Suppliers
                .GetAllSuppliersInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code.ToLower() == request.Supplier.Code.ToLower());
            ThrowError.Against(supplierCodeExisted != null, new JObject()
            {
                { $"{nameof(request.Supplier.Code)}", "Supplier code is already existed" },
            });
        }

        private static Address CreateSupplierAddress(CreateSupplierRequest request)
        {
            var address = new Address()
            {
                CountryId = request?.Supplier?.Address?.CountryId,
                CityId = request?.Supplier?.Address?.City?.Id,
                DistrictId = request?.Supplier?.Address?.District?.Id,
                WardId = request?.Supplier?.Address?.Ward?.Id,
                Address1 = request?.Supplier?.Address?.Address1,
                Address2 = request?.Supplier?.Address?.Address2,
                CityTown = request?.Supplier?.Address?.CityTown,
                PostalCode = request?.Supplier?.Address?.PostalCode,
                StateId = request?.Supplier?.Address?.State?.Id
            };

            return address;
        }

        private static Supplier CreateSupplier(CreateSupplierRequest request, Guid? storeId, Address address)
        {
            var newSupplier = new Supplier()
            {
                StoreId = (Guid)storeId,
                Address = address,
                Name = request.Supplier.Name,
                Code = request.Supplier.Code,
                PhoneNumber = request.Supplier.PhoneNumber,
                Email = request?.Supplier?.Email,
                Description = request?.Supplier?.Description
            };

            return newSupplier;
        }
    }
}
