using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Option;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Options.Queries
{
    public class GetOptionByIdRequest : IRequest<GetOptionByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetOptionByIdResponse
    {
        public OptionByIdModel Option { get; set; }
    }

    public class GetOptionByIdRequestHandler : IRequestHandler<GetOptionByIdRequest, GetOptionByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOptionByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetOptionByIdResponse> Handle(GetOptionByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var optionData = await _unitOfWork.Options.GetOptionDetailByIdAsync(request.Id.Value, loggedUser.StoreId);
            ThrowError.Against(optionData == null, "Cannot find option information");
            optionData.OptionLevel = optionData.OptionLevel.OrderBy(x => x.Quota).ToList();
            var option = _mapper.Map<OptionByIdModel>(optionData);

            return new GetOptionByIdResponse
            {
                Option = option
            };
        }
    }
}
