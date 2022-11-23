using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Languages.Commands
{
    public class CreateLanguageStoreRequest : IRequest<CreateLanguageStoreResponse>
    {
        public Guid LanguageId { get; set; }
    }

    public class CreateLanguageStoreResponse
    {
        public string LanguageName { get; set; }
    }

    public class CreateLanguageStoreRequestHandler : IRequestHandler<CreateLanguageStoreRequest, CreateLanguageStoreResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateLanguageStoreRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<CreateLanguageStoreResponse> Handle(CreateLanguageStoreRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidate(request);

            var language = _unitOfWork.Languages.GetLanguageById(request.LanguageId);
            var languageStore = CreateLanguageStore(request, language, loggerUser.StoreId.Value);
            await _unitOfWork.LanguageStores.AddAsync(languageStore);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            var response = new CreateLanguageStoreResponse
            {
                LanguageName = languageStore.Language.Name
            };

            return response;
        }

        private static void RequestValidate(CreateLanguageStoreRequest request)
        {
            ThrowError.Against(request.LanguageId.Equals(null), "Please choose language name");
        }

        private static LanguageStore CreateLanguageStore(CreateLanguageStoreRequest resquest, Language language, Guid storeId)
        {
            var result = new LanguageStore()
            {
                LanguageId = resquest.LanguageId,
                StoreId = storeId,
                IsPublish = false,
                Language = language
            };

            return result;
        }
    }
}
