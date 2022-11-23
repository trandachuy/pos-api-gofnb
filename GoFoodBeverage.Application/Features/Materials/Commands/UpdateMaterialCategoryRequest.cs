using System;
using MediatR;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class UpdateMaterialCategoryRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Guid> MaterialIds { get; set; }
    }

    public class UpdateMaterialCategoryRequestHandler : IRequestHandler<UpdateMaterialCategoryRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateMaterialCategoryRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdateMaterialCategoryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var materialCategory = await _unitOfWork.MaterialCategories.GetMaterialCategoryByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);
            ThrowError.Against(materialCategory == null, "Cannot find material category information");

            /// Check for the existence of a material category name in current store
            var materialCategoryNameExisted = await _unitOfWork.MaterialCategories
                .GetAllMaterialCategoriesInStore(store.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id != request.Id && m.Name.ToLower().Equals(request.Name.Trim().ToLower()), cancellationToken: cancellationToken);
            
            ThrowError.Against(materialCategoryNameExisted != null, new JObject()
                {
                    { $"{nameof(request.Name)}", "materialCategory.nameCategoryExisted" },
                });

            var newMaterialCategory = await UpdateMaterialCategoryAsync(materialCategory, request, loggedUser);
            _unitOfWork.MaterialCategories.Update(newMaterialCategory);

            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.MaterialCategory,
                ActionType = EnumActionType.Edited,
                ObjectId = newMaterialCategory.Id,
                ObjectName = newMaterialCategory.Name.ToString()
            });

            return true;
        }

        public async Task<MaterialCategory> UpdateMaterialCategoryAsync(MaterialCategory materialCategory, UpdateMaterialCategoryRequest request, LoggedUserModel loggedUser)
        {
            materialCategory.Name = request.Name;
            materialCategory.LastSavedUser = loggedUser.AccountId;

            /// Remove material from category
            var oldMaterialsOfCategory = await _unitOfWork.Materials.GetAllMaterialsByCategoryId(loggedUser.StoreId, materialCategory.Id).ToListAsync();
            oldMaterialsOfCategory.ForEach(m =>
            {
                m.MaterialCategoryId = null;
            });

            _unitOfWork.Materials.UpdateRange(oldMaterialsOfCategory);

            /// Add material to category
            var materials = await _unitOfWork.Materials.Find(p => p.StoreId == loggedUser.StoreId && request.MaterialIds.Any(id => id == p.Id)).ToListAsync();
            materials.ForEach(material =>
            {
                material.MaterialCategoryId = materialCategory.Id;
            });

           _unitOfWork.Materials.UpdateRange(materials);

            return materialCategory;
        }
    }
}
