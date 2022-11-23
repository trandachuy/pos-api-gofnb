using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Combo;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Product;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetComboProductByComboIdRequest : IRequest<GetComboProductByComboIdResponse>
    {
        public Guid StoreId { get; set; }

        public Guid ComboId { get; set; }

        public bool IsComboPricing { get; set; }

        public Guid? ComboPricingId { get; set; }
    }

    public class GetComboProductByComboIdResponse
    {
        public List<ComboActivatedModel.ComboProductPriceModel> ComboProductPrices { get; set; }

        public ComboActivatedModel.ComboPricingModel ComboPricing { get; set; }
    }

    public class GetComboProductByComboIdRequestHandler : IRequestHandler<GetComboProductByComboIdRequest, GetComboProductByComboIdResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetComboProductByComboIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetComboProductByComboIdResponse> Handle(GetComboProductByComboIdRequest request, CancellationToken cancellationToken)
        {
            var comboPricingResponse = new ComboActivatedModel.ComboPricingModel();
            var comboProductPricesResponse = new List<ComboActivatedModel.ComboProductPriceModel>();

            var allProductToppingInStore = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(request.StoreId)
                .Select(p => new ProductToppingModel { Id = p.Id, Name = p.Name, Price = p.ProductPrices.FirstOrDefault().PriceValue, Thumbnail = p.Thumbnail })
                .ToListAsync();
            var productToppingIds = allProductToppingInStore.Select(pt => pt.Id).ToList();

            var productToppings = await _unitOfWork.ProductToppings.GetAll().Where(pt => productToppingIds.Contains(pt.ToppingId))
                       .Select(pt => new { pt.ProductId, pt.ToppingId })
                       .ToListAsync();

            if (request.IsComboPricing)
            {
                var comboPricing = await _unitOfWork.ComboPricings
                    .Find(cp => cp.Id == request.ComboPricingId.Value && cp.ComboId == request.ComboId)
                    .Include(cp => cp.ComboPricingProducts).ThenInclude(cpp => cpp.ProductPrice).ThenInclude(p => p.Product).ThenInclude(po => po.ProductOptions).ThenInclude(o => o.Option).ThenInclude(o => o.OptionLevel)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                comboPricingResponse = _mapper.Map<ComboActivatedModel.ComboPricingModel>(comboPricing);

                foreach (var comboPricingProduct in comboPricingResponse.ComboPricingProducts)
                {
                    var toppingIdsNew = productToppings.Where(pt => pt.ProductId == comboPricingProduct.ProductPrice.ProductId).Select(pt => pt.ToppingId).ToList();
                    var productToppingsModel = allProductToppingInStore.Where(pt => toppingIdsNew.Contains(pt.Id)).ToList();
                    comboPricingProduct.ProductPrice.Product.ProductToppings = productToppingsModel;
                }
            } else
            {
                var comboProductPrices = await _unitOfWork.ComboProductPrices
                    .Find(c => c.ComboId == request.ComboId)
                        .Include(pr => pr.ProductPrice).ThenInclude(p => p.Product).ThenInclude(po => po.ProductOptions).ThenInclude(o => o.Option).ThenInclude(o => o.OptionLevel)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                comboProductPricesResponse = _mapper.Map<List<ComboActivatedModel.ComboProductPriceModel>>(comboProductPrices);

                foreach (var aComboProductPrice in comboProductPricesResponse)
                {
                    var toppingIdsNew = productToppings.Where(pt => pt.ProductId == aComboProductPrice.ProductPrice.ProductId).Select(pt => pt.ToppingId).ToList();
                    var productToppingsModel = allProductToppingInStore.Where(pt => toppingIdsNew.Contains(pt.Id)).ToList();
                    aComboProductPrice.ProductPrice.Product.ProductToppings = productToppingsModel;
                }
            }

            return new GetComboProductByComboIdResponse
            {
                ComboPricing = comboPricingResponse,
                ComboProductPrices = comboProductPricesResponse
            };
        }
    }
}
