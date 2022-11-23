using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Tax.Commands
{
    public class CreateTaxRequest : IRequest<bool>
    {
        public EnumTaxType TaxTypeId { get; set; }

        public string Name { get; set; }

        public decimal Percentage { get; set; }

        public string Description { get; set; }
    }

    public class CreateTaxRequestHandler : IRequestHandler<CreateTaxRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateTaxRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateTaxRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            await RequestValidation(request, loggedUser.StoreId.Value);

            var tax = CreateTax(request, loggedUser.StoreId.Value, loggedUser.AccountId.Value);
            await _unitOfWork.Taxes.AddAsync(tax);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);
            return true;
        }

        private async Task RequestValidation(CreateTaxRequest request, Guid storeId)
        {
            var duplicatedData = await _unitOfWork.Taxes.Find(m => m.StoreId == storeId && m.Name.Equals(request.Name.Trim())).FirstOrDefaultAsync();
            ThrowError.Against(duplicatedData != null, "Tax is duplicated in database");
        }

        private Domain.Entities.Tax CreateTax(CreateTaxRequest request, Guid storeId, Guid accountId)
        {
            var tax = new Domain.Entities.Tax()
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                StoreId = storeId,
                TaxTypeId = request.TaxTypeId,
                Percentage = request.Percentage,
                CreatedUser = accountId
            };

            return tax;
        }
    }
}
