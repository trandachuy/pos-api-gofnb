using System;
using MediatR;
using MoreLinq;
using System.Linq;
using System.Threading;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Options.Commands
{
    public class UpdateOptionRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? MaterialId { get; set; }

        public List<OptionLevelRequest> OptionLevels { get; set; }

        public class OptionLevelRequest
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public Guid OptionId { get; set; }

            public bool SetDefault { get; set; }

            public decimal? Quota { get; set; }
        }
    }

    public class UpdateOptionRequestHandler : IRequestHandler<UpdateOptionRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateOptionRequestHandler(
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

        public async Task<bool> Handle(UpdateOptionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var option = await _unitOfWork.Options.GetOptionDetailByIdAsync(request.Id, loggedUser.StoreId);
            ThrowError.Against(option == null, "Cannot find option information");

            var optionNameExisted = await _unitOfWork.Options.CheckExistOptionNameInStoreAsync(request.Id, request.Name, loggedUser.StoreId.Value);
            ThrowError.Against(optionNameExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}", "Option name has already existed" },
            });

            RequestValidation(request);

            var modifiedOption = await UpdateOptionAsync(option, request);

            await _unitOfWork.Options.UpdateAsync(modifiedOption);

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Option,
                ActionType = EnumActionType.Edited,
                ObjectId = modifiedOption.Id,
                ObjectName = modifiedOption.Name.ToString()
            });

            return true;
        }

        private static void RequestValidation(UpdateOptionRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}", "Please enter option name" },
            });

            ThrowError.Against(request.OptionLevels.Count == 0, "Please enter option level.");
        }

        public async Task<Option> UpdateOptionAsync(Option currentOption, UpdateOptionRequest request)
        {
            currentOption.Name = request.Name;
            currentOption.MaterialId = request.MaterialId;

            #region Update option level
            var currentOptionLevels = currentOption.OptionLevel.ToList();
            var newOptionLevels = new List<OptionLevel>();
            var updateOptionLevels = new List<OptionLevel>();

            if (request.OptionLevels != null && request.OptionLevels.Any())
            {
                var optionLevelDeleteItems = currentOptionLevels
                    .Where(x => x.OptionId == currentOption.Id && !request.OptionLevels.Select(ol => ol.Id).Contains(x.Id));
                if (optionLevelDeleteItems.Any())
                {
                    _unitOfWork.OptionLevels.RemoveRange(optionLevelDeleteItems);
                }

                foreach (var optionLevel in request.OptionLevels)
                {
                    var optionLevelExisted = currentOptionLevels.FirstOrDefault(x => x.Id == optionLevel.Id);
                    if (optionLevelExisted == null)
                    {
                        if (request.MaterialId != null)
                        {
                            var newOptionLevel = new OptionLevel()
                            {
                                Name = optionLevel.Name,
                                OptionId = currentOption.Id,
                                IsSetDefault = optionLevel.SetDefault,
                                Quota = optionLevel.Quota,
                                StoreId = currentOption.StoreId,
                            };
                            newOptionLevels.Add(newOptionLevel);
                        }
                        else
                        {
                            var newOptionLevel = new OptionLevel()
                            {
                                Name = optionLevel.Name,
                                OptionId = currentOption.Id,
                                IsSetDefault = optionLevel.SetDefault,
                                StoreId = currentOption.StoreId,
                            };
                            newOptionLevels.Add(newOptionLevel);
                        }
                    }
                    else
                    {
                        optionLevelExisted.Name = optionLevel.Name;
                        optionLevelExisted.IsSetDefault = optionLevel.SetDefault;
                        optionLevelExisted.Quota = request.MaterialId == null? null: optionLevel.Quota;
                        updateOptionLevels.Add(optionLevelExisted);
                    }
                }

                _unitOfWork.OptionLevels.AddRange(newOptionLevels);
                _unitOfWork.OptionLevels.UpdateRange(updateOptionLevels);

                await _unitOfWork.SaveChangesAsync();
            }
            #endregion

            return currentOption;
        }
    }
}
