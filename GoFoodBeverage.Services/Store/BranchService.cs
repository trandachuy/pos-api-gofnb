using AutoMapper;
using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoFoodBeverage.Services.Store
{
    [AutoService(typeof(IBranchService), Lifetime = ServiceLifetime.Scoped)]
    public class BranchService : IBranchService
    {

        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IMapper _mapper;

        public BranchService(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper, MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public IEnumerable<StoreBranch> GetBranches(Guid storeId, Guid? accountId)
        {
            var stores = _unitOfWork.Stores.GetStoresByAccountId(accountId).Where(x => x.Id == storeId).ToList();
            var branches = _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(storeId).AsNoTracking().ToList();

            ///return full branches if user is initialStore
            if (stores.Any()) {
                return branches;
            }

            ///filter branches if user is not initialStore or account not empty.
            if (accountId.HasValue || !stores.Any())
            {
                var staff = _unitOfWork.Staffs.GetStaffByAccountId(accountId.Value).AsNoTracking().FirstOrDefault();
                var staffGroupPermissionBranches = _unitOfWork.StaffGroupPermissionBranches
                                                                    .GetStaffGroupPermissionBranchesByStaffId(staff.Id)
                                                                    .AsNoTracking()
                                                                    .ToList();
                var isNotApplyAllBranches = staffGroupPermissionBranches.SelectMany(s => s.GroupPermissionBranches).All(i => i.IsApplyAllBranch == false);
                if (isNotApplyAllBranches)
                {
                    branches = staffGroupPermissionBranches.SelectMany(s => s.GroupPermissionBranches.Where(g => g.IsApplyAllBranch == false)
                        .Select(g => g.StoreBranch)).DistinctBy(b => b.Id).ToList();
                }
            }

            return branches;
        }

        

    }
}
