using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Units.Commands
{
    public class CreateUnitRequest : IRequest<CreateUnitResponse>
    {
        public string Name { get; set; }
    }

    public class CreateUnitResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateUnitRequestHandler : IRequestHandler<CreateUnitRequest, CreateUnitResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateUnitRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<CreateUnitResponse> Handle(CreateUnitRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            RequestValidation(request);

            var unitNameExisted = await _unitOfWork.Units.Find(g => g.StoreId == loggedUser.StoreId && g.Name == request.Name)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.Against(unitNameExisted != null, "Name of unit has already existed");

            var newUnit = await CreateUnitAsync(request, store, loggedUser.AccountId);
            await _unitOfWork.Units.AddAsync(newUnit);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            var response = new CreateUnitResponse
            {
                Id = newUnit.Id,
                Name = newUnit.Name
            };

            return response;
        }

        private static void RequestValidation(CreateUnitRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter name of unit.");
        }

        private async Task<Domain.Entities.Unit> CreateUnitAsync(CreateUnitRequest request, Store store, Guid? accountId)
        {
            var totalRecords = await _unitOfWork.Units.GetAll().CountAsync();
            var newUnit = new Domain.Entities.Unit()
            {
                StoreId = store.Id,
                Name = request.Name,
                CreatedUser = accountId,
                Position = totalRecords
            };

            return newUnit;
        }
    }
}
