using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Staffs.Commands
{
    public class DeleteStaffByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteStaffByIdRequestHandler : IRequestHandler<DeleteStaffByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteStaffByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteStaffByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var staff = await _unitOfWork.Staffs.GetStaffById(request.Id, loggedUser.StoreId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(staff == null, "Staff is not found");
            //Checked user staff of admin for store.
            var storeExisted = (await _unitOfWork.Stores.GetStoresByAccountId(staff.AccountId).AsNoTracking().ToArrayAsync(cancellationToken: cancellationToken)).Any();
            ThrowError.Against(storeExisted, "Staff is deleted unsuccessfully");
            staff.IsDeleted = true;
            var staffAccount = await _unitOfWork.Accounts.GetIdentifierAsync(staff.AccountId);
            staffAccount.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
