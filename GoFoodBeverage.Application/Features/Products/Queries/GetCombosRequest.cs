using MediatR;  
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Product;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Extensions;
using System.Linq;
using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetCombosRequest : IRequest<GetCombosResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetCombosResponse
    {
        public IEnumerable<ComboDataTableModel> Combos { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetCombosRequestHandler : IRequestHandler<GetCombosRequest, GetCombosResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDateTimeService _dateTime;

        public GetCombosRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration,
            IDateTimeService dateTime)
        {
            _mapper = mapper; 
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
            _dateTime = dateTime;
    }

        public async Task<GetCombosResponse> Handle(GetCombosRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var allCombo = await _unitOfWork.Combos
                    .GetAllCombosInStore(loggedUser.StoreId.Value)
                    .AsNoTracking()
                    .Include(c => c.ComboStoreBranches)
                    .Include(c => c.ComboProductPrices).ThenInclude(pr => pr.ProductPrice).ThenInclude(p => p.Product)
                    .Include(c => c.ComboPricings.OrderBy(cp => cp.CreatedTime)).ThenInclude(cp => cp.ComboPricingProducts).ThenInclude(cpp => cpp.ProductPrice).ThenInclude(p => p.Product)
                    .OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);

            var listAllCombo = allCombo.Result;
            var comboListResponse = _mapper.Map<List<ComboDataTableModel>>(listAllCombo);
            var allBranches = await _unitOfWork.StoreBranches
                                    .GetStoreBranchesByStoreId(loggedUser.StoreId)
                                    .AsNoTracking()
                                    .ProjectTo<ComboDataTableModel.StoreBranch>(_mapperConfiguration)
                                    .ToListAsync(cancellationToken: cancellationToken);

            comboListResponse.ForEach(c =>
            {
                if(c.IsShowAllBranches)
                {
                    c.Branch = allBranches;
                }

                if (c.IsStopped == true)
                {
                    c.StatusId = EnumComboStatus.Finished;
                }
                else
                {
                    c.StatusId = GetComboStatus(c.StartDate, c.EndDate);
                }
            });

            var response = new GetCombosResponse()
            {
                PageNumber = request.PageNumber,
                Total = allCombo.Total,
                Combos = comboListResponse
            };

            return response;
        }

        private EnumComboStatus GetComboStatus(DateTime startDate, DateTime? dueDate)
        {
            var result = _dateTime.NowUtc.Date.CompareTo(startDate.Date);

            if (result < 0)
            {
                return EnumComboStatus.Scheduled;
            }
            else if (result == 0)
            {
                return EnumComboStatus.Active;
            }
            else
            {
                if (dueDate.HasValue)
                {
                    return _dateTime.NowUtc.Date.CompareTo(dueDate.Value.Date) >= 0 ? EnumComboStatus.Finished : EnumComboStatus.Active;
                }
                else
                {
                    return EnumComboStatus.Active;
                }
            }
        }
    }
}
