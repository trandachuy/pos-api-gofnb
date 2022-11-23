using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using Newtonsoft.Json.Linq;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Options.Commands
{
    public class CreateOptionRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public Guid? MaterialId { get; set; }

        public List<OptionLevelRequest> OptionLevels { get; set; }

        public class OptionLevelRequest
        {
            public string Name { get; set; }

            public Guid OptionId { get; set; }

            public bool SetDefault { get; set; }

            public decimal? Quota { get; set; }
        }
    }

    public class CreateOptionRequestHandler : IRequestHandler<CreateOptionRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateOptionRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateOptionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            await RequestValidation(request, loggedUser.StoreId.Value);

            if (request.MaterialId != null)
            {
                var materialIdExisted = await _unitOfWork.Materials.Find(g => g.StoreId == loggedUser.StoreId && g.Id == request.MaterialId).FirstOrDefaultAsync();
                ThrowError.Against(materialIdExisted == null, new JObject()
                {
                    { $"{nameof(request.MaterialId)}", "Cannot find material information" },
                });
            }

            var storeConfig = await _unitOfWork.StoreConfigs.GetStoreConfigByStoreIdAsync(loggedUser.StoreId.Value);
            var newOption = CreateOption(
                request,
                store,
                storeConfig.CurrentMaxOptionCode.GetCode(StoreConfigConstants.OPTION_CODE));
            _unitOfWork.Options.Add(newOption);
            await _unitOfWork.SaveChangesAsync();

            // update store configure
            await _unitOfWork.StoreConfigs.UpdateStoreConfigAsync(storeConfig, StoreConfigConstants.OPTION_CODE);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Option,
                ActionType = EnumActionType.Created,
                ObjectId = newOption.Id,
                ObjectName = newOption.Name.ToString()
            }, cancellationToken);

            return true;
        }

        private async Task RequestValidation(CreateOptionRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}", "Please enter name of option." },
            });

            ThrowError.Against(request.OptionLevels.Count == 0, "Please enter option level.");

            var optionNameExisted = await _unitOfWork.Options
                .GetAllOptionsInStore(storeId)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            ThrowError.Against(optionNameExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}", "Option name is already existed" },
            });
        }

        private static Option CreateOption(CreateOptionRequest request, Store store, string code)
        {
            var newOption = new Option()
            {
                Code = code,
                StoreId = store.Id,
                Name = request.Name,
                MaterialId = request.MaterialId,
                OptionLevel = new List<OptionLevel>()
            };

            request.OptionLevels.ForEach(p =>
            {
                if (request.MaterialId != null)
                {
                    var optionLevel = new OptionLevel()
                    {
                        Name = p.Name,
                        OptionId = newOption.Id,
                        IsSetDefault = p.SetDefault,
                        Quota = p.Quota,
                        StoreId = store.Id,
                    };
                    newOption.OptionLevel.Add(optionLevel);
                }
                else
                {
                    var optionLevel = new OptionLevel()
                    {
                        Name = p.Name,
                        OptionId = newOption.Id,
                        IsSetDefault = p.SetDefault,
                        StoreId = store.Id,
                    };
                    newOption.OptionLevel.Add(optionLevel);
                }
            });

            return newOption;
        }
    }
}
