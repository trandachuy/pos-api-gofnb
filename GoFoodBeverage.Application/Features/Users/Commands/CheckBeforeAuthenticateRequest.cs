using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class CheckBeforeAuthenticateRequest : IRequest<CheckBeforeAuthenticateResponse>
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class CheckBeforeAuthenticateResponse
    {
        public bool Success { get; set; }

        public IEnumerable<StoreModel> Stores { get; set; }

        public class StoreModel
        {
            public Guid StoreId { get; set; }

            public string StoreName { get; set; }

            public string StoreThumbnail { get; set; }

            public Guid AccountId { get; set; }
        }
    }

    public class CheckBeforeAuthenticateRequestHanlder : IRequestHandler<CheckBeforeAuthenticateRequest, CheckBeforeAuthenticateResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckBeforeAuthenticateRequestHanlder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CheckBeforeAuthenticateResponse> Handle(CheckBeforeAuthenticateRequest request, CancellationToken cancellationToken)
        {
            ThrowError.ArgumentIsNull(request, request.UserName);
            ThrowError.ArgumentIsNull(request, request.Password);

            var hasher = new PasswordHasher<Domain.Entities.Account>();
            var accounts = await _unitOfWork.Accounts
                   .Find(a => a.Username.Trim().ToLower() == request.UserName.Trim().ToLower() && !a.IsDeleted)
                   .AsNoTracking()
                   .Select(a => new Domain.Entities.Account()
                   {
                       Id = a.Id,
                       Password = a.Password,
                   })
                   .ToListAsync(cancellationToken: cancellationToken);

            // find account validated
            var validAccounts = accounts.Where(acc => hasher.VerifyHashedPassword(null, acc.Password, request.Password) == PasswordVerificationResult.Success);

            var result = new CheckBeforeAuthenticateResponse
            {
                Success = validAccounts.Any()
            };

            // Get store list with account is admin
            result.Stores = await _unitOfWork.Stores.GetAll()
                    .Where(a => validAccounts.Select(account => account.Id).Contains(a.InitialStoreAccountId))
                    .AsNoTracking()
                    .Select(a => new CheckBeforeAuthenticateResponse.StoreModel
                    {
                        StoreId = a.Id,
                        StoreName = a.Title,
                        StoreThumbnail = string.Empty,
                        AccountId = a.InitialStoreAccountId
                    })
                    .ToListAsync(cancellationToken);

            // find store created by accounts
            var stores = await _unitOfWork.Stores
                .GetAll()
                .Where(store => validAccounts.Select(account => account.Id).Contains(store.InitialStoreAccountId))
                .AsNoTracking()
                .Select(store => new CheckBeforeAuthenticateResponse.StoreModel
                {
                    StoreId = store.Id,
                    StoreName = store.Title,
                    StoreThumbnail = string.Empty,
                    AccountId = store.InitialStoreAccountId
                })
                .ToListAsync(cancellationToken);

            // Get store list with account isn't admin
            if (!result.Stores.Any())
            {
                List<Staff> staffList = await _unitOfWork.Staffs.GetAll()
                    .Where(a => validAccounts.Select(account => account.Id).Contains(a.AccountId))
                    .Include(a => a.StaffGroupPermissionBranchs)
                    .ThenInclude(a => a.GroupPermissionBranches)
                    .ThenInclude(a => a.StoreBranch)
                    .ThenInclude(a => a.Store)
                    .AsNoTracking()
                    .Select(a => new Staff()
                    {
                        Id = a.Id,
                        AccountId = a.AccountId,
                        StaffGroupPermissionBranchs = a.StaffGroupPermissionBranchs
                    })
                    .ToListAsync(cancellationToken);

                foreach (Staff staff in staffList)
                {
                    var storeItem = staff.StaffGroupPermissionBranchs
                        .Where(a => a.StaffId == staff.Id)
                        .SelectMany(a => a.GroupPermissionBranches)
                        .Select(a => a.StoreBranch)
                        .Select(a => a.Store)
                        .Select(a => new CheckBeforeAuthenticateResponse.StoreModel
                        {
                            StoreId = a.Id,
                            StoreName = a.Title,
                            StoreThumbnail = string.Empty,
                            AccountId = staff.AccountId
                        })
                        .FirstOrDefault();

                    if (storeItem == null) continue;

                    stores.Add(storeItem);
                }

                result.Stores = stores;
            }

            return result;
        }
    }
}
