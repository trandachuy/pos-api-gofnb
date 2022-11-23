using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Areas.Commands
{
    public class DeleteAreaByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAreaRequestHandler : IRequestHandler<DeleteAreaByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAreaRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAreaByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var area = await  _unitOfWork.Areas.GetAllAreasInStore(loggedUser.StoreId).Include(x=>x.AreaTables).FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
            ThrowError.Against(area == null, "Area is not found");
            await _unitOfWork.Areas.RemoveAsync(area);
            return true;
        }
    }

}
