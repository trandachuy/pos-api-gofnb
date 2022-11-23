using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.EmailCampaigns.Queries
{
    public class GetAllEmailCampaignRequest : IRequest<GetAllEmailCampaignResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetAllEmailCampaignResponse
    {
        public IEnumerable<EmailCampaignModel> EmailCampaigns { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetAllEmailCampaignRequestHandler : IRequestHandler<GetAllEmailCampaignRequest, GetAllEmailCampaignResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllEmailCampaignRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllEmailCampaignResponse> Handle(GetAllEmailCampaignRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var listEmailCampaign = new PagingExtensions.Pager<EmailCampaignModel>(new List<EmailCampaignModel>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                listEmailCampaign = await _unitOfWork.EmailCampaigns
                .GetAllEmailCampaignInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ProjectTo<EmailCampaignModel>(_mapperConfiguration)
                .ToPaginationAsync(request.PageNumber, request.PageSize);

            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                listEmailCampaign = await _unitOfWork.EmailCampaigns
                .GetAllEmailCampaignInStore(loggedUser.StoreId.Value)
                .Where(qr => qr.Name.ToLower().Contains(keySearch))
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ProjectTo<EmailCampaignModel>(_mapperConfiguration)
                .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var response = new GetAllEmailCampaignResponse()
            {
                EmailCampaigns = listEmailCampaign.Result,
                PageNumber = request.PageNumber,
                Total = listEmailCampaign.Total
            };

            return response;
        }
    }
}
