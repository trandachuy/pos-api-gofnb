using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetStoreBannersAsyncRequest : IRequest<GetStoreBannersResponse>
    {
        public EnumBannerType BannerType { get; set; }
    }

    public class GetStoreBannersResponse
    {
        public IEnumerable<string> Thumbnails { get; set; }
    }

    public class GetStoreBannersRequestHandler : IRequestHandler<GetStoreBannersAsyncRequest, GetStoreBannersResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetStoreBannersRequestHandler(IUnitOfWork unitOfWork, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetStoreBannersResponse> Handle(GetStoreBannersAsyncRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(loggedUser == null, "Cannot find user information");

            var storeBanner = await _unitOfWork.StoreBanners
                .Find(x => x.StoreId == loggedUser.StoreId && x.Type == request.BannerType)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            var thumnails = storeBanner.Select(x => x.Thumbnail).ToList();

            var response = new GetStoreBannersResponse()
            {
                Thumbnails = thumnails,
            };

            return response;
        }
    }
}