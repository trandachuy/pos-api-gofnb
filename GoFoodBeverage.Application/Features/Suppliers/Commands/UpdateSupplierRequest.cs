using System;
using MediatR;
using AutoMapper;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Suppliers.Commands
{
    public class UpdateSupplierRequest : IRequest<bool>
    {
        public SupplierModel Supplier { get; set; }
    }

    public class UpdateSupplierRequestHandler : IRequestHandler<UpdateSupplierRequest, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateSupplierRequestHandler(
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

        public async Task<bool> Handle(UpdateSupplierRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);

            var supplier = await _unitOfWork.Suppliers.GetSupplierByIdInStoreAsync(request.Supplier.Id, loggerUser.StoreId.Value);
            ThrowError.Against(supplier == null, "Cannot find supplier information");

            await CheckUniqueAndValidation(request, loggerUser.StoreId.Value);

            var modifiedAdress = UpdateSupplierAddress(supplier.Address, request);
            var modifiedSupplier = UpdateSupplier(supplier, request, modifiedAdress);
            await _unitOfWork.Suppliers.UpdateAsync(modifiedSupplier);

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Supplier,
                ActionType = EnumActionType.Edited,
                ObjectId = modifiedSupplier.Id,
                ObjectName = modifiedSupplier.Name
            });

            return true;
        }

        private async Task CheckUniqueAndValidation(UpdateSupplierRequest request, Guid storeId)
        {
            var supplierNameExisted = await _unitOfWork.Suppliers
                .GetAllSuppliersInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id != request.Supplier.Id && s.Name.ToLower() == request.Supplier.Name.ToLower());
            ThrowError.Against(supplierNameExisted != null, new JObject()
            {
                { $"{nameof(request.Supplier.Name)}", "Supplier name is already existed" },
            });

            var supplierCodeExisted = await _unitOfWork.Suppliers
                .GetAllSuppliersInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id != request.Supplier.Id && s.Code.ToLower() == request.Supplier.Code.ToLower());
            ThrowError.Against(supplierCodeExisted != null, new JObject()
            {
                { $"{nameof(request.Supplier.Code)}", "Supplier code is already existed" },
            });
        }

        private static Supplier UpdateSupplier(Supplier currentSupplier, UpdateSupplierRequest request, Address address)
        {
            currentSupplier.Code = request.Supplier.Code;
            currentSupplier.Name = request.Supplier.Name;
            currentSupplier.PhoneNumber = request.Supplier.PhoneNumber;
            currentSupplier.Email = request.Supplier.Email;
            currentSupplier.Description = request.Supplier.Description;
            currentSupplier.Address = address;

            return currentSupplier;
        }

        private static Address UpdateSupplierAddress(Address currentAddress, UpdateSupplierRequest request)
        {
            currentAddress.Address1 = request?.Supplier?.Address?.Address1;
            currentAddress.Address2 = request?.Supplier?.Address?.Address2;
            currentAddress.CountryId = request?.Supplier?.Address?.CountryId;
            currentAddress.CityId = request?.Supplier?.Address?.City?.Id;
            currentAddress.DistrictId = request?.Supplier?.Address?.District?.Id;
            currentAddress.WardId = request?.Supplier?.Address?.Ward?.Id;
            currentAddress.StateId = request?.Supplier?.Address.State?.Id;
            currentAddress.CityTown = request?.Supplier?.Address?.CityTown;
            currentAddress.PostalCode = request?.Supplier?.Address?.PostalCode;

            return currentAddress;
        }
    }
}