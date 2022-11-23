using System;
using MediatR;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class CreateMaterialCategoryRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public List<Guid> MaterialIds { get; set; }
    }

    public class CreateMaterialCategoryRequestHandler : IRequestHandler<CreateMaterialCategoryRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public CreateMaterialCategoryRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(CreateMaterialCategoryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var materialCategoryNameExisted = await _unitOfWork.MaterialCategories
                .GetAllMaterialCategoriesInStore(store.Id)
                .FirstOrDefaultAsync(m => m.Name.ToLower().Equals(request.Name.Trim().ToLower()), cancellationToken: cancellationToken);

            ThrowError.Against(materialCategoryNameExisted != null, new JObject()
                {
                    { $"{nameof(request.Name)}", "materialCategory.nameCategoryExisted" },
                });

            var newMaterialCategory = await CreateMaterialCategoryAndAddMaterialToCategoryAsync(request, store);
            _unitOfWork.MaterialCategories.Add(newMaterialCategory);

            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.MaterialCategory,
                ActionType = EnumActionType.Created,
                ObjectId = newMaterialCategory.Id,
                ObjectName = newMaterialCategory.Name.ToString()
            });

            return true;
        }

        private async Task<MaterialCategory> CreateMaterialCategoryAndAddMaterialToCategoryAsync(CreateMaterialCategoryRequest request, Store store)
        {
            var newMaterialCategory = new MaterialCategory()
            {
                StoreId = store.Id,
                Name = request.Name,
            };

            var materials = await _unitOfWork.Materials.GetAllMaterialsInStoreByIds(store.Id, request.MaterialIds).ToListAsync();
            materials.ForEach(material =>
            {
                material.MaterialCategoryId = newMaterialCategory.Id;
            });

            _unitOfWork.Materials.UpdateRange(materials);

            return newMaterialCategory;
        }
    }
}
