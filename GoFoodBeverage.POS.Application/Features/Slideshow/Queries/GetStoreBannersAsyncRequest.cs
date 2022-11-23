using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Slideshow.Queries
{
    public class GetStoreBannersAsyncRequest : IRequest<GetStoreBannersAsyncResponse>
    {
        public EnumBannerType Type { get; set; }
    }

    public class GetStoreBannersAsyncResponse
    {
        public IEnumerable<BannerInfor> Banners { get; set; }
    }

    public class BannerInfor
    {
        public EnumBannerType Type { get; set; }
        public string FileUrl { get; set; }
    }

    public class GetStoreBannersRequestHandler : IRequestHandler<GetStoreBannersAsyncRequest, GetStoreBannersAsyncResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetStoreBannersRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetStoreBannersAsyncResponse> Handle(GetStoreBannersAsyncRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var banners = await _unitOfWork.StoreBanners
                .Find(x => x.StoreId == loggedUser.StoreId && x.Type == request.Type)
                .AsNoTracking()
                .Select(x => new BannerInfor
                {
                    Type = request.Type,
                    FileUrl = x.Thumbnail
                })
                .ToListAsync(cancellationToken);

            var response = new GetStoreBannersAsyncResponse
            {
                Banners = banners
            };

            return response;
        }
    }
}