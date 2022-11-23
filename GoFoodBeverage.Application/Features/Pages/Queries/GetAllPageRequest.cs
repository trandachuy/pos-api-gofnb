using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Page;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Pages.Queries
{
    public class GetAllPageRequest : IRequest<GetAllPageResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetAllPageResponse
    {
        public IEnumerable<PageModel> Pages { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetAllPageRequestHandler : IRequestHandler<GetAllPageRequest, GetAllPageResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPageRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetAllPageResponse> Handle(GetAllPageRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var listPages = await _unitOfWork.Pages
            .GetAllPagesInStore(loggedUser.StoreId.Value)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedTime)
            .ToPaginationAsync(request.PageNumber, request.PageSize);

            var pages = _mapper.Map<List<PageModel>>(listPages.Result);

            if (!string.IsNullOrEmpty(request.KeySearch) && listPages != null)
            {
                string keySearch = StringHelpers.RemoveSign4VietnameseString(request.KeySearch).Trim().ToLower();
                pages = pages.Where(x => StringHelpers.RemoveSign4VietnameseString(x.PageName).Trim().ToLower().Contains(keySearch)).ToList();
            }

            if (pages.Any())
            {
                var listStaffIds = pages.Where(x => x.CreatedUser.HasValue).Select(x => x.CreatedUser.Value).Distinct().ToList();
                if (listStaffIds.Any())
                {
                    var allStaff = await _unitOfWork.Staffs
                    .Find(x => x.StoreId == loggedUser.StoreId && listStaffIds.Contains(x.AccountId))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                    if (allStaff.Any())
                    {
                        pages.ForEach(x =>
                        {
                            x.CreatedBy = allStaff.Where(y => y.AccountId == x.CreatedUser).FirstOrDefault()?.FullName;
                        });
                    }
                }
            }
            var response = new GetAllPageResponse()
            {
                Pages = pages,
                Total = listPages.Total,
                PageNumber = request.PageNumber
            };

            return response;
        }
    }
}
