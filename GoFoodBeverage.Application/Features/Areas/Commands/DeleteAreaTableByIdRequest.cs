using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Areas.Commands
{
    public class DeleteAreaTableByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAreaTableRequestHandler : IRequestHandler<DeleteAreaTableByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAreaTableRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAreaTableByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var areaTable = await _unitOfWork.AreaTables
                .GetAllAreaTablesByStoreId(loggedUser.StoreId)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
            ThrowError.Against(areaTable == null, "AreaTable is not found");
            await _unitOfWork.AreaTables.RemoveAsync(areaTable);
            return true;
        }
    }

}
