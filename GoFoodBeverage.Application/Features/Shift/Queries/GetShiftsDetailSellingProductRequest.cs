using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using System.Linq;
using System;
using GoFoodBeverage.Models.Shift;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Shift.Queries
{
    public class GetShiftsDetailSellingProductRequest : IRequest<GetShiftsDetailSellingProductResponse>
    {
        public Guid? ShiftId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetShiftsDetailSellingProductResponse
    {
        public SellingProductsModel SellingProducts { get; set; }

        public int Total { get; set; }
    }

    public class GetShiftDetailSellingProductHandler : IRequestHandler<GetShiftsDetailSellingProductRequest, GetShiftsDetailSellingProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _iDateTimeService;

        public GetShiftDetailSellingProductHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            IDateTimeService iDateTimeService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _iDateTimeService = iDateTimeService;
        }

        public async Task<GetShiftsDetailSellingProductResponse> Handle(GetShiftsDetailSellingProductRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var result = new SellingProductsModel();

            result.SellingProductTable = new List<SellingProductTableModel>();
            var totalProducts = new List<Product>();
            var listOrder = _unitOfWork.Orders.Find(x => x.StoreId == loggedUser.StoreId && x.ShiftId == request.ShiftId).Include(p => p.OrderItems);
            foreach (var order in listOrder)
            {
                var listProductPriceIds = order.OrderItems.Select(x => x.ProductPriceId);
                var listProductIds = _unitOfWork.ProductPrices.Find(x => x.StoreId == loggedUser.StoreId && listProductPriceIds.Contains(x.Id)).Select(x => x.ProductId);
                var listProducts = _unitOfWork.Products.Find(x => x.StoreId == loggedUser.StoreId && listProductIds.Contains(x.Id));
                if (listProducts.Count() > 0)
                {
                    totalProducts.AddRange(listProducts);
                }
            }

            var totalProductsDistinct = totalProducts.Distinct();
            var index = 0;
            var totalQuantity = 0;
            foreach (var product in totalProductsDistinct)
            {
                var itemResult = new SellingProductTableModel();
                itemResult.No = index + ((request.PageNumber - 1) * request.PageSize) + 1;
                itemResult.ProductName = product.Name;
                itemResult.Quantity = totalProducts.Count(x => x.Id == product.Id);
                totalQuantity = totalQuantity + itemResult.Quantity;
                result.SellingProductTable.Add(itemResult);
                index++;
            }

            var totalSellingProducts = result.SellingProductTable.Count;
            result.SellingProductTable = result.SellingProductTable.ToPagination(request.PageNumber, request.PageSize).Result.ToList();
            result.TotalQuantity = totalQuantity;

            var response = new GetShiftsDetailSellingProductResponse()
            {
                SellingProducts = result,
                Total = totalSellingProducts
            };

            return response;
        }
    }
}
