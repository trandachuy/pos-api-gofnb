using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.POS.Models.Combo;
using System.Linq;

namespace GoFoodBeverage.POS.Application.Features.Combos.Queries
{
    public class GetAllComboActivatedRequest : IRequest<GetAllComboActivatedResponse>
    {
    }

    public class GetAllComboActivatedResponse
    {
        public IEnumerable<ComboDataTableModel> Combos { get; set; }
    }

    public class GetAllComboActivatedRequestHandler : IRequestHandler<GetAllComboActivatedRequest, GetAllComboActivatedResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllComboActivatedRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetAllComboActivatedResponse> Handle(GetAllComboActivatedRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var combos = await _unitOfWork.Combos
                .GetAllCombosInStoreInclude(loggedUser.StoreId.Value)
                .Include(c => c.ComboStoreBranches)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var allCombosStoreBranch = combos.Where(c => c.IsShowAllBranches == true || (c.IsShowAllBranches == false
                    && c.ComboStoreBranches.Select(csb => csb.BranchId).Contains(loggedUser.BranchId.Value)));

            var result = _mapper.Map<List<ComboDataTableModel>>(allCombosStoreBranch);

            //Get default OriginalPrice and SellingPrice for combos display on POS's dashboard
            foreach (var combo in result)
            {
                if (combo.ComboTypeId == Domain.Enums.EnumComboType.Flexible)
                {
                    var comboProductGroupProductPrices = combo.ComboProductGroups.Select(x => x.ComboProductGroupProductPrices);

                    var defaultComboProductGroupProductPriceIds = comboProductGroupProductPrices.Select(x => x.FirstOrDefault().ProductPriceId).ToList();

                    var comboPricingProducts = combo.ComboPricings.Select(x => x.ComboPricingProducts).ToList();

                    var defaultComboPricingProducts = comboPricingProducts
                        .FirstOrDefault(x => x.All(y => defaultComboProductGroupProductPriceIds.Contains(y.ProductPriceId)));

                    if (defaultComboPricingProducts != null && defaultComboPricingProducts.Count > 0)
                    {
                        var comboPricingId = defaultComboPricingProducts.FirstOrDefault().ComboPricingId;
                        var defaultComboPricing = combo.ComboPricings.FirstOrDefault(x => x.Id == comboPricingId);
                        combo.SellingPrice = defaultComboPricing.SellingPrice;
                        combo.OriginalPrice = defaultComboPricing.OriginalPrice;
                    }
                }
            }

            var response = new GetAllComboActivatedResponse()
            {
                Combos = result
            };

            return response;
        }
    }
}
