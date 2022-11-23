using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.OnlineStoreMenus.Queries
{
    public class GetMenuItemReferenceToPageByPageIdRequest : IRequest<List<GetMenuItemReferenceToPageByPageIdResponse>>
    {
        public Guid PageId { get; set; }
    }

    public class GetMenuItemReferenceToPageByPageIdResponse
    {
        public Guid MenuItemId { get; set; }

        public string MenuItemName { get; set; }
    }

    public class GetMenuItemReferenceToPageByPageIdRequestHandler : IRequestHandler<GetMenuItemReferenceToPageByPageIdRequest, List<GetMenuItemReferenceToPageByPageIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        public GetMenuItemReferenceToPageByPageIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<List<GetMenuItemReferenceToPageByPageIdResponse>> Handle(GetMenuItemReferenceToPageByPageIdRequest request, CancellationToken cancellationToken)
        {
            List<GetMenuItemReferenceToPageByPageIdResponse> response = new List<GetMenuItemReferenceToPageByPageIdResponse>();
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            List<GetMenuItemReferenceToPageByPageIdResponse> onlineStoreMenuItemEntity = await _unitOfWork.OnlineStoreMenuItems
                .Where(a => a.OnlineStoreMenu.StoreId == loggedUser.StoreId.Value && a.HyperlinkOption == EnumHyperlinkMenu.MyPages && a.Url.Contains(request.PageId.ToString()))
                .Select(a => new GetMenuItemReferenceToPageByPageIdResponse()
                {
                    MenuItemId = a.Id,
                    MenuItemName = a.Name,
                })
                .ToListAsync();

            return response;
        }
    }
}
