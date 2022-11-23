using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Page;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Pages.Queries
{
    public class GetPageByIdRequest : IRequest<GetPageByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetPageByIdResponse
    {
        public bool IsSuccess { get; set; }

        public PageModel Page { get; set; }
    }

    public class GetPageByIdRequestHandler : IRequestHandler<GetPageByIdRequest, GetPageByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPageByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetPageByIdResponse> Handle(GetPageByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var Page = await _unitOfWork.Pages
                .GetPageByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);

            var PageDetail = _mapper.Map<PageModel>(Page);

            return new GetPageByIdResponse
            {
                IsSuccess = true,
                Page = PageDetail
            };
        }
    }
}
