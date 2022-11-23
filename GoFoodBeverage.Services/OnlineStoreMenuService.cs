using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services
{
    [AutoService(typeof(IOnlineStoreMenuService), Lifetime = ServiceLifetime.Scoped)]
    public class OnlineStoreMenuService : IOnlineStoreMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public OnlineStoreMenuService(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> CreateDataMenuDefaultAsync(Guid storeId)
        {
            var isDataMenuDefaultByStore = _unitOfWork.OnlineStoreMenus
                .Find(m => m.StoreId == storeId && m.IsDefault == true)
                .Any();
            if (!isDataMenuDefaultByStore)
            {
                Guid idMenu = Guid.NewGuid();
                var defaultMenu = new OnlineStoreMenu()
                {
                    Id = idMenu,
                    StoreId = storeId,
                    Name = "Default menu",
                    Level = EnumLevelMenu.Level1,
                    IsDefault = true,
                    OnlineStoreMenuItems = new List<OnlineStoreMenuItem>()
                    {
                        new OnlineStoreMenuItem {
                            MenuId = idMenu,
                            Name = "Home",
                            HyperlinkOption = EnumHyperlinkMenu.HomePage,
                            Url = "",
                            Position = 1
                        },
                        new OnlineStoreMenuItem {
                            MenuId = idMenu,
                            Name = "Product",
                            HyperlinkOption = EnumHyperlinkMenu.Products,
                            Url = "",
                            Position = 2
                        }
                    }
                };

                var newMenu = new OnlineStoreMenu()
                {
                    Id = defaultMenu.Id,
                    StoreId = defaultMenu.StoreId,
                    Name = defaultMenu.Name,
                    Level = defaultMenu.Level,
                    IsDefault = defaultMenu.IsDefault,
                    OnlineStoreMenuItems = defaultMenu.OnlineStoreMenuItems
                };

                _unitOfWork.OnlineStoreMenus.Add(newMenu);

                await _unitOfWork.SaveChangesAsync();

                await _userActivityService.LogAsync(newMenu);

                return true;
            }

            return false;
        }
    }
}
