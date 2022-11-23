using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Payments.Commands
{
    public class EnablePaymentConfigRequest : IRequest<bool>
    {
        public EnumPaymentMethod EnumId { get; set; }

        public bool IsActive { get; set; }
    }

    public class EnablePaymentConfigRequestHandler : IRequestHandler<EnablePaymentConfigRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public EnablePaymentConfigRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(EnablePaymentConfigRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var paymentConfigExisted = await _unitOfWork.PaymentConfigs
                   .Find(p => p.StoreId == loggedUser.StoreId && p.PaymentMethodEnumId == request.EnumId)
                   .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (paymentConfigExisted == null)
            {
                return false;
            }

            paymentConfigExisted.IsActivated = request.IsActive;
            _unitOfWork.PaymentConfigs.Update(paymentConfigExisted);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
