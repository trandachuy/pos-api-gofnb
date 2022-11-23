using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class UpdateStoreBannerAsyncRequest : IRequest<UpdateStoreBannerResponse>
    {
        public Guid StoreId { get; }
        public EnumBannerType Type { get; set; }
        public List<string> Thumbnails { get; set; }
    }

    public class UpdateStoreBannerResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Thumbnails { get; set; }
    }

    public class UpdateStoreBannerRequestHandler : IRequestHandler<UpdateStoreBannerAsyncRequest, UpdateStoreBannerResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateStoreBannerRequestHandler(IUnitOfWork unitOfWork, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<UpdateStoreBannerResponse> Handle(UpdateStoreBannerAsyncRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(loggedUser == null, "Cannot find user information");
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var existStoreBanners = await _unitOfWork.StoreBanners.Find(x => x.StoreId == loggedUser.StoreId && x.Type == request.Type).ToListAsync();
            var existStoreBannersThumnbs = existStoreBanners.Select(x => x.Thumbnail).ToList();

            var response = new UpdateStoreBannerResponse();

            //Add or Delete banner (FullScreen / LeftSide)
            if (request.Thumbnails.Any())
            {
                var listRemoveBanners = existStoreBannersThumnbs.Except(request.Thumbnails).ToList();
                var listAddNewBanners = request.Thumbnails.Except(existStoreBannersThumnbs).ToList();

                //Remove
                if (listRemoveBanners.Any())
                {
                    var listRemoveStoreBanner = existStoreBanners.Where(x => listRemoveBanners.Contains(x.Thumbnail));
                    _unitOfWork.StoreBanners.RemoveRange(listRemoveStoreBanner);
                }

                //Add new
                if (listAddNewBanners.Any())
                {
                    var listAddNewStoreBanner = new List<StoreBanner>();
                    listAddNewBanners.ForEach(thumb =>
                    {
                        var storeBanner = new StoreBanner
                        {
                            StoreId = loggedUser.StoreId.Value,
                            Type = request.Type,
                            Thumbnail = thumb
                        };
                        listAddNewStoreBanner.Add(storeBanner);
                    });
                    _unitOfWork.StoreBanners.AddRange(listAddNewStoreBanner);
                }
            }
            //Remove all exists banners (FullScreen / LeftSide)
            else
            {
                _unitOfWork.StoreBanners.RemoveRange(existStoreBanners);
            }

            await _unitOfWork.SaveChangesAsync();
            var listReturnThumbs = await _unitOfWork.StoreBanners
                    .Find(x => x.StoreId == loggedUser.StoreId && x.Type == request.Type)
                    .OrderByDescending(x => x.CreatedTime)
                    .Select(x => x.Thumbnail).ToListAsync();

            response.Success = true;
            response.Thumbnails = listReturnThumbs;

            return response;
        }
    }
}