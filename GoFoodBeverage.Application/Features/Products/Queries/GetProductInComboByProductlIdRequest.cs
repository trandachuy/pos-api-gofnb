using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Product;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetProductInComboByProductlIdRequest : IRequest<GetProductInComboByProductlIdResponse>
    {
        public Guid ProductId { get; set; }
    }

    public class GetProductInComboByProductlIdResponse
    {
        public List<ComboInFoModel> Combos { get; set; }

        public bool IsEditProduct { get; set; }
    }

    public class GetProductInComboByProductlIdHandler : IRequestHandler<GetProductInComboByProductlIdRequest, GetProductInComboByProductlIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductInComboByProductlIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetProductInComboByProductlIdResponse> Handle(GetProductInComboByProductlIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var combos = await _unitOfWork.Combos
                .GetAllCombosInStoreInclude(loggedUser.StoreId.Value)
                .AsNoTracking()
                .Select(x=>new {x.Id, x.Name, x.ComboProductPrices, x.ComboProductGroups })
                .ToListAsync(cancellationToken: cancellationToken);

            var listCombo = new List<ComboInFoModel>();
            foreach (var combo in combos)
            {
                if (combo.ComboProductPrices != null && combo.ComboProductPrices.Count > 0)
                {
                    var checkProductInCombo = combo.ComboProductPrices.FirstOrDefault(x => x.ProductPrice?.ProductId == request.ProductId);
                    if (checkProductInCombo != null)
                    {
                        listCombo.Add(new ComboInFoModel { Id = combo.Id, Name = combo.Name });
                    }
                }
                else
                {
                    foreach (var comboProductGroup in combo.ComboProductGroups)
                    {
                        var checkProductInCombo = comboProductGroup.ComboProductGroupProductPrices.FirstOrDefault(x => x.ProductPrice?.ProductId == request.ProductId);
                        if (checkProductInCombo != null)
                        {
                            listCombo.Add(new ComboInFoModel { Id = combo.Id, Name = combo.Name });
                            break;
                        }
                    }
                }
            }


            return new GetProductInComboByProductlIdResponse
            {
                Combos = listCombo,
                IsEditProduct = listCombo.Count > 0 ? false : true
            };
        }
    }
}
