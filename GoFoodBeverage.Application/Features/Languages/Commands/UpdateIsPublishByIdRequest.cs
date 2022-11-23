using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Languages.Commands
{
    public class UpdateIsPublishByIdRequest : IRequest<bool>
    {
        public Guid? Id { get; set; }

        public bool IsPublish { get; set; }
    }

    public class UpdateIsPublishByIdRequestHandler : IRequestHandler<UpdateIsPublishByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateIsPublishByIdRequestHandler(
            IUnitOfWork unitOfWork
        )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateIsPublishByIdRequest request, CancellationToken cancellationToken)
        {
            LanguageStore languageStore = await _unitOfWork.LanguageStores.GetLanguageStoreById(request?.Id);
            languageStore.IsPublish = request.IsPublish;
            await _unitOfWork.LanguageStores.UpdateAsync(languageStore);

            return true;

        }

    }

}
