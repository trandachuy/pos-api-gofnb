using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Package;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static GoFoodBeverage.Common.Extensions.PagingExtensions;

namespace GoFoodBeverage.Application.Features.Package.Queries
{
    public class GetOrderPackagesRequest : IRequest<GetOrderPackagesResponse>
    {
        public int Page { get; set; }

        public int Size { get; set; }

        public bool Paged { get; set; }

        /// <summary>
        /// Example: Id,desc
        /// Description: sorting "desc" for the column "Id"
        /// </summary>
        public string Sort { get; set; }

        [BindProperty(Name = "fromDate.greaterOrEqualThan")]
        public string FromDateGreaterOrEqualThan { get; set; }

        [BindProperty(Name = "toDate.lessOrEqualThan")]
        public string ToDateLessOrEqualThan { get; set; }

        public string Status { get; set; }
    }

    public class GetOrderPackagesResponse
    {
        public int TotalCount { get; set; }

        public IEnumerable<OrderPackageInternalToolDatatableModel> Data { get; set; }
    }

    public class GetOrderPackagesRequestHandler : IRequestHandler<GetOrderPackagesRequest, GetOrderPackagesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderPackagesRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetOrderPackagesResponse> Handle(GetOrderPackagesRequest request, CancellationToken cancellationToken)
        {
            /// Get order packages from database
            var orderPackages = _unitOfWork.OrderPackages.GetAll();

            if (request.Page == 0) request.Page = 1;
            if (request.Size == 0) request.Size = 20;

            if (request.Paged == true)
            {
                var orderPackagesPaged = await PaginationAsync(orderPackages, request);
                var responseData = _mapper.Map<List<OrderPackageInternalToolDatatableModel>>(orderPackagesPaged.Result);
                var response = new GetOrderPackagesResponse()
                {
                    TotalCount = orderPackagesPaged.Total,
                    Data = responseData
                };

                return response;
            }
            else
            {
                request.Page = 1;
                request.Size = orderPackages.AsNoTracking().Count();
                var orderPackagesPaged = await PaginationAsync(orderPackages, request);
                var responseData = _mapper.Map<List<OrderPackageInternalToolDatatableModel>>(orderPackagesPaged.Result);
                var response = new GetOrderPackagesResponse()
                {
                    TotalCount = orderPackagesPaged.Total,
                    Data = responseData
                };

                return response;
            }
        }

        private static async Task<Pager<OrderPackage>> PaginationAsync(IQueryable<OrderPackage> query, GetOrderPackagesRequest request)
        {
            if (!string.IsNullOrEmpty(request.FromDateGreaterOrEqualThan))
            {
                _ = DateTime.TryParse(request.FromDateGreaterOrEqualThan, out DateTime dateTimeValue);
                query = query.Where(i => i.CreatedTime.Value.CompareTo(dateTimeValue) >= 0);
            }

            if (!string.IsNullOrEmpty(request.ToDateLessOrEqualThan))
            {
                _ = DateTime.TryParse(request.ToDateLessOrEqualThan, out DateTime dateTimeValue);
                query = query.Where(i => i.CreatedTime.Value.CompareTo(dateTimeValue) <= 0);
            }

            /// Sort by column
            var sort = request.Sort.Split(',');
            var column = CapitalizeFirstLetter(sort[0]);
            var sortType = sort[1];
            var defaultColumn = (column == "Id" || column == "Code") ? "Code" : column;
            if (sortType?.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(defaultColumn);
            }

            if (sortType?.ToUpper() == "ASC")
            {
                query = query.OrderBy(defaultColumn);
            }

            /// Filter by status
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(s => s.Status.Contains(request.Status));
            }

            var pagedResult = await query.ToPaginationAsync(request.Page, request.Size);

            return pagedResult;
        }

        private static string CapitalizeFirstLetter(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0)
            {
                return string.Empty;
            }
            else if (str.Length == 1)
            {
                return char.ToUpper(str[0]).ToString();
            }
            else
            {
                return char.ToUpper(str[0]).ToString() + str.Substring(1);
            }
        }
    }
}
