using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Fees.Commands
{
    public class CreateFeeRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public bool IsShowAllBranches { get; set; }

        public decimal Value { get; set; }

        public bool IsPercentage { get; set; }

        public string Description { get; set; }

        public List<Guid> FeeBranchIds { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsAutoApplied { get; set; }

        public List<EnumOrderType> ServingTypes { get; set; }
    }

    public class CreateFeeRequestHandler : IRequestHandler<CreateFeeRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateFeeRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateFeeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            bool feeNameHasExistedInStore = _unitOfWork.Fees.GetAll().Any(x => x.StoreId == store.Id && x.Name.ToLower() == request.Name.ToLower());
            ThrowError.Against(feeNameHasExistedInStore, "The fee name already exists in the store.");

            RequestValidation(request, loggedUser.StoreId.Value);
            var listAllBranchIds = new List<Guid>();
            if (request.IsShowAllBranches)
            {
                listAllBranchIds = _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(loggedUser.StoreId).Select(x => x.Id).ToList();
            }
            var newFee = CreateFee(request, store, listAllBranchIds);
            await _unitOfWork.Fees.AddAsync(newFee);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(CreateFeeRequest request, Guid storeId)
        {
            ThrowError.Against(!request.IsShowAllBranches && request.FeeBranchIds.Count() == 0, "Please select branch.");
        }

        private static Fee CreateFee(CreateFeeRequest request, Store store, List<Guid> listAllBranchIds)
        {
            var newFee = new Fee()
            {
                StoreId = store.Id,
                Name = request.Name,
                IsShowAllBranches = request.IsShowAllBranches,
                IsPercentage = request.IsPercentage,
                Value = request.Value,
                Description = request.Description,
                StartDate = request.StartDate.HasValue? request.StartDate.Value.StartOfDay().ToUniversalTime(): null,
                EndDate = request.EndDate.HasValue ? request.EndDate.Value.EndOfDay().ToUniversalTime() : null,
                FeeBranches = new List<FeeBranch>(),
                IsAutoApplied = request.IsAutoApplied,
                FeeServingTypes = new List<FeeServingType>()
            };

            if (!request.IsShowAllBranches)
            {
                request.FeeBranchIds.ForEach(feeBranchId =>
                {
                    var feeBranche = new FeeBranch()
                    {
                        FeeId = newFee.Id,
                        BranchId = feeBranchId,
                        StoreId = store.Id,
                    };
                    newFee.FeeBranches.Add(feeBranche);
                });
            }
            else
            {
                listAllBranchIds.ForEach(feeBranchId =>
                {
                    var feeBranche = new FeeBranch()
                    {
                        FeeId = newFee.Id,
                        BranchId = feeBranchId,
                        StoreId = store.Id,
                    };
                    newFee.FeeBranches.Add(feeBranche);
                });
            }

            // Add record to table FeeServingType
            request.ServingTypes.ForEach(servingTypeId =>
            {
                var feeServingType = new FeeServingType
                {
                    FeeId = newFee.Id,
                    OrderServingType = servingTypeId,
                    StoreId = store.Id,
                };
                newFee.FeeServingTypes.Add(feeServingType);
            });

            return newFee;
        }
    }
}
