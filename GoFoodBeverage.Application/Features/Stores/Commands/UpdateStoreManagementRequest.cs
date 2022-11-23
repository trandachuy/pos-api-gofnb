using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Staff;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class UpdateStoreManagementRequest : IRequest<bool>
    {
        public StoreModel Store { get; set; }

        public StaffModel Staff { get; set; }

        public StoreBankAccountModel StoreBankAccount { get; set; }
    }

    public class UpdateStoreManagementRequestHandler : IRequestHandler<UpdateStoreManagementRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;

        public UpdateStoreManagementRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdateStoreManagementRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidationAsync(request);

            //Update Staff
            var staffModel = _unitOfWork.Staffs
                .Find(s => s.StoreId == loggedUser.StoreId && s.Id == request.Staff.Id)
                .FirstOrDefault();
            var staffUpdate = UpdateStoreStaff(staffModel, request);
            await _unitOfWork.Staffs.UpdateAsync(staffUpdate);

            //Update Address
            var addressModel = _unitOfWork.Addresses
                    .Find(a => a.Id == request.Store.AddressId)
                    .FirstOrDefault();
            var addressUpdate = UpdateStoreAddress(addressModel, request);
            await _unitOfWork.Addresses.UpdateAsync(addressUpdate);

            //Update Store
            var storeModel = await _unitOfWork.Stores
                .GetStoreByIdAsync(loggedUser.StoreId);
            var storeUpdate = UpdateStore(storeModel, request, loggedUser.AccountId.Value);
            await _unitOfWork.Stores.UpdateAsync(storeUpdate);

            //StoreBankAccount
            var storeBankAccount = await _unitOfWork.StoreBankAccounts
                .GetStoreBankAccountByStoreIdAsync(loggedUser.StoreId);

            if (storeBankAccount == null)
            {
                //Add New
                var storeBankAccountAddNew = CreateStoreBankAccount(request, loggedUser.AccountId.Value, loggedUser.StoreId.Value);

                await _unitOfWork.StoreBankAccounts.AddAsync(storeBankAccountAddNew);
            }
            else
            {
                //Update
                var storeBankAccountUpdate = UpdateStoreBankAccount(storeBankAccount, request, loggedUser.AccountId.Value);

                await _unitOfWork.StoreBankAccounts.UpdateAsync(storeBankAccountUpdate);
            }

            return true;
        }

        private static void RequestValidationAsync(UpdateStoreManagementRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Store.Title), "Please enter store name");
            ThrowError.Against(string.IsNullOrEmpty(request.Staff.FullName), "Please enter fullname");
            ThrowError.Against(string.IsNullOrEmpty(request.Staff.PhoneNumber), "Please enter phone number");
            ThrowError.BadRequestAgainstNull(request.Store.AddressId, "Please choosse country");
        }

        public static Staff UpdateStoreStaff(Staff staff, UpdateStoreManagementRequest request)
        {
            staff.FullName = request?.Staff?.FullName;
            staff.PhoneNumber = request?.Staff?.PhoneNumber;

            return staff;
        }

        public static Address UpdateStoreAddress(Address address, UpdateStoreManagementRequest request)
        {
            address.CountryId = request?.Store?.Address?.CountryId;
            address.StateId = request?.Store?.Address?.State?.Id;
            address.CityTown = request?.Store?.Address?.CityTown;
            address.CityId = request?.Store?.Address?.City?.Id;
            address.Address1 = request?.Store?.Address?.Address1;
            address.Address2 = request?.Store?.Address?.Address2;
            address.PostalCode = request?.Store?.Address?.PostalCode;
            address.DistrictId = request?.Store?.Address?.District?.Id;
            address.WardId = request?.Store?.Address?.Ward?.Id;

            return address;
        }

        public static Store UpdateStore(Store store, UpdateStoreManagementRequest request, Guid accountId)
        {
            store.Title = request.Store.Title;
            store.LastSavedUser = accountId;
            store.AddressId = request.Store.AddressId;
            store.IsStoreHasKitchen = request.Store.IsStoreHasKitchen;
            store.IsAutoPrintStamp = request.Store.IsAutoPrintStamp;
            store.IsPaymentLater = request.Store.IsPaymentLater;
            store.IsCheckProductSell = request.Store.IsCheckProductSell;

            return store;
        }

        public static StoreBankAccount CreateStoreBankAccount(UpdateStoreManagementRequest request, Guid accountId, Guid storeId)
        {
            var storeBankAccount = new StoreBankAccount()
            {
                StoreId = storeId,
                SwiftCode = request?.StoreBankAccount?.SwiftCode,
                RoutingNumber = request?.StoreBankAccount?.RoutingNumber,
                AccountHolder = request?.StoreBankAccount?.AccountHolder,
                AccountNumber = request?.StoreBankAccount?.AccountNumber,
                BankName = request?.StoreBankAccount?.BankName,
                BankBranchName = request?.StoreBankAccount.BankBranchName,
                CountryId = request.StoreBankAccount.CountryId,
                CityId = request.StoreBankAccount?.CityId,
                CreatedUser = accountId
            };

            return storeBankAccount;
        }

        public static StoreBankAccount UpdateStoreBankAccount(StoreBankAccount storeBankAccount, UpdateStoreManagementRequest request, Guid accountId)
        {
            storeBankAccount.SwiftCode = request?.StoreBankAccount?.SwiftCode;
            storeBankAccount.RoutingNumber = request?.StoreBankAccount?.RoutingNumber;
            storeBankAccount.AccountHolder = request?.StoreBankAccount?.AccountHolder;
            storeBankAccount.AccountNumber = request?.StoreBankAccount?.AccountNumber;
            storeBankAccount.BankName = request?.StoreBankAccount?.BankName;
            storeBankAccount.BankBranchName = request?.StoreBankAccount.BankBranchName;
            storeBankAccount.CountryId = request.StoreBankAccount.CountryId;
            storeBankAccount.CityId = request?.StoreBankAccount?.CityId;
            storeBankAccount.LastSavedUser = accountId;

            return storeBankAccount;
        }
    }
}
