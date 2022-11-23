using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetThemesRequest : IRequest<GetThemesResponse>
    {
    }

    public class GetThemesResponse
    {
        public IEnumerable<ThemeModel> Themes { get; set; }
    }

    public class GetThemesHandler : IRequestHandler<GetThemesRequest, GetThemesResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetThemesHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetThemesResponse> Handle(GetThemesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            var defaultThemeId = new Guid();

            var checkThemeByStoreId = await _unitOfWork.StoreThemes.Find(x => x.StoreId == loggedUser.StoreId).FirstOrDefaultAsync();
            if (checkThemeByStoreId == null)
            {
                var storeThemeAdd = new StoreTheme();
                storeThemeAdd.ThemeId = EnumTheme.Default.ToGuid();
                storeThemeAdd.StoreId = loggedUser.StoreId.Value;
                await _unitOfWork.StoreThemes.AddAsync(storeThemeAdd);
                defaultThemeId = storeThemeAdd.ThemeId;
            }
            else
            {
                defaultThemeId = checkThemeByStoreId.ThemeId;
            }

            var listThemes = await _unitOfWork.Themes.GetAll().ToListAsync();

            var result = _mapper.Map<List<ThemeModel>>(listThemes);
            foreach (var theme in result)
            {
                if (theme.Id == defaultThemeId)
                {
                    theme.IsDefault = true;
                }
            }

            var response = new GetThemesResponse()
            {
                Themes = result
            };

            return response;
        }
    }
}
