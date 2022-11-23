using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Combo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Combos.Queries
{
    public class GetComboByIdRequest : IRequest<GetComboByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetComboByIdResponse
    {
        public bool IsSuccess { get; set; }

        public ComboDetailPosOrderDto Combo { get; set; }
    }

    public class GetComboByIdRequestHandler : IRequestHandler<GetComboByIdRequest, GetComboByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetComboByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetComboByIdResponse> Handle(GetComboByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var comboData = await _unitOfWork.Combos
                .GetComboByIdWithoutTracking(loggedUser.StoreId.Value, request.Id.Value)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var response = new GetComboByIdResponse();
            if (comboData == null)
            {
                return response;
            }

            var combo = _mapper.Map<ComboDetailPosOrderDto>(comboData);
            if (combo.ComboPricings.Count > 0)
            {
                var productPriceIds = combo.ComboProductGroups
                    .Where(cpg => cpg.ComboProductGroupProductPrices.FirstOrDefault() != null)
                    .Select(p => p.ComboProductGroupProductPrices.FirstOrDefault().ProductPriceId)
                    .Distinct();

                var comboPricingSelectedList = combo.ComboPricings
                    .Where(c => c.ComboPricingProducts.Any(cpp => productPriceIds.Any(ppid => ppid == cpp.ProductPriceId)));

                if (comboPricingSelectedList.Any())
                {
                    var comboPricingSelected = comboPricingSelectedList.FirstOrDefault();

                    combo.OriginalPrice = comboPricingSelected.OriginalPrice;
                    combo.SellingPrice = comboPricingSelected.SellingPrice;
                    combo.ComboId = comboPricingSelected.ComboId;
                    combo.ComboName = comboPricingSelected.ComboName;
                    combo.ComboPricingId = comboPricingSelected.Id;
                }
            }

            response.IsSuccess = true;
            response.Combo = combo;

            return response;
        }
    }
}
