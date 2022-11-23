using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GoFoodBeverage.Application.Features.Fees.Commands
{
    public class DeleteFeeByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteFeeByIdRequestHandler : IRequestHandler<DeleteFeeByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserProvider _userProvider;

        public DeleteFeeByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(DeleteFeeByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var fee = await _unitOfWork.Fees.Find(o => o.StoreId == loggedUser.StoreId && o.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (fee == null)
            {
                return false;
            }

            await _unitOfWork.Fees.RemoveAsync(fee);
            await _userActivityService.LogAsync(request);

            return true;
        }
    }
}
