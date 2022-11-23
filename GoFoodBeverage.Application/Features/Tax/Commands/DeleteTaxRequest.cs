using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Tax.Commands
{
    public class DeleteTaxByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteTaxByIdRequestHandler : IRequestHandler<DeleteTaxByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaxByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteTaxByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var tax = await _unitOfWork.Taxes.Find(m => m.StoreId == loggedUser.StoreId && m.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.Against(tax == null, "Tax is not found");
            await _unitOfWork.Taxes.RemoveAsync(tax);
            return true;
        }
    }

}
