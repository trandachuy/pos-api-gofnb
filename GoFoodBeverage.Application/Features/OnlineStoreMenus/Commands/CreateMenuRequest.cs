using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.OnlineStoreMenus.Commands
{
    public class CreateMenuRequest : IRequest<bool>
    {
        public string MenuName { get; set; }

        public EnumLevelMenu LevelId { get; set; }

        public IEnumerable<MenuItemDto> MenuItems { get; set; }

        public class MenuItemDto
        {
            public Guid Id { get; set; }

            public Guid MenuId { get; set; }

            public string MenuItemName { get; set; }

            public EnumHyperlinkMenu HyperlinkOption { get; set; }

            public string Url { get; set; }

            public int Position { get; set; }
        }
    }

    public class CreateMenuRequestHandler : IRequestHandler<CreateMenuRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateMenuRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateMenuRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            ThrowError.Against(string.IsNullOrEmpty(request.MenuName), "Please enter online store menu name");

            var menuNameExisted = await _unitOfWork.OnlineStoreMenus.GetMenuNameInStoreAsync(request.MenuName, loggedUser.StoreId.Value);
            ThrowError.Against(menuNameExisted != null, new JObject()
            {
                { $"{nameof(request.MenuName)}", "Online store menu has already existed" },
            });

            var newMenu = new OnlineStoreMenu()
            {
                StoreId = loggedUser.StoreId.Value,
                Name = request.MenuName,
                Level = request.LevelId,
                IsDefault = false,
                OnlineStoreMenuItems = new List<OnlineStoreMenuItem>(),
            };

            if (request.MenuItems != null && request.MenuItems.Any())
            {
                foreach (var menuItem in request.MenuItems)
                {
                    var newMenuItem = new OnlineStoreMenuItem()
                    {
                        MenuId = newMenu.Id,
                        Id = menuItem.Id,
                        Name = menuItem.MenuItemName,
                        HyperlinkOption = menuItem.HyperlinkOption,
                        Url = menuItem.Url,
                        Position = menuItem.Position,
                        SubMenuId = menuItem.HyperlinkOption == EnumHyperlinkMenu.SubMenu ? menuItem.Url.ToGuid() : null,
                    };
                    newMenu.OnlineStoreMenuItems.Add(newMenuItem);
                }
            }

            _unitOfWork.OnlineStoreMenus.Add(newMenu);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }
    }
}
