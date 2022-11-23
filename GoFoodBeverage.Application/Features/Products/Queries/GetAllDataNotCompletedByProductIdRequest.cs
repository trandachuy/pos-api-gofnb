using AutoMapper;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Combo;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllDataNotCompletedByProductIdRequest : IRequest<GetAllDataNotCompletedByProductIdResponse>
    {
        public Guid ProductId { get; set; }
    }

    public class GetAllDataNotCompletedByProductIdResponse
    {
        public PreventDeleteProductModel PreventDeleteProduct { get; set; }
    }

    public class GetAllDataNotCompletedByProductIdHandler : IRequestHandler<GetAllDataNotCompletedByProductIdRequest, GetAllDataNotCompletedByProductIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;

        public GetAllDataNotCompletedByProductIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IDateTimeService dateTimeService
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTimeService = dateTimeService;
        }

        public async Task<GetAllDataNotCompletedByProductIdResponse> Handle(GetAllDataNotCompletedByProductIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var product = _unitOfWork.Products
                .GetProductByIdInStore(loggedUser.StoreId.Value, request.ProductId)
                .Include(p => p.ProductPrices)
                .FirstOrDefault();
            var productPriceIds = product.ProductPrices.Select(pp => pp.Id).ToList();

            //Check product is in order not complete
            var orders = await _unitOfWork
                .Orders
                .GetAll()
                .Where(o => o.StoreId == loggedUser.StoreId && (o.StatusId != EnumOrderStatus.Completed && o.StatusId != EnumOrderStatus.Draft && o.StatusId != EnumOrderStatus.Canceled
                && o.OrderItems.Any(oi => productPriceIds.Contains(oi.ProductPriceId.Value))))
                .ToListAsync();
            
            if (orders.Any())
            {
                var response = new PreventDeleteProductModel
                {
                    IsPreventDelete = true,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ReasonType = 0,
                    Reasons = new List<PreventDeleteProductModel.Reason>()
                };

                
                orders.ForEach(x =>
                {
                    PreventDeleteProductModel.Reason reason = new PreventDeleteProductModel.Reason();
                    reason.ReasonId = x.Id;
                    reason.ReasonName = x.StringCode;

                    response.Reasons.Add(reason);
                });

                return new GetAllDataNotCompletedByProductIdResponse
                {
                    PreventDeleteProduct = response
                };
            }

            //Check product is in combo
            var combos = await _unitOfWork
                .Combos
                .GetAll()
                .Where(c => c.StoreId == loggedUser.StoreId && (c.ComboProductPrices.Any(cp => productPriceIds.Contains(cp.ProductPriceId))
                || c.ComboPricings.Any(cp => cp.ComboPricingProducts.Any(cpp => productPriceIds.Contains(cpp.ProductPriceId.Value)))))
                .ToListAsync();

            if (combos.Any())
            {
                var response = new PreventDeleteProductModel
                {
                    IsPreventDelete = true,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ReasonType = 1,
                    Reasons = new List<PreventDeleteProductModel.Reason>()
                };

                combos.ForEach(x =>
                {
                    PreventDeleteProductModel.Reason reason = new PreventDeleteProductModel.Reason();
                    reason.ReasonId = x.Id;
                    reason.ReasonName = x.Name;

                    response.Reasons.Add(reason);
                });

                return new GetAllDataNotCompletedByProductIdResponse
                {
                    PreventDeleteProduct = response
                };
            }

            //Check product has promotion
            DateTime today = _dateTimeService.NowUtc;
            var promotions = await _unitOfWork
                .Promotions
                .GetAll()
                .Where(p => p.StoreId == loggedUser.StoreId && (p.PromotionProducts.Any(pp => pp.ProductId == request.ProductId)
                && p.StartDate.CompareTo(today) <= 0 &&
                (p.EndDate.Value.CompareTo(today) >= 0 || !p.EndDate.HasValue) && p.IsStopped == false))
                .ToListAsync();

            if (promotions.Any())
            {
                var response = new PreventDeleteProductModel
                {
                    IsPreventDelete = true,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ReasonType = 2,
                    Reasons = new List<PreventDeleteProductModel.Reason>()
                };

                promotions.ForEach(x =>
                {
                    PreventDeleteProductModel.Reason reason = new PreventDeleteProductModel.Reason();
                    reason.ReasonId = x.Id;
                    reason.ReasonName = x.Name;

                    response.Reasons.Add(reason);
                });

                return new GetAllDataNotCompletedByProductIdResponse
                {
                    PreventDeleteProduct = response
                };
            }

            var respone = new PreventDeleteProductModel
            {
                IsPreventDelete = false,
                ProductId = product.Id,
            };

            return new GetAllDataNotCompletedByProductIdResponse
            {
                PreventDeleteProduct = respone
            };
        }
    }
}
